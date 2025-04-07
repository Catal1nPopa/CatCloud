
using Domain.Entities.Mail;

namespace Domain.Interfaces
{
    public interface IHelpRepository
    {
        Task RequestHelp(RequestHelpEntity helpEntity);
    }
}
