

using Application.Interfaces;
using System.Net.Mail;
using System.Net;
using Application.DTOs.Mail;
using Domain.Interfaces;
using Mapster;
using Domain.Entities.Mail;

namespace Application.Services
{
    public class SendMailService(IAuthService authService, IHelpRepository helpRepository) : ISendMailService
    {
        //yvqs lfrj osaa ygqo
        private readonly IAuthService _authService = authService;
        private readonly IHelpRepository _helpRepository = helpRepository;
        public async Task<bool> SendReuqestMoreSpace()
        {
            try
            {
                var user = await _authService.GetUser();

                using ( var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("misterco2002@gmail.com", "yvqs lfrj osaa ygqo"); 

                    using (var message = new MailMessage(
                        from: new MailAddress(user.Email, user.Username),
                        to: new MailAddress("catalin.p2002@gmail.com", "admins")))
                    {
                        message.Subject = "Cat Storage - increase space!";
                        message.Body = $"User {user.Username} request more space. Available space now : { Math.Round(user.AvailableStorage / (1024.0 * 1024.0), 2)} MB. Total space : {Math.Round(user.TotalStorage / (1024.0 * 1024.0), 2)} MB";

                        await client.SendMailAsync(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<bool> RequestHelp(RequestHelpDTO requestHelp)
        {
            try
            {
                var user = await _authService.GetUser();

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("misterco2002@gmail.com", "yvqs lfrj osaa ygqo");

                    using (var message = new MailMessage(
                        from: new MailAddress(user.Email, user.Username),
                        to: new MailAddress("catalin.p2002@gmail.com", "admins")))
                    {
                        message.Subject = $"Cat Storage - help | {requestHelp.topic}!";
                        message.Body = $"User {user.Username} request help \n. " +
                            $"{requestHelp.body}";

                        await client.SendMailAsync(message);
                    }
                }
                HelpRequestDTO helpDTO = new HelpRequestDTO();
                helpDTO.UserId = user.Id;
                helpDTO.CreatedDate = DateTime.UtcNow;
                helpDTO.Message = requestHelp.body;
                helpDTO.Topic = requestHelp.topic;
                helpDTO.Status = "Open";
                await _helpRepository.RequestHelp(helpDTO.Adapt<RequestHelpEntity>());

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
