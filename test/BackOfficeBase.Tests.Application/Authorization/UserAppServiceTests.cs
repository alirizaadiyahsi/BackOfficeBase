using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users;
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
        private readonly User _testUser = GetTestUser("test_user_for_user_app_service", "test_user_for_user_app_service@mail.com");
        private readonly Role _testRole = GetTestRole("test_role_for_user_app_service");

        public UserAppServiceTests()
        {
            AddUserToRole(_testUser, _testRole);
            _dbContext = GetTestDbContext();

            var mapper = GetNewHostServiceProvider().GetRequiredService<IMapper>();
            _userAppService = new UserAppService(_dbContext, mapper);
        }

        [Fact]
        public async Task Should_Get_Async()
        {
            await _dbContext.Users.AddAsync(_testUser);
            await _dbContext.Roles.AddAsync(_testRole);
            await _dbContext.UserClaims.AddAsync(new UserClaim
            {
                UserId = _testUser.Id,
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = AppPermissions.Users.Read
            });

            await _dbContext.SaveChangesAsync();

            var userOutput = await _userAppService.GetAsync(_testUser.Id);

            Assert.NotNull(userOutput);
            Assert.True(userOutput.AllRoles != null && userOutput.AllRoles.Any());
            Assert.True(userOutput.SelectedPermissions != null && userOutput.SelectedPermissions.Length > 0);
            Assert.True(userOutput.SelectedRoleIds != null && userOutput.SelectedRoleIds.Length > 0);
        }
    }
}
