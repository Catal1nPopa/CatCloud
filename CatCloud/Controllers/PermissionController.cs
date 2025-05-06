using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController(IPermissionsService permissionsService) : ControllerBase
    {
        private readonly IPermissionsService _permissionsService = permissionsService;

        //[Authorize]
        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromQuery] string roleName)
        {
            await _permissionsService.AddRole(roleName);
            return Ok(new { Message = $"rol adaugat {roleName}" });
        }

        [Authorize]
        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            await _permissionsService.DeleteRole(roleName);
            return Ok();
        }

        [Authorize]
        [HttpGet("roles")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return Ok(await _permissionsService.GetRoles());
        }

        [Authorize]
        [HttpPost("permissions")]
        public async Task<IActionResult> AddPermission([FromQuery] string permission)
        {
            await _permissionsService.AddPermission(permission);
            return Ok(new { Message = $"Permisiune {permission} adaugata" });
        }

        [Authorize]
        [HttpDelete("permissions/{permission}")]
        public async Task<IActionResult> DeletePermission(string permission)
        {
            await _permissionsService.DeletePermission(permission);
            return Ok(new { Message = $"Permisiune {permission} stearsa" });
        }

        [Authorize]
        [HttpGet("permissions")]
        public async Task<ActionResult<List<string>>> GetPermissions()
        {
            try
            {
                return Ok(await _permissionsService.GetPermissions());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("roles/{roleName}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(string roleName, [FromBody] List<string> permissions)
        {
            try
            {
                await _permissionsService.AssignPermissionsToRole(roleName, permissions);
                return Ok(new { Message = $"Permisiunile au fost asignate rolului '{roleName}'." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("roles/{roleName}/permissions")]
        public async Task<ActionResult<List<string>>> GetRolePermissions(string roleName)
        {
            try
            {

                var permissions = await _permissionsService.GetRolePermissions(roleName);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Authorize]
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignRoleToUser(Guid userId, string roleName)
        {
            try
            {
                await _permissionsService.AssignRoleToUser(userId, roleName);
                return Ok(new { Message = $"Rolul '{roleName}' a fost asignat utilizatorului." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<List<string>>> GetUserRoles(Guid userId)
        {
            try
            {
                var roles = await _permissionsService.GetUserRoles(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, string roleName)
        {
            try
            {
                await _permissionsService.RemoveRoleFromUser(userId, roleName);
                return Ok(new { Message = $"Rolul '{roleName}' a fost eliminat." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("groups/{groupId}/users/{userId}/roles")]
        public async Task<IActionResult> AssignRoleToUserInGroup(Guid groupId, Guid userId, [FromBody] string roleName)
        {
            try
            {
                await _permissionsService.AssignRoleToUserInGroup(userId, groupId, roleName);
                return Ok(new { Message = $"Rolul '{roleName}' a fost asignat utilizatorului în grupul '{groupId}'." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("groups/{groupId}/users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUserInGroup(Guid groupId, Guid userId, string roleName)
        {
            try
            {
                await _permissionsService.RemoveRoleFromUserInGroup(userId, groupId, roleName);
                return Ok(new { Message = $"Rolul '{roleName}' a fost eliminat din grupul '{groupId}' pentru utilizator." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("groups/{groupId}/users/{userId}/roles")]
        public async Task<ActionResult<List<string>>> GetUserRolesInGroup(Guid groupId, Guid userId)
        {
            try
            {
                var roles = await _permissionsService.GetUserRolesInGroup(userId, groupId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
