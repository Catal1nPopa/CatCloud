
using Domain.Entities.Files;
using Domain.Entities.Mail;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository
{
    public class HelpRepository(CloudDbContext cloudDbContext) : IHelpRepository
    {
        private readonly CloudDbContext _cloudDbContext = cloudDbContext;
        public async Task RequestHelp(RequestHelpEntity helpEntity)
        {
            try
            {
                _cloudDbContext.RequestHelps.Add(helpEntity);
                await _cloudDbContext.SaveChangesAsync();
            }
            catch (PostgresException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex) 
            {
                throw new Exception($"Eroare la salvare request in baza de date, utilizator || {helpEntity.UserId}| {ex.Message}");
            }
        }

        public async Task<List<ResponseHelpEntity>> ShowAllHelpRequests()
        {
            try
            {
                List<ResponseHelpEntity> data = await _cloudDbContext.RequestHelps
                    .Include(r => r.User) 
                    .Select(r => new ResponseHelpEntity
                        {
                            Id = r.Id,
                            UserMail = r.User.Email,
                            Username = r.User.Username,
                            CreatedDate = r.CreatedDate,
                            UserId = r.UserId,
                            Topic = r.Topic,
                            Message = r.Message,
                            Status = r.Status
                        }).ToListAsync();


                return data;
            }
            catch(Exception ex)
            {
                throw new Exception($"Eroare la obtinerea datelor {ex.Message}");
            }
        }

        public async Task UpdateHelpRequest(ResponseHelpEntity helpRequest)
        {
            try
            {
                RequestHelpEntity row = await _cloudDbContext.RequestHelps.FirstOrDefaultAsync( user => user.Id == helpRequest.Id);
                if (row != null)
                {
                    row.Status = helpRequest.Status;
                    row.CreatedDate = helpRequest.CreatedDate;
                    await _cloudDbContext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Eroare la actualizarea datelor {ex.Message}");
            }
        } 
    }
}
