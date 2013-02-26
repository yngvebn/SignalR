using System;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class DefaultContractsGenerator : IContractsGenerator
    {
        private Lazy<JRaw> _generatedContracts;
        private IJsonSerializer _serializer;
        private IPayloadDescriptorProvider _payloadProvider;
        private IMethodDescriptorProvider _methodProvider;
        private IHubDescriptorProvider _hubProvider;

        public DefaultContractsGenerator(IDependencyResolver resolver)
            : this(resolver.Resolve<IJsonSerializer>(),
                   resolver.Resolve<IPayloadDescriptorProvider>(),
                   resolver.Resolve<IMethodDescriptorProvider>(),
                   resolver.Resolve<IHubDescriptorProvider>())
        {
        }

        public DefaultContractsGenerator(IJsonSerializer serializer, IPayloadDescriptorProvider payloadProvider, IMethodDescriptorProvider methodProvider, IHubDescriptorProvider hubProvider)
        {
            _serializer = serializer;
            _payloadProvider = payloadProvider;
            _methodProvider = methodProvider;
            _hubProvider = hubProvider;
            _generatedContracts = new Lazy<JRaw>(() => new JRaw(_serializer.Stringify(CreateContractsCache(_serializer, _payloadProvider, _methodProvider, _hubProvider))));
        }

        private static object CreateContractsCache(IJsonSerializer serializer, IPayloadDescriptorProvider payloadProvider, IMethodDescriptorProvider methodProvider, IHubDescriptorProvider hubProvider)
        {
            var contactableMethods = hubProvider.GetHubs()
                                                .Select(hub => methodProvider.GetMethods(hub)
                                                .Where(methodDescriptor => payloadProvider.IsPayload(methodDescriptor.ReturnType)))
                                                .Where(methodList => methodList.Count() > 0)
                                                .ToDictionary(methodList => methodList.First().Hub.Name,
                                                              methodList => methodList
                                                              .Select(methodDescriptor =>
                                                                 new object[]{
                                                                                    methodDescriptor.Name,
                                                                                    payloadProvider.GetPayload(methodDescriptor.ReturnType).ID
                                                                             }).ToDictionary(methodNameToID => methodNameToID[0],
                                                                                             methodNameToID => methodNameToID[1]));

            var contracts = payloadProvider.GetPayloads()
                                           .Select(payloadDescriptor => payloadDescriptor)
                                           .ToDictionary(payloadDescriptor => payloadDescriptor.ID,
                                                         payloadDescriptor => payloadDescriptor.Data
                                                                              .Select(dataDescriptor => new object[] 
                                                                              { 
                                                                                  dataDescriptor.Name, 
                                                                                  payloadProvider.IsPayload(dataDescriptor.Type) ? payloadProvider.GetPayload(dataDescriptor.Type).ID : -1
                                                                              }));

            // Format is:
            // [0] = Methods that return a contractable type (Used for Hub Responses)
            // [1] = Payload contracts
            return new object[] { contactableMethods, contracts };
        }

        public object GenerateContracts()
        {
            return _generatedContracts.Value;
        }
    }
}
