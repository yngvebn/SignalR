using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Compression;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.Samples
{
    [Payload]
    public class BigData
    {
        public int BAR { get; set; }
    }
}