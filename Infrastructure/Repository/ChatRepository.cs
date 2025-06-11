using Domain.Entities.Chat;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ChatRepository(CloudDbContext dbContext) : IChatRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;

        public async Task<List<ChatEntity>> GetHistoryMessage(Guid chatRoomId)
        {
            try
            {
                var messages = await _dbContext.ChatMessages
                .Where(m => m.GroupId == chatRoomId)
                .Include(user => user.User)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

                return messages;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task SaveMessage(ChatEntity chatEntity)
        {
            try
            {
                _dbContext.Add(chatEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch ( Exception ex )
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<List<Guid>> GetUserIdsInGroup(Guid chatRoomId)
        {
            var groupIDs = await _dbContext.UserGroups
                .Where(g => g.GroupId == chatRoomId)
                .Select(g => g.UserId)
                .ToListAsync();

            return groupIDs;
        }
    }
}
