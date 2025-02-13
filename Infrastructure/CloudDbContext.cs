using Domain.Entities.Auth;
using Domain.Entities.Files;
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
        public DbSet<UserGroupRoleEntity> UserGroupRoles { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<FileUserShareEntity> FileUserShares { get; set; }
        public DbSet<FileGroupShareEntity> FileGroupShares { get; set; }

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

            //---------------------------------------------------
            // Configurare pentru UserGroupRole - cheie compusa
            modelBuilder.Entity<UserGroupRoleEntity>()
                .HasKey(ugr => new { ugr.UserId, ugr.GroupId, ugr.RoleId });

            modelBuilder.Entity<UserGroupRoleEntity>()
                .HasOne(ugr => ugr.User)
                .WithMany(u => u.UserGroupRoles)
                .HasForeignKey(ugr => ugr.UserId);

            modelBuilder.Entity<UserGroupRoleEntity>()
                .HasOne(ugr => ugr.Group)
                .WithMany(g => g.UserGroupRoles)
                .HasForeignKey(ugr => ugr.GroupId);

            modelBuilder.Entity<UserGroupRoleEntity>()
                .HasOne(ugr => ugr.Role)
                .WithMany()
                .HasForeignKey(ugr => ugr.RoleId);

            // Configurare pentru RolePermission - cheie compusa
            modelBuilder.Entity<RolePermissionEntity>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermissionEntity>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermissionEntity>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);

            //configurare user role many-to-many
            modelBuilder.Entity<UserRoleEntity>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // ------------------------------------------

            modelBuilder.Entity<FileEntity>()
                .HasOne(g => g.Owner)
                .WithMany()
                .HasForeignKey(g => g.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.UserEntities)
                .WithMany(u => u.UploadedFiles)
                .UsingEntity<FileUserShareEntity>(f => f.HasKey(fg => new { fg.FileId, fg.UserId }));

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.GroupEntities)
                .WithMany(u => u.UploadedFiles)
                .UsingEntity<FileGroupShareEntity>(f => f.HasKey(fg => new { fg.FileId, fg.GroupId }));

            base.OnModelCreating(modelBuilder);
        }
    }
}
