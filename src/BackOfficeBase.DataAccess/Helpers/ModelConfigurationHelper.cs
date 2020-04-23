using System;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Domain.Entities.OrganizationUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOfficeBase.DataAccess.Helpers
{
    public class ModelConfigurationHelper
    {
        public static void SetModelConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");
            modelBuilder.Entity<UserToken>().ToTable("UserToken");
            modelBuilder.Entity<OrganizationUnit>().ToTable("OrganizationUnit");

            modelBuilder.Entity((Action<EntityTypeBuilder<User>>) (b =>
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

            modelBuilder.Entity((Action<EntityTypeBuilder<UserRole>>) (b =>
            {
                b.ToTable("UserRole");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<OrganizationUnitUser>>) (b =>
            {
                b.ToTable("OrganizationUnitUser");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.OrganizationUnitUsers)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.OrganizationUnit)
                    .WithMany(r => r.OrganizationUnitUsers)
                    .HasForeignKey(ur => ur.OrganizationUnitId);
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<OrganizationUnitRole>>) (b =>
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
    }
}
