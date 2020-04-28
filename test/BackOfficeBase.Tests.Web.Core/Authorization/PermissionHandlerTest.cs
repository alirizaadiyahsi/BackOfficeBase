using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Web.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Web.Core.Authorization
{
    public class PermissionHandlerTest : WebCoreTestBase
    {
        private readonly IPermissionAppService _permissionAppService;
        private static readonly string TestPermissionClaimForUser = "TestPermissionClaimForUser";
        private static readonly string TestPermissionClaimForRole = "TestPermissionClaimForRoe";
        private readonly User _testUser = GetTestUser();
        private readonly Role _testRole = GetTestRole();

        public PermissionHandlerTest()
        {
            AddUserToRole(_testUser, _testRole);

            var mockUserManager = SetupMockUserManager();
            var mockRoleManager = SetupMockRoleManager();

            _permissionAppService = new PermissionAppService(mockUserManager.Object, mockRoleManager.Object);
        }

        [Fact]
        public async Task Should_User_Has_Permission()
        {
            var requirements = new List<PermissionRequirement>
            {
                new PermissionRequirement(TestPermissionClaimForUser)
            };

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, _testUser.UserName)
                    },
                    "TestAuthorizationType"
                )
            );

            var authorizationHandlerContext = new AuthorizationHandlerContext(requirements, claimsPrincipal, null);
            var permissionHandler = new PermissionHandler(_permissionAppService);
            await permissionHandler.HandleAsync(authorizationHandlerContext);

            Assert.True(authorizationHandlerContext.HasSucceeded);
        }

        [Fact]
        public async Task Should_Not_User_Has_Permission()
        {
            var requirements = new List<PermissionRequirement>
            {
                new PermissionRequirement("NotGrantedPermissionClaim")
            };

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, _testUser.UserName)
                    },
                    "TestAuthorizationType"
                )
            );

            var authorizationHandlerContext = new AuthorizationHandlerContext(requirements, claimsPrincipal, null);
            var permissionHandler = new PermissionHandler(_permissionAppService);
            await permissionHandler.HandleAsync(authorizationHandlerContext);

            Assert.False(authorizationHandlerContext.HasSucceeded);
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
