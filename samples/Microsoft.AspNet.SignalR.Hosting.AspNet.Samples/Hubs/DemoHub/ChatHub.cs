using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Client.Samples
{
    public class ChatHub : Hub
    {
        // User Names mapped to User objects
        private static readonly ConcurrentDictionary<string, ChatUser> _users = new ConcurrentDictionary<string, ChatUser>(StringComparer.OrdinalIgnoreCase);

        // User Names mapped to list of rooms
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userRooms = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        // Room names mapped to romm objects
        private static readonly ConcurrentDictionary<string, ChatRoom> _rooms = new ConcurrentDictionary<string, ChatRoom>(StringComparer.OrdinalIgnoreCase);

        public void Send(string message)
        {
            // Call the broadcastMessage method to update clients.
            foreach (string roomName in _userRooms[Clients.Caller.userName])
            {
                Clients.OthersInGroup(Clients.Caller.roomName).broadcastMessage(String.Format(" {0} : {1}", Clients.Caller.userName, message));
            }
        }

        public Task AddUser(string userName)
        {
            var user = new ChatUser(userName, Context.ConnectionId);
            _users.TryAdd(userName, user);
            Clients.Caller.userName = user.Name;
            Clients.Caller.userId = user.Id;
            // return Clients.Group("foo").broadcastMessage(userName);
            return Clients.Client(null).broadcastMessage(userName);
            //return Clients.All.broadcastMessage(String.Format("New user {0} has joined", user.Name));
        }


        public Task Join(string roomName)
        {
            // adding the room to the list of rooms of the user
            Console.WriteLine("Here1");
            var userRooms = new HashSet<string>();
            Console.WriteLine("Here1");
            _userRooms.TryGetValue(Clients.Caller.userName, out userRooms);
            Console.WriteLine("Here2");
            userRooms.Add(roomName);
            Console.WriteLine("Here3");

            /*
            _rooms.TryAdd(roomName, new ChatRoom());

            var chatRoom = new ChatRoom();
            // adding the new room to the list of rooms
            _rooms.TryGetValue(roomName, out chatRoom);
            // adding the user to the room
            chatRoom.Users.Add(Context.ConnectionId); */

            return Groups.Add(Context.ConnectionId, roomName);
        }

        public override Task OnDisconnected()
        {
            // string groupName = Clients.Caller.groupName;
            return Clients.Others.broadcastMessage(String.Format("Client {0} has left the chat room", "left"));
        }

        public override Task OnConnected()
        {
            return Clients.Client(Context.ConnectionId).broadcastMessage("Connected");
        }

        [Serializable]
        public class ChatUser
        {
            public string ConnectionId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }

            public ChatUser()
            {
            }

            public ChatUser(string name, string connectionId)
            {
                Name = name;
                ConnectionId = connectionId;
                Id = Guid.NewGuid().ToString("d");
            }
        }

        public class ChatRoom
        {
            // should User be readonly
            public HashSet<string> Users { get; private set; }
            public List<string> ChatMessages { get; set; }

            public ChatRoom()
            {
                Users = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                ChatMessages = new List<string>();
            }
        }
    }
}