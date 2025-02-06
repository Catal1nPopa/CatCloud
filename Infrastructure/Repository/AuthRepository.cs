using Domain.Entities.Auth;
using Domain.Interfaces;
using Helper.Serilog;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repository
{
    public class AuthRepository(CloudDbContext dbContext) : IAuthRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;
        public async Task AddUser(UserEntity userEntity)
        {
            //    try
            //    {
            var checkIfExist = await GetUserByUsername(userEntity.Username);
            if (checkIfExist == null)
            {
                _dbContext.Users.Add(userEntity);
                await _dbContext.SaveChangesAsync();
                LoggerHelper.LogInformation($"Utilizator creat : Email {userEntity.Email}, username: {userEntity.Username}, data : {userEntity.Added}, spatiu acordat : {userEntity.TotalStorage}");
            }
            else
                throw new Exception("Utilizator cu acest nume deja exista, alegeti alt nume va rog");
            //}
            //catch (NpgsqlException exception)
            //{
            //    LoggerHelper.LogInformation($"A aparut o eroare la adaugarea utilizatorului in baza de date - {exception}");
            //    throw new InvalidOperationException(exception.Message);
            //}
            //catch (Exception exception)
            //{
            //    LoggerHelper.LogInformation($"A aparut o eroare la adaugare utilizator - {exception}");
            //    throw new ApplicationException(exception.Message);
            //}
        }

        public async Task<UserEntity> GetUserByUsername(string username)
        {
            //try
            //{
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
            return user;
            //}
            //catch (NpgsqlException exception)
            //{
            //    LoggerHelper.LogInformation($"Eroare la obtinerea din baza de date a utilizatorului - {username}, exception - {exception}");
            //    throw new InvalidOperationException($"Eroare la obtinerea din baza de date a utilizatorului : {username}", exception);
            //}
            //catch (Exception exception)
            //{
            //    LoggerHelper.LogInformation($"Eroare la obtinerea utilizator - {username}, exception - {exception}");
            //    throw new ApplicationException($"Eroare la obtinerea utilizator : {username}");
            //}
        }

        public async Task<UserEntity> GetUserById(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
                return user;
            }
            catch (NpgsqlException exception)
            {
                LoggerHelper.LogInformation($"Eroare la obtinerea din baza de date a utilizatorului - {userId}, exception - {exception}");
                throw new InvalidOperationException($"Eroare la obtinerea din baza de date a utilizatorului : {userId}", exception);
            }
            catch (Exception exception)
            {
                LoggerHelper.LogInformation($"Eroare la obtinerea utilizator - {userId}, exception - {exception}");
                throw new ApplicationException($"Eroare la obtinerea utilizator : {userId}");
            }
        }
    }
}
