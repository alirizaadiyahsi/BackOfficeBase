using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.DataAccess.Extensions;
using BackOfficeBase.Domain.Entities;
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

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                    modelBuilder.Entity(entityType.ClrType).AddQueryFilter<ISoftDelete>(e => e.IsDeleted == false);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            SetAuditingProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SetAuditingProperties();
            return base.SaveChanges();
        }

        private void SetAuditingProperties()
        {
            foreach (var entry in ChangeTracker.Entries())
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
                        SetDeletionAuditedProperties(entry);
                        break;
                }
            }
        }

        private void SetDeletionAuditedProperties(EntityEntry entry)
        {
            if (entry.Entity is IHasDeletionTime objectWithDeletionTime)
            {
                objectWithDeletionTime.DeletionTime = DateTime.Now;
            }

            if (entry.Entity is IDeletionAudited objectWithDeleterUser)
            {
                objectWithDeleterUser.DeleterUserId = _currentUserId;
            }

            if (entry.Entity is ISoftDelete objectIsSoftDelete)
            {
                entry.State = EntityState.Modified;
                objectIsSoftDelete.IsDeleted = true;
            }
        }

        private void SetModificationAuditedProperties(EntityEntry entry)
        {
            if (entry.Entity is IHasModificationTime objectWithModificationTime)
            {
                objectWithModificationTime.ModificationTime = DateTime.Now;
            }

            if (entry.Entity is IModificationAudited objectWithModifierUser)
            {
                objectWithModifierUser.ModifierUserId = _currentUserId;
            }
        }

        private void SetCreationAuditedProperties(EntityEntry entry)
        {
            if (entry.Entity is IHasCreationTime objectWithCreationTime)
            {
                objectWithCreationTime.CreationTime = DateTime.Now;
            }

            if (entry.Entity is ICreationAudited objectWithCreatorUser)
            {
                objectWithCreatorUser.CreatorUserId = _currentUserId;
            }
        }
    }
}
