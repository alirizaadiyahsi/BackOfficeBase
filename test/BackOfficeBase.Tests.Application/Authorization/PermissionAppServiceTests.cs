using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class PermissionAppServiceTests : AppServiceTestBase
    {
        private readonly IPermissionAppService _permissionAppService;

        private static string TestPermissionClaimForUser = "TestPermissionClaimForUser";
        private static string TestPermissionClaimForRole = "TestPermissionClaimForRoe";

        public PermissionAppServiceTests()
        {
            var testUser = GetTestUser();
            var testRole = GetTestRole();
            AddUserToRole(testUser, testRole);

            var mockUserClaimStore = SetupMockUserClaimStore(testUser);
            var mockRoleClaimStore = SetupMockRoleClaimStore(testRole);

            var userManager = new UserManager<User>(mockUserClaimStore.Object, null, null, null, null, null, null, null, null);
            var roleManager = new RoleManager<Role>(mockRoleClaimStore.Object, null, null, null, null);
            _permissionAppService = new PermissionAppService(userManager, roleManager);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", TestPermissionClaimForUser);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User_Role()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", TestPermissionClaimForRole);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Not_Permission_Granted_To_User()
        {
            var isPermissionNotGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", "NotGrantedPermissionClaim");

            Assert.False(isPermissionNotGranted);
        }

        private static Mock<IRoleClaimStore<Role>> SetupMockRoleClaimStore(Role testRole)
        {
            var mockRoleClaimStore = new Mock<IRoleClaimStore<Role>>();
            mockRoleClaimStore.Setup(x => x.GetClaimsAsync(testRole, CancellationToken.None)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForRole)
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
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForUser)
                });

            return mockUserClaimStore;
        }
    }
}
