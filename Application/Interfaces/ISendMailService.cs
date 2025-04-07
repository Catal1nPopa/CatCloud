
using Application.DTOs.Mail;

namespace Application.Interfaces
{
    public interface ISendMailService
    {
        Task<bool> SendReuqestMoreSpace();
        Task<bool> RequestHelp(RequestHelpDTO requestHelp);
    }
}
