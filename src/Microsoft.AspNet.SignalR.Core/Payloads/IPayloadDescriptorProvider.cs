using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public interface IPayloadDescriptorProvider
    {
        /// <summary>
        /// Retrieve all avaiable payload types.
        /// </summary>
        /// <returns>Collection of payload descriptors.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This call might be expensive")]
        IEnumerable<PayloadDescriptor> GetPayloads();
    }
}
