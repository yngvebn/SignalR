using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Client.Samples
{
    class ChatClientPC
    {
        public void Start()
        {
            var connection = new Connection("http://localhost:40476/chat");

            connection.Start().Wait();

            string line = null;
            while ((line = Console.ReadLine()) != null)
            {
                // Send a message to the server
                connection.Send(line).Wait();
            }
        }
    }
}
