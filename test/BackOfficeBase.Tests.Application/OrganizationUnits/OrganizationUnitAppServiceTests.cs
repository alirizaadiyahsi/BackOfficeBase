using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.OrganizationUnits;
using BackOfficeBase.Application.OrganizationUnits.Dto;
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
                Name = "test organization unit to add users"
            };

            var testUser1 = GetTestUser("test_user_for_add_or_remove_ou1");
            var testUser2 = GetTestUser("test_user_for_add_or_remove_ou2");

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.Users.AddAsync(testUser1);
            await _dbContext.Users.AddAsync(testUser2);
            await _dbContext.SaveChangesAsync();

            await _organizationUnitAppService.AddUsersToOrganizationUnitAsync(new AddOrRemoveUsersToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedUserIds = new List<Guid>
                {
                    testUser1.Id,
                    testUser2.Id
                }
            });
            await _dbContext.SaveChangesAsync();

            var organizationUnitUsers =
                _dbContext.OrganizationUnitUsers.Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitUsers);
            Assert.Equal(2, organizationUnitUsers.Count());
        }

        [Fact]
        public async Task Should_Remove_Users_To_OrganizationUnit_Async()
        {
            var testOrganizationUnit = new OrganizationUnit
            {
                Name = "test organization unit to remove users"
            };

            var testUser1 = GetTestUser("test_user_for_add_or_remove_ou1");
            var testUser2 = GetTestUser("test_user_for_add_or_remove_ou2");

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.Users.AddAsync(testUser1);
            await _dbContext.Users.AddAsync(testUser2);
            await _dbContext.SaveChangesAsync();

            await AddUsersToOrganizationUnit(testOrganizationUnit, testUser1, testUser2);

            _organizationUnitAppService.RemoveUsersFromOrganizationUnit(new AddOrRemoveUsersToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedUserIds = new List<Guid> { testUser2.Id }
            });
            await _dbContext.SaveChangesAsync();

            var organizationUnitUsers =
                _dbContext.OrganizationUnitUsers.Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitUsers);
            Assert.Equal(1, organizationUnitUsers.Count());
        }

        [Fact]
        public async Task Should_Add_Roles_To_OrganizationUnit_Async()
        {
            var testOrganizationUnit = new OrganizationUnit
            {
                Name = "test organization unit to add role"
            };

            var testRole1 = GetTestUser("test_role_for_add_or_remove_ou1");
            var testRole2 = GetTestUser("test_role_for_add_or_remove_ou2");

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.Users.AddAsync(testRole1);
            await _dbContext.Users.AddAsync(testRole2);
            await _dbContext.SaveChangesAsync();

            await _organizationUnitAppService.AddRolesToOrganizationUnitAsync(new AddOrRemoveRolesToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedRoleIds = new List<Guid>
                {
                    testRole1.Id,
                    testRole1.Id
                }
            });
            await _dbContext.SaveChangesAsync();

            var organizationUnitRoles =
                _dbContext.OrganizationUnitRoles.Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitRoles);
            Assert.Equal(2, organizationUnitRoles.Count());
        }

        [Fact]
        public async Task Should_Remove_Roles_To_OrganizationUnit_Async()
        {
            var testOrganizationUnit = new OrganizationUnit
            {
                Name = "test organization unit to remove role"
            };

            var testRole1 = GetTestUser("test_role_for_add_or_remove_ou1");
            var testRole2 = GetTestUser("test_role_for_add_or_remove_ou2");

            await _dbContext.OrganizationUnits.AddAsync(testOrganizationUnit);
            await _dbContext.Users.AddAsync(testRole1);
            await _dbContext.Users.AddAsync(testRole2);
            await _dbContext.SaveChangesAsync();

            await AddRolesToOrganizationUnit(testOrganizationUnit, testRole1, testRole2);

            var organizationUnitRoles =
                _dbContext.OrganizationUnitRoles.Where(x => x.OrganizationUnitId == testOrganizationUnit.Id);

            Assert.NotNull(organizationUnitRoles);
            Assert.Equal(1, organizationUnitRoles.Count());
        }

        private async Task AddRolesToOrganizationUnit(OrganizationUnit testOrganizationUnit, User testRole1, User testRole2)
        {
            await _dbContext.OrganizationUnitRoles.AddAsync(new OrganizationUnitRole
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                RoleId = testRole1.Id
            });
            await _dbContext.OrganizationUnitRoles.AddAsync(new OrganizationUnitRole
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                RoleId = testRole2.Id
            });
            await _dbContext.SaveChangesAsync();

            _organizationUnitAppService.RemoveRolesFromOrganizationUnit(new AddOrRemoveRolesToOrganizationUnitInput
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                SelectedRoleIds = new List<Guid> { testRole2.Id }
            });
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddUsersToOrganizationUnit(OrganizationUnit testOrganizationUnit, User testUser1, User testUser2)
        {
            await _dbContext.OrganizationUnitUsers.AddAsync(new OrganizationUnitUser
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                UserId = testUser1.Id
            });
            await _dbContext.OrganizationUnitUsers.AddAsync(new OrganizationUnitUser
            {
                OrganizationUnitId = testOrganizationUnit.Id,
                UserId = testUser2.Id
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
