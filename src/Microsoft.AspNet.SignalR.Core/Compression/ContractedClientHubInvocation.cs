using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class ContractedClientHubInvocation : ClientHubInvocation
    {
        public ContractedClientHubInvocation(ClientHubInvocation invocation)
        {
            Target = invocation.Target;
            Hub = invocation.Hub;
            Method = invocation.Method;
            Args = invocation.Args;
            State = invocation.State;
        }

        /// <summary>
        /// A klist of contract Ids, it's a 1-to-1 map to args
        /// </summary>
        [JsonProperty("C")]
        public long[] ContractIds { get; set; }
    }
}
