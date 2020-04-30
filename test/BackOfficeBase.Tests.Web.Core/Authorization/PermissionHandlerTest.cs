using System.Collections.Generic;
using System.Security.Claims;
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
        private readonly User _testUser;
        private readonly Role _testRole;

        public PermissionHandlerTest()
        {
            _testRole = GetTestRole();
            _testUser = GetTestUser();
            AddUserToRole(_testUser, _testRole);

            var userManagerMock = SetupUserManagerMock();
            var roleManagerMock = SetupRoleManagerMock();

            _permissionAppService = new PermissionAppService(userManagerMock.Object, roleManagerMock.Object);
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

        private Mock<UserManager<User>> SetupUserManagerMock()
        {
            var userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null,
                null, null, null);
            userManagerMock.Setup(x => x.FindByNameAsync(_testUser.UserName)).ReturnsAsync(_testUser);
            userManagerMock.Setup(x => x.GetClaimsAsync(_testUser)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForUser)
                });
            return userManagerMock;
        }

        private Mock<RoleManager<Role>> SetupRoleManagerMock()
        {
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, null, null, null, null);
            roleManagerMock.Setup(x => x.GetClaimsAsync(_testRole)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForRole)
                });
            return roleManagerMock;
        }
    }
}
