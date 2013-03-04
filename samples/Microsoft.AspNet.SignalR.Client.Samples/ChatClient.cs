using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json.Linq;

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

            chatHub.OnMissing((args, methodName) =>
            {
                string argList = "";
                foreach (JToken argument in args)
                {
                    argList += argument.ToObject<string>() + ", ";
                }
                Debug.WriteLine(String.Format("Client method '{0}' not defined on the client", methodName));
            });

            //chatHub.OnMissing<<IList<JToken>,string>(methodName => Debug.WriteLine(String.Format("Client method {0} not found", methodName)));

            chatHub.OnAny((args, methodName) =>
            {
                string argList = "";
                foreach (JToken argument in args)
                {
                    argList += argument.ToObject<string>() + ", ";
                }
                Debug.WriteLine(String.Format("Client method '{0}' executed with the arguments : {1}", methodName, argList));
            });

            connection.Start().Wait();
            Thread.Sleep(2 * 1000);
            string line = null;

            // Console.ReadLine();
            Console.Write("Enter your Name : ");
            userName = Console.ReadLine();
            chatHub.Invoke("AddUser", userName).Wait();

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