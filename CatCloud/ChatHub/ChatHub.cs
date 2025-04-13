using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace CatCloud.ChatHub
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string userName, string message);
    }
    public class ChatHub : Hub<IChatClient>
    {
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();

        public async Task JoinChat(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            _connections[Context.ConnectionId] = connection;

            await Clients.Group(connection.ChatRoom)
                .ReceiveMessage("Admin", $"{connection.UserName} s-a conectat la {connection.ChatRoom}");
        }

        public async Task LeaveChat()
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);

                //await Clients.Group(connection.ChatRoom)
                //    .ReceiveMessage("Admin", $"{connection.UserName} a părăsit {connection.ChatRoom}");
            }
        }


        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var connection))
            {
                await Clients.Group(connection.ChatRoom)
                    .ReceiveMessage(connection.UserName, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);
                await Clients.Group(connection.ChatRoom)
                    .ReceiveMessage("Admin", $"{connection.UserName} a părăsit chat-ul");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
