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
            string userName, roomName;

            var connection = new HubConnection("http://localhost:40476/");

            IHubProxy chatHub = connection.CreateHubProxy("ChatHub");

            chatHub.On("broadcastMessage", message => Console.WriteLine(message));

            connection.Start().Wait();

            string line = null;

            Console.Write("Enter your Name : ");
            userName = Console.ReadLine();
            chatHub.Invoke("AddUser", userName).Wait();

            Thread.Sleep(2 * 1000);

            //Console.Write("Enter the name of the room you want to join : ");
            //roomName = Console.ReadLine();
            //chatHub.Invoke("Join", roomName);

            // connection.Items.Add("clientName", clientName);
            //var nameCookie = new Cookie("clientName", clientName);
            //nameCookie.Domain = new Uri("http://localhost:27427/").OriginalString;
            //connection.CookieContainer = new CookieContainer();
            //connection.CookieContainer.Add(nameCookie);

            /*Console.Write("Enter the Name of the Group: ");
            roomName = Console.ReadLine();
            connection.Items.Add("groupName", roomName);
            chatHub.Invoke("Join", roomName).Wait();*/

            //connection.CookieContainer

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
            Console.WriteLine("Hello");
        }
    }
}
