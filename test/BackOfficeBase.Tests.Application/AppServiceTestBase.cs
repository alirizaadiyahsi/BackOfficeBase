using System;
using System.Globalization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared;
using BackOfficeBase.Tests.Shared.DataAccess;

namespace BackOfficeBase.Tests.Application
{
    public class AppServiceTestBase : TestBase
    {
        protected readonly TestBackOfficeBaseDbContext DbContextTest;

        public AppServiceTestBase()
        {
            DbContextTest = GetDbContextTest();
        }

        public static void AddUserToRole(User testUser, Role testRole)
        {
            var testUserRole = new UserRole
            {
                User = testUser,
                Role = testRole
            };

            testUser.UserRoles.Add(testUserRole);
            testRole.UserRoles.Add(testUserRole);
        }

        public static Role GetTestRole(string roleName = "TestRoleName")
        {
            var testRole = new Role
            {
                Name = roleName
            };

            return testRole;
        }

        public static User GetTestUser(string userName = "TestUserName", string email = "testuser@mail.com")
        {
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = email,
                IsDeleted = false,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(CultureInfo.GetCultureInfo("en-US")),
                NormalizedUserName = userName.ToUpper(CultureInfo.GetCultureInfo("en-US")),
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            return testUser;
        }
    }
}
