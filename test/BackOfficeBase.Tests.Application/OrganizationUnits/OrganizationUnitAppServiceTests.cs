using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.OrganizationUnits;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Domain.Entities.OrganizationUnits;
using BackOfficeBase.Tests.Shared.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BackOfficeBase.Tests.Application.OrganizationUnits
{
    public class OrganizationUnitAppServiceTests : AppServiceTestBase
    {
        private readonly IOrganizationUnitAppService _organizationUnitAppService;
        private readonly TestBackOfficeBaseDbContext _dbContext;

        public OrganizationUnitAppServiceTests()
        {
            _dbContext = GetDefaultTestDbContext();
            var mapper = GetNewHostServiceProvider().GetRequiredService<IMapper>();
            _organizationUnitAppService = new OrganizationUnitAppService(_dbContext, mapper);
        }

        [Fact]
        public async Task Should_Add_Users_To_OrganizationUnit_Async()
        {
            var testOrganizationUnit = new OrganizationUnit
            {
                Code = "0000",
                Name = "test organization unit"
            };

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.SaveChangesAsync();

            var organizationUnitOutput = await _organizationUnitAppService.AddUsersToOrganizationUnitAsync(new AddOrRemoveUsersToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedUserIds = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            });
            await _dbContext.SaveChangesAsync();

            var selectedOrganizationUnitUsers = _dbContext.OrganizationUnitUsers
                    .Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitOutput);
            Assert.Equal(2, selectedOrganizationUnitUsers.Count());
        }

        [Fact]
        public async Task Should_Remove_Users_To_OrganizationUnit_Async()
        {
            var testOrganizationUnitId = Guid.NewGuid();
            var testUserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            foreach (var testUserId in testUserIds)
            {
                await _dbContext.OrganizationUnitUsers.AddAsync(new OrganizationUnitUser
                {
                    OrganizationUnitId = testOrganizationUnitId,
                    UserId = testUserId
                });
            }
            await _dbContext.SaveChangesAsync();

            await _organizationUnitAppService.RemoveUsersFromOrganizationUnitAsync(new AddOrRemoveUsersToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnitId,
                SelectedUserIds = testUserIds.GetRange(0, 1)
            });
            await _dbContext.SaveChangesAsync();

            var selectedOrganizationUnitUsers = _dbContext.OrganizationUnitUsers
                .Where(x => x.OrganizationUnitId == testOrganizationUnitId);

            Assert.Equal(1, selectedOrganizationUnitUsers.Count());
        }

        [Fact]
        public async Task Should_Add_Roles_To_OrganizationUnit_Async()
        {
            var testOrganizationUnit = new OrganizationUnit
            {
                Code = "0000",
                Name = "test organization unit"
            };

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.SaveChangesAsync();

            var organizationUnitOutput = await _organizationUnitAppService.AddRolesToOrganizationUnitAsync(new AddOrRemoveRolesToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedRoleIds = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            });
            await _dbContext.SaveChangesAsync();

            var selectedOrganizationUnitRoles = _dbContext.OrganizationUnitRoles
                .Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitOutput);
            Assert.Equal(2, selectedOrganizationUnitRoles.Count());
        }

        [Fact]
        public async Task Should_Remove_Roles_To_OrganizationUnit_Async()
        {
            var testOrganizationUnitId = Guid.NewGuid();
            var testRoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            foreach (var testRoleId in testRoleIds)
            {
                await _dbContext.OrganizationUnitRoles.AddAsync(new OrganizationUnitRole
                {
                    OrganizationUnitId = testOrganizationUnitId,
                    RoleId = testRoleId
                });
            }
            await _dbContext.SaveChangesAsync();

            await _organizationUnitAppService.RemoveRolesFromOrganizationUnitAsync(new AddOrRemoveRolesToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnitId,
                SelectedRoleIds = testRoleIds.GetRange(0, 1)
            });
            await _dbContext.SaveChangesAsync();

            var selectedOrganizationUnitRoles = _dbContext.OrganizationUnitRoles
                .Where(x => x.OrganizationUnitId == testOrganizationUnitId);

            Assert.Equal(1, selectedOrganizationUnitRoles.Count());
        }
    }
}
