using Application.DTOs.Auth;
using Application.DTOs.UserGroup;
using Application.Interfaces;
using Domain.Entities.UserGroup;
using Domain.Interfaces;
using Helper.Serilog;
using Mapster;

namespace Application.Services
{
    public class UserGroupService(IGroupRepository groupRepository) : IUserGroupService
    {
        private readonly IGroupRepository _groupRepository = groupRepository;
        public async Task CreateGroup(GroupDTO groupEntity)
        {
            try
            {
                groupEntity.Created = DateTime.UtcNow;
                await _groupRepository.CreateGroup(groupEntity.Adapt<GroupEntity>());
                LoggerHelper.LogWarning($"Grup nou creat {groupEntity.Name}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task DeleteGroup(Guid groupId)
        {
            try
            {
                await _groupRepository.DeleteGroup(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la stergere grup - {ex}");
            }
        }

        public async Task EditGroup(GroupDTO newGroupData)
        {
            try
            {
                await _groupRepository.EditGroup(newGroupData.Adapt<GroupEntity>());
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la editare grup {ex}");
            }
        }

        public async Task<List<UserInfoDTO>> GetGroupUsers(Guid groupId)
        {
            try
            {
                var users = await _groupRepository.GetGroupUsers(groupId);
                return users.Adapt<List<UserInfoDTO>>();
            }
            catch(Exception ex)
            {
                throw new Exception($"Eroare la obtinere utilizatori pentru grupul {groupId}");
            }
        }

        public async Task<List<GroupDTO>> GetUserGroups(Guid userId)
        {
            try
            {
                var groups = await _groupRepository.GetUserGroups(userId);
                return groups.Adapt<List<GroupDTO>>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinere grupuri pentru user {userId}");
            }
        }

        public async Task LinkUserToGroup(UserToGroupDTO data)
        {
            try
            {
                await _groupRepository.LinkUserToGroup(data.Adapt<LinkUserToGroupEntity>());
            }
            catch (Exception ex)
            {
                throw new Exception($"Exceptie la adaugare utilizator in grup {ex}");
            }
        }

        public async Task UnlinkUserFromGroup(UserToGroupDTO data)
        {
            try
            {
                await _groupRepository.UnlinkUserFromGroup(data.Adapt<LinkUserToGroupEntity>());
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la eliminarea utilizatorului din grup: {ex.Message}");
            }
        }
    }
}
