using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class DefaultPayloadCompressor : IPayloadCompressor
    {
        private IPayloadDescriptorProvider _provider;

        public DefaultPayloadCompressor(IDependencyResolver resolver)
            : this(resolver.Resolve<IPayloadDescriptorProvider>())
        {
        }

        public DefaultPayloadCompressor(IPayloadDescriptorProvider provider)
        {
            _provider = provider;
        }

        public object Compress(object payload)
        {
            if (payload != null)
            {
                var payloadDescriptor = _provider.GetPayload(payload.GetType());

                // Only compress the payload if we have a payload descriptor for it
                if (payloadDescriptor != null)
                {
                    return payloadDescriptor.Data.Select(dataDescriptor =>
                            {
                                // Recursively compress the object value until it's at a base type
                                return Compress(dataDescriptor.GetValue(payload));
                            });
                }
            }

            return payload;
        }
    }
}
