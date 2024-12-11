using Domain.Entities.Auth;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repository
{
    public class AuthRepository(CloudDbContext dbContext) : IAuthRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;
        public async Task AddUser(UserEntity userEntity)
        {
            try
            {
                _dbContext.Users.Add(userEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch (NpgsqlException exception)
            {
                //log exception
                throw new InvalidOperationException("A aparut o eroare la adaugarea utilizatorului in baza de date.", exception);
            }
            catch (Exception exception)
            {
                //log exception
                throw new ApplicationException("A aparut o eroare la adaugare utilizator.", exception);
            }
        }

        public async Task<UserEntity> GetUserByUsername(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
                return user;
            }
            catch (NpgsqlException exception)
            {
                throw new InvalidOperationException($"Eroare la obtinerea din baza de date a utilizatorului : {username}");
            }
            catch (Exception exception)
            {
                throw new ApplicationException($"Eroare la obtinerea utilizator : {username}");
            }

        }
    }
}
