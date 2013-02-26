using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    /// <summary>
    /// Apply to classes or interfaces that represent data to be sent down to client
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PayloadAttribute : Attribute
    {
    }
}
