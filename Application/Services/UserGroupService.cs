using Application.DTOs.Auth;
using Application.DTOs.UserGroup;
using Application.Interfaces;
using Domain.Entities.UserGroup;
using Domain.Interfaces;
using Helper.Serilog;
using Mapster;

namespace Application.Services
{
    public class UserGroupService(IGroupRepository groupRepository,
        IUserProvider userProvider) : IUserGroupService
    {
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IUserProvider _userProvider = userProvider;
        public async Task CreateGroup(GroupDTO groupEntity)
        {
            groupEntity.OwnerId = _userProvider.GetUserId();
            groupEntity.Created = DateTime.UtcNow;
            await _groupRepository.CreateGroup(groupEntity.Adapt<GroupEntity>());
            LoggerHelper.LogWarning($"Grup nou creat {groupEntity.Name}");
        }

        public async Task DeleteGroup(Guid groupId)
        {
            await _groupRepository.DeleteGroup(groupId);
        }

        public async Task EditGroup(GroupDTO newGroupData)
        {
            await _groupRepository.EditGroup(newGroupData.Adapt<GroupEntity>());
        }

        public async Task<List<GroupDTO>> GetGroupsNotSharedWithFile(Guid fileId)
        {
            Guid userId = _userProvider.GetUserId();
            List<GroupEntity> groups = await _groupRepository.GetGroupsNotSharedWithFile(fileId,userId);
            return groups.Adapt<List<GroupDTO>>();
        }

        public async Task<List<UserInfoDTO>> GetGroupUsers(Guid groupId)
        {
            var users = await _groupRepository.GetGroupUsers(groupId);
            return users.Adapt<List<UserInfoDTO>>();
        }

        public async Task<List<GroupDTO>> GetUserGroups()
        {
            var groups = await _groupRepository.GetUserGroups(_userProvider.GetUserId());
            return groups.Adapt<List<GroupDTO>>();
        }

        public async Task LinkUserToGroup(UserToGroupDTO data)
        {
            await _groupRepository.LinkUserToGroup(data.Adapt<LinkUserToGroupEntity>());
        }

        public async Task UnlinkUserFromGroup(UserToGroupDTO data)
        {
            await _groupRepository.UnlinkUserFromGroup(data.Adapt<LinkUserToGroupEntity>());
        }

        public async Task<List<UserInfoDTO>> GetUsersToLink(Guid groupId)
        {
            var users = await _groupRepository.GetUsersToLink(groupId);
            return users.Adapt<List<UserInfoDTO>> ();
        }

        public async Task<GroupDTO> GetGroup(Guid groupId)
        {
            var group = await _groupRepository.GetGroup(groupId);
            return group.Adapt<GroupDTO>();
        }
    }
}
