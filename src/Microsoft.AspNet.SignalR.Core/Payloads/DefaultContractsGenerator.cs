using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public class DefaultContractsGenerator : IContractsGenerator
    {
        private Lazy<object> _generatedContracts;
        private IJsonSerializer _serializer;
        private IPayloadDescriptorProvider _payloadProvider;

        public DefaultContractsGenerator(IDependencyResolver resolver)
            : this(resolver.Resolve<IJsonSerializer>(),
                   resolver.Resolve<IPayloadDescriptorProvider>())
        {
        }

        public DefaultContractsGenerator(IJsonSerializer serializer, IPayloadDescriptorProvider payloadProvider)
        {
            _serializer = serializer;
            _payloadProvider = payloadProvider;

            _generatedContracts = new Lazy<object>(() => CreateContractsCache(_serializer, _payloadProvider));
        }

        private static object CreateContractsCache(IJsonSerializer serializer, IPayloadDescriptorProvider payloadProvider)
        {
            return payloadProvider.GetPayloads();
        }

        public object GenerateContracts()
        {
            return _generatedContracts.Value;
        }
    }
}
