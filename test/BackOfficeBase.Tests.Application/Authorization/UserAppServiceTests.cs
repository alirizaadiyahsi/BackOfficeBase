using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConsts.Authorization;
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

            var userOutput = await _userAppService.GetAsync(testRole.Id);

            Assert.NotNull(userOutput);
            Assert.True(userOutput.AllRoles != null && userOutput.AllRoles.Any());
            Assert.True(userOutput.SelectedPermissions != null && userOutput.SelectedPermissions.Length > 0);
            Assert.True(userOutput.SelectedRoleIds != null && userOutput.SelectedRoleIds.Length > 0);
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

            var insertedUser = await GetTestDbContext().Users.FindAsync(userOutput.Data.Id);

            Assert.NotNull(insertedUser);
            Assert.True(insertedUser.UserRoles != null && insertedUser.UserRoles.Any());
            Assert.True(insertedUser.UserClaims != null && insertedUser.UserClaims.Any());
        }
    }
}
