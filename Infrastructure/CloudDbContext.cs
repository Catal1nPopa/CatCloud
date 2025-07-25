﻿using Domain.Entities.Auth;
using Domain.Entities.Chat;
using Domain.Entities.Files;
using Domain.Entities.Folder;
using Domain.Entities.Mail;
using Domain.Entities.Notification;
using Domain.Entities.Permission;
using Domain.Entities.UserGroup;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CloudDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UserGroupEntity> UserGroups { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<RolePermissionEntity> RolePermissions { get; set; }
        public DbSet<UserGroupPermissionsEntity> UserGroupPermissions { get; set; }
        public DbSet<GroupPermissionsEntity> GroupPermissions { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<FileUserShareEntity> FileUserShares { get; set; }
        public DbSet<FileGroupShareEntity> FileGroupShares { get; set; }
        public DbSet<FolderEntity> Folders { get; set; }
        public DbSet<RequestHelpEntity> RequestHelps { get; set; }
        public DbSet<ChatEntity> ChatMessages { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CatCloud;Username=postgres;Password=admin;IncludeErrorDetail=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupEntity>()
                .HasOne(g => g.Owner)
                .WithMany()
                .HasForeignKey(g => g.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupEntity>()
                .HasMany(f => f.UserEntities)
                .WithMany(u => u.Groups)
                .UsingEntity<UserGroupEntity>(f => f.HasKey(fg => new { fg.GroupId, fg.UserId }));

            modelBuilder.Entity<RoleEntity>()
                .HasMany(permissions => permissions.Permissions)
                .WithMany(roles => roles.Roles)
                .UsingEntity<RolePermissionEntity>(f => f.HasKey(fg => new { fg.RoleId, fg.PermissionId }));

            modelBuilder.Entity<UserGroupPermissionsEntity>()
            .HasKey(ugp => new { ugp.UserId, ugp.GroupId, ugp.PermissionId });

            modelBuilder.Entity<UserGroupPermissionsEntity>()
                .HasOne(ugp => ugp.User)
                .WithMany(u => u.UserGroupPermissions)
                .HasForeignKey(ugp => ugp.UserId);

            modelBuilder.Entity<UserGroupPermissionsEntity>()
                .HasOne(ugp => ugp.Group)
                .WithMany(g => g.UserGroupPermissions)
                .HasForeignKey(ugp => ugp.GroupId);

            modelBuilder.Entity<UserGroupPermissionsEntity>()
                .HasOne(ugp => ugp.Permission)
                .WithMany(gp => gp.UserGroupPermissions)
                .HasForeignKey(ugp => ugp.PermissionId);

            modelBuilder.Entity<FileEntity>()
                .HasOne(g => g.Owner)
                .WithMany()
                .HasForeignKey(g => g.UploadedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.UserEntities)
                .WithMany(u => u.UploadedFiles)
                .UsingEntity<FileUserShareEntity>(f => f.HasKey(fg => new { fg.FileId, fg.UserId }));

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.GroupEntities)
                .WithMany(u => u.UploadedFiles)
                .UsingEntity<FileGroupShareEntity>(f => f.HasKey(fg => new { fg.FileId, fg.GroupId }));

            modelBuilder.Entity<FolderEntity>()
                .HasOne(owner => owner.Owner)
                .WithMany(folders => folders.Folders)
                .HasForeignKey(fkey => fkey.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileEntity>()
                .HasOne(folder => folder.Folder)
                .WithMany(folder => folder.Files)
                .HasForeignKey(fkey => fkey.FolderId)
                .OnDelete(DeleteBehavior.Cascade); // cand stergi un fodler se sterge si fisiere, pentru a nu stergi setam la SetNull

            modelBuilder.Entity<RequestHelpEntity>()
                .HasOne(r => r.User)
                .WithMany(u => u.RequestHelps)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ChatEntity>()
                .HasOne(c => c.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ChatEntity>()
                .HasOne(c => c.Group)
                .WithMany(g => g.ChatMessages)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<NotificationEntity>()
                .HasOne(n => n.User)
                .WithMany(notifications => notifications.Notifications)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
