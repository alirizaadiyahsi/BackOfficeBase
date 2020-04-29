using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class PermissionAppServiceTests : AppServiceTestBase
    {
        private readonly IPermissionAppService _permissionAppService;
        private static readonly string TestPermissionClaimForUser = "TestPermissionClaimForUser";
        private static readonly string TestPermissionClaimForRole = "TestPermissionClaimForRoe";
        private readonly User _testUser = GetTestUser();
        private readonly Role _testRole = GetTestRole();

        public PermissionAppServiceTests()
        {
            AddUserToRole(_testUser, _testRole);

            var userManagerMock = SetupMockUserManager();
            var roleManagerMock = SetupMockRoleManager();

            _permissionAppService = new PermissionAppService(userManagerMock.Object, roleManagerMock.Object);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, TestPermissionClaimForUser);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User_Role()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, TestPermissionClaimForRole);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Not_Permission_Granted_To_User()
        {
            var isPermissionNotGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, "NotGrantedPermissionClaim");

            Assert.False(isPermissionNotGranted);
        }

        private Mock<UserManager<User>> SetupMockUserManager()
        {
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null,
                null, null, null);
            mockUserManager.Setup(x => x.FindByNameAsync(_testUser.UserName)).ReturnsAsync(_testUser);
            mockUserManager.Setup(x => x.GetClaimsAsync(_testUser)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForUser)
                });
            return mockUserManager;
        }

        private Mock<RoleManager<Role>> SetupMockRoleManager()
        {
            var mockRoleManager = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, null, null, null, null);
            mockRoleManager.Setup(x => x.GetClaimsAsync(_testRole)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForRole)
                });
            return mockRoleManager;
        }
    }
}
