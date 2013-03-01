using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class PayloadDescriptor
    {
        public virtual long ID { get; set; }

        public virtual IEnumerable<DataDescriptor> Data { get; set; }

        public virtual Type Type { get; set; }

        public virtual CompressionSettings Settings { get; set; }
    }
}
