using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Auth;
using Application.DTOs.Mail;
using Application.Interfaces;
using Application.Services;
using CatCloud.Models.Mail;
using CatCloud.Models.User;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController(ISendMailService sendMailService) : ControllerBase
    {
        private readonly ISendMailService _sendMailService = sendMailService;

        [Authorize]
        [HttpPost("RequestSpace")]
        public async Task<IActionResult> RequestSpace()
        {
            try
            {
                bool status = await _sendMailService.SendReuqestMoreSpace();
                return Ok(new { message = status});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("RequestHelp")]
        public async Task<IActionResult> RequestHelp([FromBody] RequestHelpModel  requestHelp)
        {
            try
            {
                bool status = await _sendMailService.RequestHelp(requestHelp.Adapt<RequestHelpDTO>());
                return Ok(new { message = status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("RequestHelp")]
        public async Task<ActionResult<List<ResponseHelpModel>>> GetAllHelpRequests(Guid fileId)
        {
            try
            {
                var requests = await _sendMailService.ShowAllHelpRequests();
                return Ok(requests.Adapt<List<ResponseHelpModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("RequestHelp")]
        public async Task<IActionResult> UpdatehelpRequest(ResponseHelpModel requestData)
        {
            try
            {
                await _sendMailService.UpdateHelpRequest(requestData.Adapt<ResponseHelpDTO>());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
