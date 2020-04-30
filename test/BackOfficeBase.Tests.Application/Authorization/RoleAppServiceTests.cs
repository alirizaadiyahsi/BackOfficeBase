using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class RoleAppServiceTests : AppServiceTestBase
    {
        private readonly IRoleAppService _roleAppService;
        private readonly TestBackOfficeBaseDbContext _dbContext;

        public RoleAppServiceTests()
        {
            _dbContext = GetTestDbContext();
            var mapper = GetNewHostServiceProvider().GetRequiredService<IMapper>();
            _roleAppService = new RoleAppService(_dbContext, mapper);
        }

        [Fact]
        public async Task Should_Get_Async()
        {
            var testRole = GetTestRole("test_role_for_role_app_service_get");

            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.RoleClaims.AddAsync(new RoleClaim
            {
                RoleId = testRole.Id,
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = AppPermissions.Roles.Read
            });
            await _dbContext.SaveChangesAsync();

            var roleOutput = await _roleAppService.GetAsync(testRole.Id);

            Assert.NotNull(roleOutput);
            Assert.True(roleOutput.SelectedPermissions != null && roleOutput.SelectedPermissions.Any());
        }

        [Fact]
        public async Task Should_Create_Async()
        {
            var testRole = GetTestRole("test_role_for_role_app_service_create");
            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.SaveChangesAsync();

            var createRoleInput = new CreateRoleInput
            {
                Name = "test_role_for_role_app_service_create",
                SelectedPermissions = new[] { AppPermissions.Roles.Read }
            };
            var roleOutput = await _roleAppService.CreateAsync(createRoleInput);
            await _dbContext.SaveChangesAsync();

            var insertedRole = await GetTestDbContext().Roles.FindAsync(roleOutput.Id);

            Assert.NotNull(roleOutput);
            Assert.True(roleOutput.SelectedPermissions!= null && roleOutput.SelectedPermissions.Any(x => x == AppPermissions.Roles.Read));

            Assert.NotNull(insertedRole);
            Assert.True(insertedRole.RoleClaims != null && insertedRole.RoleClaims.Any(x => x.ClaimValue == AppPermissions.Roles.Read));
        }

        [Fact]
        public async Task Should_Update()
        {
            var testRole = GetTestRole("test_role_for_role_app_service_update");

            await _dbContext.RoleClaims.AddAsync(new RoleClaim { Role = testRole, ClaimType = CustomClaimTypes.Permission, ClaimValue = AppPermissions.Roles.Read });
            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.SaveChangesAsync();

            var updateRoleInput = new UpdateRoleInput
            {
                Name = "test_role_for_role_app_service_update_updated",
                SelectedPermissions = new[] { AppPermissions.Roles.Create }
            };
            var roleOutput = _roleAppService.Update(updateRoleInput);
            _dbContext.SaveChanges();

            var updatedRole = await GetTestDbContext().Roles.FindAsync(roleOutput.Id);

            Assert.NotNull(roleOutput);
            Assert.True(roleOutput.SelectedPermissions != null && roleOutput.SelectedPermissions.Any(x => x == AppPermissions.Roles.Create));

            Assert.NotNull(updatedRole);
            Assert.True(updatedRole.RoleClaims != null && updatedRole.RoleClaims.Any(x => x.ClaimValue == AppPermissions.Roles.Create));
            Assert.Null(updatedRole.RoleClaims.FirstOrDefault(x => x.ClaimValue == AppPermissions.Roles.Read));
        }

        [Fact]
        public async Task Should_Delete_Async()
        {
            var testRole = GetTestRole("test_role_for_role_app_service_delete");
            await _dbContext.Roles.AddAsync(testRole);
            await _dbContext.SaveChangesAsync();

            var roleOutput = await _roleAppService.DeleteAsync(testRole.Id);
            await _dbContext.SaveChangesAsync();

            var deletedRole = await GetTestDbContext().Roles.FindAsync(roleOutput.Id);

            Assert.NotNull(roleOutput);
            Assert.Null(deletedRole);
        }
    }
}
