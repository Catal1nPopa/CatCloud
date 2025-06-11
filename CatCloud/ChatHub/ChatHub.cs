using System;
using Application.DTOs.Chat;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Application.DTOs.Notification;
using CatCloud.Models.Notification;
using Mapster;

namespace CatCloud.ChatHub
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string userName, string message);
        Task UsersOnline(List<string> usernames);
    }
    public class ChatHub(IAuthService _authService, IChatService _chatService, IHubContext<NotificationHub.NotificationHub> notification,
        INotificationService notificationService, IUserGroupService userGroupService) : Hub<IChatClient>
    {
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();
        public async Task JoinChat(UserConnection connection)
        {
            try
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

                var usersOnline = _connections
                    .Where(x => x.Value.ChatRoom == connection.ChatRoom)
                    .Select(async x => (await _authService.GetUserById(x.Value.UserId)).Username)
                    .Select(t => t.Result)
                    .Distinct()
                    .ToList();

                await Clients.Group(connection.ChatRoom.ToString())
                    .UsersOnline(usersOnline);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
 
        }

        public async Task LeaveChat()
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom.ToString());
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
                
                var groupMembers = await _chatService.GetUserIdsInGroup(connection.ChatRoom);
                
                var connectedUserIdsInGroup = _connections
                    .Where(x => x.Value.ChatRoom == connection.ChatRoom)
                    .Select(x => x.Value.UserId)
                    .Distinct()
                    .ToList();
                var offlineUsers = groupMembers.Except(connectedUserIdsInGroup);
                
                var group = await userGroupService.GetGroup(connection.ChatRoom);
                
                foreach (var userId in offlineUsers)
                {
                    var notificationModel = new NotificationModel
                    {
                        Message = $"Ai un mesaj nou în grupul: {group.Name}",
                        Timestamp = DateTime.UtcNow,
                        UserId = userId
                    };
                    await notification.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", new { message = notificationModel.Message, timestamp = notificationModel.Timestamp });
                    await notificationService.SaveNotification(notificationModel.Adapt<NotificationDTO>());

                    await notificationService.SaveNotification(notificationModel.Adapt<NotificationDTO>());

                    await notification.Clients.Group(userId.ToString())
                        .SendAsync("ReceiveNotification", notificationModel);
                }
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

                var usersOnline = _connections
                    .Where(x => x.Value.ChatRoom == connection.ChatRoom)
                    .Select(async x => (await _authService.GetUserById(x.Value.UserId)).Username)
                    .Select(t => t.Result)
                    .Distinct()
                    .ToList();

                await Clients.Group(connection.ChatRoom.ToString())
                    .UsersOnline(usersOnline);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<MessagesDTO>> GetMessages(Guid groupId)
        {
            return await _chatService.GetMessagesDTOs(groupId);
        }

    }
}
