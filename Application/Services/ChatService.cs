using Application.DTOs.Chat;
using Application.Interfaces;
using Domain.Entities.Chat;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class ChatService(IChatRepository chatRepository) : IChatService
    {
        private readonly IChatRepository _chatService = chatRepository;
        public async Task<List<MessagesDTO>> GetMessagesDTOs(Guid chatRoomId)
        {
            var messages = await _chatService.GetHistoryMessage(chatRoomId);
            //return messages.Adapt<List<MessagesDTO>>();
            return messages.Select(m => new MessagesDTO
            {
                Username = m.User.Username,
                Message = m.Message,
                GroupId = m.GroupId ?? Guid.Empty,
                Timestamp = m.Timestamp
            }).ToList();

        }

        public async Task SaveMessage(SaveMessageDTO messageDTO)
        {
            await _chatService.SaveMessage(messageDTO.Adapt<ChatEntity>());
        }

        public async Task<List<Guid>> GetUserIdsInGroup(Guid chatRoomId)
        {
            var userIds = await _chatService.GetUserIdsInGroup(chatRoomId);
            return userIds;
        }
    }
}
