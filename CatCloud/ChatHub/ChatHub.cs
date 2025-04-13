using Application.DTOs.Chat;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CatCloud.ChatHub
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string userName, string message);
    }
    public class ChatHub(IAuthService authService, IChatService chatService) : Hub<IChatClient>
    {
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();
        private readonly IAuthService _authService = authService;
        private readonly IChatService _chatService = chatService;
        public async Task JoinChat(UserConnection connection)
        {
            var user = await _authService.GetUserById(connection.UserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom.ToString());

            _connections[Context.ConnectionId] = connection;

            await Clients.Group(connection.ChatRoom.ToString())
                .ReceiveMessage("", $"{user.Username} s-a conectat!");

            var pastMessages = await _chatService.GetMessagesDTOs(connection.ChatRoom);
            foreach (var msg in pastMessages)
            {
                await Clients.Caller.ReceiveMessage(msg.Username, msg.Message);
            }

        }

        public async Task LeaveChat()
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom.ToString());

                //await Clients.Group(connection.ChatRoom)
                //    .ReceiveMessage("Admin", $"{connection.UserName} a părăsit {connection.ChatRoom}");
            }
        }


        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var connection))
            {
                var user = await _authService.GetUserById(connection.UserId);
                await Clients.Group(connection.ChatRoom.ToString())
                    .ReceiveMessage(user.Username, message);

                await _chatService.SaveMessage(new SaveMessageDTO
                {
                    Message = message,
                    GroupId = connection.ChatRoom,
                    UserId = connection.UserId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                var user = await _authService.GetUserById(connection.UserId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom.ToString());
                await Clients.Group(connection.ChatRoom.ToString())
                    .ReceiveMessage("", $"{user.Username} a părăsit chat-ul");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<MessagesDTO>> GetMessages(Guid groupId)
        {
            return await _chatService.GetMessagesDTOs(groupId);
        }

    }
}
