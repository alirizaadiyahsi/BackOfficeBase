using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using BackOfficeBase.Domain.AppConsts.Authorization;
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

            var mockUserClaimStore = SetupMockUserClaimStore(_testUser);
            var mockRoleClaimStore = SetupMockRoleClaimStore(_testRole);

            var userManager = new UserManager<User>(mockUserClaimStore.Object, null, null, null, null, null, null, null, null);
            var roleManager = new RoleManager<Role>(mockRoleClaimStore.Object, null, null, null, null);
            _permissionAppService = new PermissionAppService(userManager, roleManager);
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
