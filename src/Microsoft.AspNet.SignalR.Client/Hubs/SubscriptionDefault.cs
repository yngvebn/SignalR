using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.Client.Hubs
{
    public class SubscriptionDefault
    {
        public event Action<IList<JToken>, string> Received;

        internal void OnReceived(IList<JToken> data, string methodName)
        {
            if (Received != null)
            {
                Received(data, methodName);
            }
        }
    }
}
