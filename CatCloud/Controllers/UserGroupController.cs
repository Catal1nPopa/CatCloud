using Application.DTOs.UserGroup;
using Application.Interfaces;
using CatCloud.Models.Group;
using CatCloud.Models.User;
using Domain.Entities.UserGroup;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController(IUserGroupService userGroupService) : ControllerBase
    {
        private readonly IUserGroupService _userGroupService = userGroupService;

        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup(GroupModel group)
        {
            await _userGroupService.CreateGroup(group.Adapt<GroupDTO>());
            return Ok();
        }

        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            await _userGroupService.DeleteGroup(groupId);
            return Ok();
        }

        [HttpPut("edit/{groupId}")]
        public async Task<IActionResult> EditGroup([FromBody] GroupModel updatedGroup)
        {
            try
            {
                await _userGroupService.EditGroup(updatedGroup.Adapt<GroupDTO>());
                return Ok(new { message = "Grup editat cu succes." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("LinkUserToGroup")]
        public async Task<IActionResult> LinkUserToGroup(UserToGroupModel userToGroup)
        {
            await _userGroupService.LinkUserToGroup(userToGroup.Adapt<UserToGroupDTO>());
            return Ok();
        }

        [HttpPost("UnlinkUserToGroup")]
        public async Task<IActionResult> UnlinkUserToGroup(UserToGroupModel userToGroup)
        {
            await _userGroupService.UnlinkUserFromGroup(userToGroup.Adapt<UserToGroupDTO>());
            return Ok();
        }

        [HttpGet("GetUserGroups")]
        public async Task<ActionResult<GroupModel>> GetUserGroups(Guid userId)
        {
            var groups = await _userGroupService.GetUserGroups(userId);
            return Ok(groups.Adapt<List<GroupModel>>());
        }

        [HttpGet("GetGroupUsers")]
        public async Task<ActionResult<UserInfoModel>> GetGroupsUsers(Guid groupId)
        {
            var users = await _userGroupService.GetGroupUsers(groupId);
            return Ok(users.Adapt<List<UserInfoModel>>());
        }
    }
}
