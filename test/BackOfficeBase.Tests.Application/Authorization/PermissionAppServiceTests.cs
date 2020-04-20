using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class PermissionAppServiceTests : AppServiceTestBase
    {
        private readonly IPermissionAppService _permissionAppService;

        public PermissionAppServiceTests()
        {
            _permissionAppService = new PermissionAppService(UserManager, RoleManager);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", "TestPermissionForUserClaim");

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User_Role()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", "TestPermissionForRoleClaim");

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Not_Permission_Granted_To_User()
        {
            var isPermissionNotGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", "TestPermissionForUserClaim1");

            Assert.False(isPermissionNotGranted);
        }

        [Fact]
        public async Task Should_Not_Permission_Granted_To_User_Role()
        {
            var isPermissionNotGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync("TestUserName", "TestPermissionForRoleClaim1");

            Assert.False(isPermissionNotGranted);
        }
    }
}
