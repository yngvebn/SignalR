using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Microsoft.AspNet.SignalR.Client.Samples
{
    public class ChatClient
    {
        public void Start()
        {
            string userName;

            var connection = new HubConnection("http://localhost:40476/");

            IHubProxy chatHub = connection.CreateHubProxy("ChatHub");

            chatHub.On("broadcastMessage", message => Console.WriteLine(message));

            chatHub.OnX<string>("!", methodName => Console.WriteLine(String.Format("Method {0} not found on the client", methodName)));

            chatHub.OnX<string>("*", methodName => Console.WriteLine(String.Format("Method {0} was just executed on the client", methodName)));

            connection.Start().Wait();

            string line = null;

            Console.Write("Enter your Name : ");
            userName = Console.ReadLine();
            chatHub.Invoke("AddUser1", userName).Wait();

            Thread.Sleep(2 * 1000);

            while ((line = Console.ReadLine()) != null)
            {
                if (line.CompareTo("exit") == 0)
                {
                    connection.Stop();
                }
                else
                {
                    chatHub.Invoke("Send", line).Wait();
                }
            }

            Console.ReadKey();
        }
    }
}