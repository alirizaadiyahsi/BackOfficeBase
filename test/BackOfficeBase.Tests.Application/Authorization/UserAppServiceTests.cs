using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class UserAppServiceTests : AppServiceTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly TestBackOfficeBaseDbContext _dbContext;

        public UserAppServiceTests()
        {
            _dbContext = GetTestDbContext();
            var mapper = GetNewHostServiceProvider().GetRequiredService<IMapper>();
            _userAppService = new UserAppService(_dbContext, mapper);
        }

        [Fact]
        public async Task Should_Get_Async()
        {
            var testUser = GetTestUser("test_user_for_user_app_service_get", "test_user_for_user_app_service_get@mail.com");
            var testRole = GetTestRole("test_role_for_user_app_service_get");
            AddUserToRole(testUser, testRole);

            await _dbContext.Users.AddAsync(testUser);
            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.UserClaims.AddAsync(new UserClaim
            {
                UserId = testUser.Id,
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = AppPermissions.Users.Read
            });
            await _dbContext.SaveChangesAsync();

            var userOutput = await _userAppService.GetAsync(testUser.Id);

            Assert.NotNull(userOutput);
            Assert.True(userOutput.AllRoles != null && userOutput.AllRoles.Any());
            Assert.True(userOutput.SelectedPermissions != null && userOutput.SelectedPermissions.Any());
            Assert.True(userOutput.SelectedRoleIds != null && userOutput.SelectedRoleIds.Any());
        }

        [Fact]
        public async Task Should_Create_Async()
        {
            var testRole = GetTestRole("test_role_for_user_app_service_create");
            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.SaveChangesAsync();

            var createUserInput = new CreateUserInput
            {
                UserName = "test_user_for_user_app_service_create",
                Email = "test_user_for_user_app_service_create@mail.com",
                SelectedRoleIds = new[] { testRole.Id },
                SelectedPermissions = new[] { AppPermissions.Users.Read }
            };
            var userOutput = await _userAppService.CreateAsync(createUserInput);
            await _dbContext.SaveChangesAsync();

            var insertedUser = await GetTestDbContext().Users.FindAsync(userOutput.Id);

            Assert.NotNull(userOutput);
            Assert.True(userOutput.SelectedRoleIds != null && userOutput.SelectedRoleIds.Any(x => x == testRole.Id));
            Assert.True(userOutput.SelectedPermissions!= null && userOutput.SelectedPermissions.Any(x => x == AppPermissions.Users.Read));

            Assert.NotNull(insertedUser);
            Assert.True(insertedUser.UserRoles != null && insertedUser.UserRoles.Any(x => x.RoleId == testRole.Id));
            Assert.True(insertedUser.UserClaims != null && insertedUser.UserClaims.Any(x => x.ClaimValue == AppPermissions.Users.Read));
        }

        [Fact]
        public async Task Should_Update()
        {
            var testUser = GetTestUser("test_user_for_user_app_service_update", "test_user_for_user_app_service_update@mail.com");
            var grantedRole = GetTestRole("test_role_for_user_app_service_update1");
            var roleToGrant = GetTestRole("test_role_for_user_app_service_update2");

            await _dbContext.UserRoles.AddAsync(new UserRole { Role = grantedRole, User = testUser });
            await _dbContext.UserClaims.AddAsync(new UserClaim { User = testUser, ClaimType = CustomClaimTypes.Permission, ClaimValue = AppPermissions.Users.Read });
            await _dbContext.Users.AddAsync(testUser);
            await _dbContext.Roles.AddAsync(grantedRole);
            await _dbContext.Roles.AddAsync(roleToGrant);
            await _dbContext.SaveChangesAsync();

            var updateUserInput = new UpdateUserInput
            {
                UserName = "test_user_for_user_app_service_update_updated",
                Email = "test_user_for_user_app_service_update_updated@mail.com",
                SelectedRoleIds = new[] { roleToGrant.Id },
                SelectedPermissions = new[] { AppPermissions.Users.Create }
            };
            var userOutput = _userAppService.Update(updateUserInput);
            _dbContext.SaveChanges();

            var updatedUser = await GetTestDbContext().Users.FindAsync(userOutput.Id);

            Assert.NotNull(userOutput);
            Assert.True(userOutput.SelectedRoleIds != null && userOutput.SelectedRoleIds.Any(x => x == roleToGrant.Id));
            Assert.True(userOutput.SelectedPermissions != null && userOutput.SelectedPermissions.Any(x => x == AppPermissions.Users.Create));

            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.UserRoles != null && updatedUser.UserRoles.Any(x => x.RoleId == roleToGrant.Id));
            Assert.Null(updatedUser.UserRoles.FirstOrDefault(x => x.RoleId == grantedRole.Id));
            Assert.True(updatedUser.UserClaims != null && updatedUser.UserClaims.Any(x => x.ClaimValue == AppPermissions.Users.Create));
            Assert.Null(updatedUser.UserClaims.FirstOrDefault(x => x.ClaimValue == AppPermissions.Users.Read));
        }

        [Fact]
        public async Task Should_Delete_Async()
        {
            var testUser = GetTestUser("test_user_for_user_app_service_delete", "test_user_for_user_app_service_delete@mail.com");
            await _dbContext.Users.AddAsync(testUser);
            await _dbContext.SaveChangesAsync();

            var userOutput = await _userAppService.DeleteAsync(testUser.Id);
            await _dbContext.SaveChangesAsync();

            var deletedUser = await GetTestDbContext().Users.FindAsync(userOutput.Id);

            Assert.Null(deletedUser);
        }
    }
}
