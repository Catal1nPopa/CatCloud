
using Domain.Entities.Files;
using Domain.Entities.Mail;
using Domain.Interfaces;
using Npgsql;

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
            catch
            {
                throw new Exception($"Eroare la salvare request in baza de date, utilizator || {helpEntity.UserId}");
            }
        }
    }
}
