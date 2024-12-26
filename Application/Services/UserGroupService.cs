using Application.DTOs.UserGroup;
using Application.Interfaces;
using Domain.Entities.UserGroup;
using Domain.Interfaces;
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
                await _groupRepository.CreateGroup(groupEntity.Adapt<GroupEntity>());
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
