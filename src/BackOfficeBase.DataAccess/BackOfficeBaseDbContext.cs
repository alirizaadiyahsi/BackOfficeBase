using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Domain.Entities.Auditing;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Domain.Entities.OrganizationUnits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOfficeBase.DataAccess
{
    public class BackOfficeBaseDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        // TODO: Move following line to another class to use from another classes too. And write an extension method to get userId
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Guid _currentUserId;

        public BackOfficeBaseDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = new Guid(_httpContextAccessor.HttpContext.User.FindFirstValue("Id"));
        }

        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<OrganizationUnitUser> OrganizationUnitUsers { get; set; }
        public DbSet<OrganizationUnitRole> OrganizationUnitRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");
            modelBuilder.Entity<UserToken>().ToTable("UserToken");
            modelBuilder.Entity<OrganizationUnit>().ToTable("OrganizationUnit");

            modelBuilder.Entity((Action<EntityTypeBuilder<User>>)(b =>
            {
                b.ToTable("User");

                b.HasOne(x => x.CreatorUser)
                .WithMany()
                .HasForeignKey(x => x.CreatorUserId);

                b.HasOne(x => x.ModifierUser)
                    .WithMany()
                    .HasForeignKey(x => x.ModifierUserId);

                b.HasOne(x => x.DeleterUser)
                    .WithMany()
                    .HasForeignKey(x => x.DeleterUserId);
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<UserRole>>)(b =>
            {
                b.ToTable("UserRole");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<OrganizationUnitUser>>)(b =>
            {
                b.ToTable("OrganizationUnitUser");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.OrganizationUnitUsers)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.OrganizationUnit)
                    .WithMany(r => r.OrganizationUnitUsers)
                    .HasForeignKey(ur => ur.OrganizationUnitId);
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<OrganizationUnitRole>>)(b =>
            {
                b.ToTable("OrganizationUnitRole");

                b.HasOne(ur => ur.Role)
                    .WithMany(u => u.OrganizationUnitRoles)
                    .HasForeignKey(ur => ur.RoleId);

                b.HasOne(ur => ur.OrganizationUnit)
                    .WithMany(r => r.OrganizationUnitRoles)
                    .HasForeignKey(ur => ur.OrganizationUnitId);
            }));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                SetAuditingProperties(entry);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        // TODO: Write tests for auditing property settings
        private void SetAuditingProperties(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreationAuditedProperties(entry);
                    break;
                case EntityState.Modified:
                    SetModificationAuditedProperties(entry);
                    break;
                case EntityState.Deleted:
                    // TODO: Implement oft delete
                    SetDeletionAuditedProperties(entry);
                    break;
            }
        }

        private void SetDeletionAuditedProperties(EntityEntry entry)
        {
            if (entry is IHasDeletionTime objectWithDeletionTime)
            {
                objectWithDeletionTime.DeletionTime = DateTime.Now;
            }

            if (entry is IDeletionAudited objectWithDeleterUser)
            {
                objectWithDeleterUser.DeleterUserId = _currentUserId;
            }
        }

        private void SetModificationAuditedProperties(EntityEntry entry)
        {
            if (entry is IHasModificationTime objectWithModificationTime)
            {
                objectWithModificationTime.ModificationTime = DateTime.Now;
            }

            if (entry is IModificationAudited objectWithModifierUser)
            {
                objectWithModifierUser.ModifierUserId = _currentUserId;
            }
        }

        private void SetCreationAuditedProperties(EntityEntry entry)
        {
            if (entry is IHasCreationTime objectWithCreationTime)
            {
                objectWithCreationTime.CreationTime = DateTime.Now;
            }

            if (entry is ICreationAudited objectWithCreatorUser)
            {
                objectWithCreatorUser.CreatorUserId = _currentUserId;
            }
        }
    }
}
