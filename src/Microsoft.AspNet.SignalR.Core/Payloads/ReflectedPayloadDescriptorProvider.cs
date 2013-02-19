using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public class ReflectedPayloadDescriptorProvider : IPayloadDescriptorProvider
    {
        private readonly Lazy<IDictionary<long, PayloadDescriptor>> _payloads;
        private readonly Lazy<IAssemblyLocator> _locator;

        private static long _payloadDescriptorID = 0;

        public ReflectedPayloadDescriptorProvider(IDependencyResolver resolver)
        {
            _locator = new Lazy<IAssemblyLocator>(resolver.Resolve<IAssemblyLocator>);
            _payloads = new Lazy<IDictionary<long, PayloadDescriptor>>(BuildPayloadsCache);
        }

        protected IDictionary<long, PayloadDescriptor> BuildPayloadsCache()
        {
            // Getting all payloads that have a payload attribute
            var types = _locator.Value.GetAssemblies()
                        .SelectMany(GetTypesSafe)
                        .Where(IsPayload);

            // Building cache entries for each descriptor
            // Each descriptor is stored in dictionary under a key
            // that is it's name
            var cacheEntries = types
                .Select(type => new PayloadDescriptor
                {
                    ID = Interlocked.Increment(ref _payloadDescriptorID),
                    Data = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Select(propertyInfo => new DataDescriptor
                                {
                                    Name = propertyInfo.Name,
                                })
                               .Union(type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                               .Select(memberInfo => new DataDescriptor
                               {
                                   Name = memberInfo.Name
                               }))
                               .OrderBy(dataDescriptor => dataDescriptor.Name)
                })
                .ToDictionary(payload => payload.ID,
                              payload => payload);

            return cacheEntries;
        }

        public IEnumerable<PayloadDescriptor> GetPayloads()
        {
            return _payloads.Value
                .Select(a => a.Value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "If we throw then we have an empty type")]
        private static IEnumerable<Type> GetTypesSafe(Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        private static bool IsPayload(Type type)
        {
            try
            {
                return Attribute.IsDefined(type, typeof(PayloadAttribute));
            }
            catch
            {
                return false;
            }
        }
    }
}
