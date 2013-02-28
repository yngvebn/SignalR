// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

/*global window:false */
/// <reference path="jquery.signalR.core.js" />

(function ($, window) {
    "use strict";

    var signalR = $.signalR,
        hubConnection = $.hubConnection,
        savedConnection = $.signalR.fn.init,
        savedReceived = signalR.prototype.received,
        savedCreateHubProxy = hubConnection.prototype.createHubProxy,
        getContractFromResponse = function (hubName, methodName, contracts) {
            var contractInfo = contracts[0][hubName][methodName];

            return [getContract(contractInfo[0], contracts), contractInfo[1]];
        },
        buildResult = function (hubName, methodName, decompress) {
            return {
                decompress: decompress,
                hubName: hubName,
                methodName: methodName
            };
        },
        getContract = function (contractId, contracts) {
            return contracts[2][contractId];
        },
        isPayload = function (contractId, contracts) {
            return !!getContract(contractId, contracts);
        },
        decompress = function (compressed, contract, contracts) {
            var result;

            if (compressed) {
                result = {};

                $.each(contract, function (i, val) {
                    var propertyName = val[0],
                        compressedTypeId = val[1][0],
                        enumerable = val[1][1],
                        enumerated;

                    // Check the payload type of the parameter, if it's a payload we need to recursively decompress it
                    if (isPayload(compressedTypeId, contracts)) {
                        if (enumerable) {
                            enumerated = [];
                            
                            for (var j = 0; j < compressed[i].length; j++) {
                                enumerated[j] = decompress(compressed[i][j], getContract(compressedTypeId, contracts), contracts);
                            }
                            
                            result[propertyName] = enumerated;
                        }
                        else {
                            result[propertyName] = decompress(compressed[i], getContract(compressedTypeId, contracts), contracts);
                        }
                    }
                    else {
                        result[propertyName] = compressed[i];
                    }
                });
            }

            return result;
        },
        compress = function (uncompressed, contract, contracts) {
            var result,
                enumerated;

            if (uncompressed) {
                result = [];

                $.each(contract, function (i, val) {
                    var propertyName = val[0],
                        compressedTypeId = val[1][0],
                        enumerable = val[1][1];

                    // Check the payload type of the parameter, if it's a payload we need to recursively compress it
                    if (isPayload(compressedTypeId, contracts)) {
                        if (enumerable) {
                            enumerated = [];

                            for (var j = 0; j < uncompressed[propertyName].length; j++) {
                                enumerated[j] = compress(uncompressed[propertyName][j], getContract(compressedTypeId, contracts), contracts);
                            }

                            result[propertyName].push(enumerated);
                        }
                        else {
                            result.push(compress(uncompressed[propertyName], getContract(compressedTypeId, contracts), contracts));
                        }
                    }
                    else {
                        result.push(uncompressed[propertyName] || null);
                    }
                });
            }

            return result || null;
        };

    $.signalR.fn.init = function () {
        var connection = this,
            compressionData = {
                decompressResult: [], // Array of Booleans representing if we should decompress an invocation result,
                contracts: {} // Contracts to abide by when sending/receiving data
            };

        savedConnection.apply(connection, arguments),

        connection._.compressionData = compressionData;

        connection.starting(function (result) {
            compressionData.contracts = result.Contracts;
        });

        return connection;
    };

    signalR.prototype.received = function (fn) {
        var layer = function (minData) {
            var callbackId,
            connection = this,
            compressionData = connection._.compressionData,
            contracts = compressionData.contracts,
            data,
            contract,
            enumerable,
            result;

            // Verify this is a return value
            if (typeof (minData.I) !== "undefined" && minData.R) {
                data = compressionData.decompressResult.shift();
                callbackId = minData.I;

                // Check if we should decompress this payload
                if (data.decompress) {
                    // Pull the contract for the given method
                    contract = getContractFromResponse(data.hubName, data.methodName, contracts);
                    enumerable = contract[1];
                    contract = contract[0];

                    if (enumerable === false) {
                        result = decompress(minData.R, contract, contracts);
                    }
                    else {
                        result = [];
                        $.each(minData.R, function (i, val) {
                            result.push(decompress(val, contract, contracts));
                        });
                    }

                    minData.R = result;
                }
            }
            else if (typeof (minData.A) !== "undefined") {
                $.each(minData.A, function (i, arg) {
                    var contractId = minData.C[i],
                        contract,
                        enumerable = false;

                    // Checking if the contract that's sent down is an array of contractables
                    if (contractId.length > 2 && contractId.substring(contractId.length - 2) === "[]") {
                        contractId = contractId.substring(0, contractId.length - 2);
                        enumerable = true;
                    }

                    contractId = parseInt(contractId, 10);

                    // Verify there's a valid contract
                    if (isPayload(contractId, contracts)) {
                        contract = getContract(contractId, contracts);

                        if (enumerable === false) {
                            result = decompress(arg, contract, contracts);
                        }
                        else {
                            result = [];
                            $.each(arg, function (i, val) {
                                result.push(decompress(val, contract, contracts));
                            });
                        }

                        minData.A[i] = result;
                    }
                });
            }

            fn.call(this, minData);
        };

        savedReceived.call(this, layer);
    };

    hubConnection.prototype.createHubProxy = function (hubName) {
        var proxy = savedCreateHubProxy.apply(this, arguments),
            savedInvoke = proxy.invoke,
            connection = this,
            compressionData = connection._.compressionData;

        proxy.invoke = function (methodName) {
            var contracts = compressionData.contracts,
                returnContracts = contracts[0][hubName],
                invokeContracts = contracts[1][hubName],
                invokeContractData,
                contractId,
                contract,
                enumerable,
                enumerated,
                methodArgs = $.makeArray(arguments);

            // Check if we need to return a result
            if (returnContracts && returnContracts[methodName]) {
                compressionData.decompressResult.push(buildResult(hubName, methodName, true));
            }
            else {
                compressionData.decompressResult.push(buildResult(hubName, methodName, false));
            }

            if (invokeContracts && invokeContracts[methodName]) {
                invokeContractData = invokeContracts[methodName];

                for (var i = 1; i < methodArgs.length; i++) {
                    contractId = invokeContractData[i - 1][0];
                    enumerable = invokeContractData[i - 1][1];                    

                    if (isPayload(contractId, contracts)) {
                        contract = getContract(contractId, contracts);

                        if (enumerable) {
                            enumerated = [];

                            for(var j = 0;j<methodArgs[i].length;j++) {
                                enumerated.push(compress(methodArgs[i][j], contract, contracts));
                            }

                            methodArgs[i] = enumerated;
                        }
                        else {
                            methodArgs[i] = compress(methodArgs[i], contract, contracts);
                        }
                    }
                }
            }

            return savedInvoke.apply(this, methodArgs);
        };

        return proxy;
    };

}(window.jQuery, window));