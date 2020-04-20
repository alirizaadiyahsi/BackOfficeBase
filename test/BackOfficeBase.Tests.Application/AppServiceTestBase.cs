using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BackOfficeBase.Tests.Application
{
    public class AppServiceTestBase : TestBase
    {
        protected readonly UserManager<User> UserManager;
        protected readonly RoleManager<Role> RoleManager;

        public AppServiceTestBase()
        {
            var testUser = GetTestUser();
            var testRole = GetTestRole();
            AddUserToRole(testUser, testRole);

            var mockUserClaimStore = SetupMockUserClaimStore(testUser);
            var mockRoleClaimStore = SetupMockRoleClaimStore(testRole);

            var userManager = new UserManager<User>(mockUserClaimStore.Object, null, null, null, null, null, null, null, null);
            var roleManager = new RoleManager<Role>(mockRoleClaimStore.Object, null, null, null, null);

            UserManager = userManager;
            RoleManager = roleManager;
        }

        private static Mock<IRoleClaimStore<Role>> SetupMockRoleClaimStore(Role testRole)
        {
            var mockRoleClaimStore = new Mock<IRoleClaimStore<Role>>();
            mockRoleClaimStore.Setup(x => x.GetClaimsAsync(testRole, CancellationToken.None)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, "TestPermissionForRoleClaim")
                });
            return mockRoleClaimStore;
        }

        private static Mock<IUserClaimStore<User>> SetupMockUserClaimStore(User testUser)
        {
            var mockUserClaimStore = new Mock<IUserClaimStore<User>>();
            mockUserClaimStore.Setup(x => x.FindByNameAsync(testUser.UserName, CancellationToken.None)).ReturnsAsync(testUser);
            mockUserClaimStore.Setup(x => x.GetClaimsAsync(testUser, CancellationToken.None)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, "TestPermissionForUserClaim")
                });
            return mockUserClaimStore;
        }

        private static void AddUserToRole(User testUser, Role testRole)
        {
            var testUserRole = new UserRole
            {
                User = testUser,
                Role = testRole
            };

            testUser.UserRoles.Add(testUserRole);
            testRole.UserRoles.Add(testUserRole);
        }

        private static Role GetTestRole(string roleName = "TestRoleName")
        {
            var testRole = new Role
            {
                Name = "TestRoleName"
            };

            return testRole;
        }

        private static User GetTestUser(string userName = "TestUserName")
        {
            var testUser = new User
            {
                UserName = userName
            };

            return testUser;
        }
    }
}
