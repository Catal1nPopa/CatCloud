﻿using Domain.Entities.Chat;
using Domain.Entities.Files;
using Domain.Entities.Folder;
using Domain.Entities.Mail;
using Domain.Entities.Permission;
using Domain.Entities.UserGroup;
using System.Security.Cryptography;
using Domain.Entities.Notification;

namespace Domain.Entities.Auth
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Enabled { get; set; }
        public double TotalStorage { get; set; }
        public double AvailableStorage { get; set; }
        public DateTime Added { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpires { get; set; }

        public UserEntity() { }
        public bool CheckPassword(string password)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromHexString(Salt), 350000, HashAlgorithmName.SHA512, 32);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(Password));
        }

        public UserEntity(string username, string password, string salt, string email, bool emailConf, double totalSpace, double availableSpace, DateTime added, string? emailConfirmationToken, DateTime? emailConfirmationTokenExpires)
        {
            Username = username;
            Password = password;
            Salt = salt;
            EmailConfirmed = emailConf;
            TotalStorage = totalSpace;
            Email = email;
            AvailableStorage = availableSpace;
            Added = added;
            EmailConfirmationToken = emailConfirmationToken;
            EmailConfirmationTokenExpires = emailConfirmationTokenExpires;
        }

        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
        public ICollection<FileEntity> UploadedFiles { get; set; }
        public ICollection<GroupEntity> Groups { get; set; }
        public ICollection<UserGroupPermissionsEntity> UserGroupPermissions { get; set; }
        public ICollection<FolderEntity> Folders { get; set; }
        public ICollection<RequestHelpEntity> RequestHelps { get; set; } = new List<RequestHelpEntity>();
        public ICollection<ChatEntity> ChatMessages { get; set; } = new List<ChatEntity>();
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();

    }
}
