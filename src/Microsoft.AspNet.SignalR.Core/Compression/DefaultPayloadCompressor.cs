using System;
using System.Collections;
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
                var payloadType = payload.GetType();
                var payloadDescriptor = _provider.GetPayload(payloadType);

                // Only compress the payload if we have a payload descriptor for it
                if (payloadDescriptor != null)
                {
                    return payloadDescriptor.Data.Select(dataDescriptor =>
                            {
                                // Recursively compress the object value until it's at a base type
                                return Compress(dataDescriptor.GetValue(payload));
                            });
                }
                else
                {
                    // At this point the payload object isn't directly a payload but may contain a payload
                    if (_provider.HasPayload(payloadType))
                    {
                        payloadType = payloadType.GetEnumerableType();
                        var itemType = payload.GetType();
                        payloadDescriptor = _provider.GetPayload(payloadType);
                        var payloadList = payload as ICollection;
                        var compressedList = new List<object>();

                        if (payloadDescriptor != null)
                        {
                            foreach (var item in payloadList)
                            {
                                compressedList.Add(Compress(item));
                            }

                            return compressedList;
                        }
                    }
                }
            }

            return payload;
        }
    }
}
