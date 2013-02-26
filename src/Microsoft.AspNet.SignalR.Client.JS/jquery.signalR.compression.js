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
            var contractId = contracts[0][hubName][methodName];

            return getContract(contractId, contracts);
        },
        buildResult = function (hubName, methodName, decompress) {
            return {
                decompress: decompress,
                hubName: hubName,
                methodName: methodName
            };
        },
        getContract = function (contractId, contracts) {
            return contracts[1][contractId];
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
                        compressedTypeId = val[1];


                    // Check the payload type of the parameter, we need to recursively 
                    if (isPayload(compressedTypeId, contracts)) {
                        result[propertyName] = decompress(compressed[i], getContract(compressedTypeId, contracts), contracts);
                    }
                    else {
                        result[propertyName] = compressed[i];
                    }
                });
            }

            return result;
        };

    $.signalR.fn.init = function () {
        var connection = this,
            compressionData = {
                decompressResult: [], // Array of booleans representing if we should decompress an invocation result,
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
            contract;

            // Verify this is a return value
            if (typeof (minData.I) !== "undefined" && minData.R) {
                data = compressionData.decompressResult.shift();
                callbackId = minData.I;

                // Check if we should decompress this payload
                if (data.decompress) {
                    // Pull the contract for the given method
                    contract = getContractFromResponse(data.hubName, data.methodName, contracts);

                    minData.R = decompress(minData.R, contract, contracts);
                }
            }
            else if(typeof(minData.A) !== "undefined") {
                $.each(minData.A, function (i, arg) {
                    var contractId = minData.C[i],
                        contract;

                    // Verify there's a valid contract
                    if (isPayload(contractId, contracts)) {
                        contract = getContract(contractId, contracts);

                        minData.A[i] = decompress(arg, contract, contracts);
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
            var contracts = compressionData.contracts[0][hubName];

            if (contracts && contracts[methodName]) {
                compressionData.decompressResult.push(buildResult(hubName, methodName, true));
            }
            else {
                compressionData.decompressResult.push(buildResult(hubName, methodName, false));
            }

            return savedInvoke.apply(this, arguments);
        };

        return proxy;
    };

}(window.jQuery, window));