using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Samples
{
    public class ChatServer : PersistentConnection
    {
        private static int _numConnections = 0;

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            Interlocked.Increment(ref _numConnections);
            return this.Connection.Broadcast("New connection: " + connectionId);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            Debug.WriteLine(data);
            //return Groups.Send("foo", data);
            return this.Connection.Send(null, "Hello");
        }

        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            Interlocked.Decrement(ref _numConnections);
            return this.Connection.Broadcast("Connection id: " + connectionId + "has left");
        }
    }
}