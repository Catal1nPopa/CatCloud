

using Application.DTOs.Chat;

namespace Application.Interfaces
{
    public interface IChatService
    {
        Task<List<MessagesDTO>> GetMessagesDTOs(Guid chatRoomId);
        Task SaveMessage(SaveMessageDTO messageDTO);
    }
}
