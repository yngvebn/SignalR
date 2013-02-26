using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public interface IPayloadCompressor
    {
        object Compress(object payload);
    }
}
