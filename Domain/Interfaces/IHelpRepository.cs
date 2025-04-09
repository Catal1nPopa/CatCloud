
using Domain.Entities.Mail;

namespace Domain.Interfaces
{
    public interface IHelpRepository
    {
        Task RequestHelp(RequestHelpEntity helpEntity);
        Task<List<ResponseHelpEntity>> ShowAllHelpRequests();
        Task UpdateHelpRequest(ResponseHelpEntity helpRequest);
    }
}
