using Domain.Entities.Chat;


namespace Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<List<ChatEntity>> GetHistoryMessage(Guid chatRoomId);
        Task SaveMessage(ChatEntity chatEntity);
    }
}
