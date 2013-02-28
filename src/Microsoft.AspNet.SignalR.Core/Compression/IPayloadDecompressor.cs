using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR.Json;

namespace Microsoft.AspNet.SignalR.Compression
{
    public interface IPayloadDecompressor
    {
        object Decompress(object payload, Type expected);
    }
}
