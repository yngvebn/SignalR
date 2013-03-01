using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class CompressionSettings
    {
        public static CompressionSettings Default = new CompressionSettings
        {
            DigitsToMaintain = -1
        };

        public int DigitsToMaintain { get; set; }
    }
}
