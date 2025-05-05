using Domain.Entities.Auth;
using Domain.Entities.Permission;
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
            try
            {
                var checkIfExistName = await GetUserByUsername(userEntity.Username);
                if (checkIfExistName != null)
                {
                    throw new Exception("Utilizator cu acest nume deja exista");
                }
                var checkIfExistMail = await GetUserByEmail(userEntity.Email);
                if (checkIfExistMail != null)
                {
                    throw new Exception("Utilizator cu acest email deja exista");
                }

                if (checkIfExistName == null && checkIfExistMail == null)
                {
                    userEntity.Enabled = true;
                    _dbContext.Users.Add(userEntity);
                    await _dbContext.SaveChangesAsync();
                    LoggerHelper.LogInformation($"Utilizator creat : Email {userEntity.Email}, username: {userEntity.Username}, data : {userEntity.Added}, spatiu acordat : {userEntity.TotalStorage}");
                }
            }
            catch (NpgsqlException exception)
            {
                LoggerHelper.LogInformation($"A aparut o eroare la adaugarea utilizatorului in baza de date - {exception}");
                throw new InvalidOperationException(exception.Message);
            }
            catch (Exception exception)
            {
                LoggerHelper.LogInformation($"A aparut o eroare la adaugare utilizator - {exception}");
                throw new ApplicationException(exception.Message);
            }
        }

        public async Task<UserEntity> GetUserByUsername(string username)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(user => user.Username == username);
            return user;
        }

        public async Task<List<string>> GetUserRole(Guid userId)
        {
            try
            {
                var userRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == userId)
                        .Select(ur => ur.Role.Name) 
                        .ToListAsync();
                return userRoles;
            }
            catch(Exception exception)
            {
                throw new Exception($"{exception.Message}");
            }
        }

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
                return user;
            }
            catch (NpgsqlException exception)
            {
                LoggerHelper.LogInformation($"Eroare la obtinerea din baza de date a utilizatorului - {email}, exception - {exception}");
                throw new InvalidOperationException($"Eroare la obtinerea din baza de date a utilizatorului : {email}", exception);
            }
            catch (Exception exception)
            {
                LoggerHelper.LogInformation($"Eroare la obtinerea utilizator - {email}, exception - {exception}");
                throw new ApplicationException($"Eroare la obtinerea utilizator : {email}");
            }
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

        public async Task DeleteUser(Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Eroare la stergere utilizator | {userId} | {exception}");
            }
        }

        public async Task<List<UserInfoEntity>> GetAllUsers()
        {
            var users = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(f => new UserInfoEntity
            {
                Id = f.Id,
                Username = f.Username,
                Email = f.Email,
                EmailConfirmed = f.EmailConfirmed,
                TotalStorage = f.TotalStorage,
                AvailableStorage = f.AvailableStorage,
                Enabled = f.Enabled,
                Added = f.Added,
                Role = f.UserRoles.Select(u => u.Role.Name).ToList()
            }).ToListAsync();

            return users;
        }

        public async Task<bool> DecreaseAvailableSize(long fileSize, Guid userId)
        {
            try     
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
                user.AvailableStorage = user.AvailableStorage - fileSize;
                if(user.AvailableStorage < 0)
                {
                    throw new Exception($"Nu aveti memorie disponibila");
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception($"Erroare la modificare spatiu {ex.Message}");
            }
        }

        public async Task<bool> IncreaseAvailableSize(long fileSize, Guid userId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
                user.AvailableStorage = user.AvailableStorage + fileSize;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erroare la modificare spatiu {ex.Message}");
            }
        }
        public async Task<List<UserEntity>> GetUsersNotSharedWithFile(Guid fileId, Guid currentUserId)
        {
            var sharedUserIds = _dbContext.FileUserShares
                .Where(fus => fus.FileId == fileId)
                .Select(fus => fus.UserId);

            var usersNotShared = await _dbContext.Users
                .Where(u => !sharedUserIds.Contains(u.Id) && u.Id != currentUserId)
                .ToListAsync();

            return usersNotShared;
        }

        public async Task ConfirmEmail(string token)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.EmailConfirmationToken == token);
                if (user == null || user.EmailConfirmationTokenExpires < DateTime.UtcNow)
                {
                    throw new Exception("Token invalid sau expirat.");
                }
                user.EmailConfirmed = true;
                user.EmailConfirmationToken = null;
                user.EmailConfirmationTokenExpires = null;
                
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erroare confirmare email - {token}");
            }
        }

        public async Task UpdateUser(UserInfoEntity userEntity)
        {
            try
            {

                var user = await _dbContext.Users
                   .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == userEntity.Id); 
                if (user == null)
                    throw new Exception("Utilizatorul nu a fost găsit.");

                double UsedMemory = user.TotalStorage - user.AvailableStorage; 
                user.AvailableStorage = userEntity.TotalStorage - UsedMemory;
                user.Email = userEntity.Email;
                user.TotalStorage = userEntity.TotalStorage;
                user.Enabled = userEntity.Enabled;

                _dbContext.UserRoles.RemoveRange(user.UserRoles);

                var rolesFromDb = await _dbContext.Roles
                    .Where(r => userEntity.Role.Contains(r.Name))
                    .ToListAsync();

                foreach (var role in rolesFromDb)
                {
                    user.UserRoles.Add(new UserRoleEntity
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Eroare la actualizare utilizator, {ex.Message}");
            }
        }
    }
}
