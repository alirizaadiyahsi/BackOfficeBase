using System;
using System.Globalization;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.RolesControllerTests.DataBuilder
{
    public class TestDataBuilderForRoles
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        public static Role TestRoleForGet;
        public static Role TestRoleForUpdate;
        public static Role TestRoleForDelete;

        public TestDataBuilderForRoles(BackOfficeBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            CreateTestRoleForGet();
            CreateTestRoleForUpdate();
            CreateTestRoleForDelete();
        }

        private void CreateTestRoleForGet()
        {
            TestRoleForGet = new Role
            {
                Id = Guid.NewGuid(),
                Name = "GetRoleName_" + Guid.NewGuid()
            };
            TestRoleForGet.NormalizedName = TestRoleForGet.Name.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Roles.Add(TestRoleForGet);
            _dbContext.SaveChanges();
        }

        private void CreateTestRoleForUpdate()
        {
            TestRoleForUpdate = new Role
            {
                Id = Guid.NewGuid(),
                Name = "UpdateRoleName_" + Guid.NewGuid()
            };
            TestRoleForUpdate.NormalizedName = TestRoleForUpdate.Name.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Roles.Add(TestRoleForUpdate);
            _dbContext.SaveChanges();
        }

        private void CreateTestRoleForDelete()
        {
            TestRoleForDelete = new Role
            {
                Id = Guid.NewGuid(),
                Name = "DeleteRoleName_" + Guid.NewGuid()
            };
            TestRoleForDelete.NormalizedName = TestRoleForDelete.Name.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Roles.Add(TestRoleForDelete);
            _dbContext.SaveChanges();
        }
    }
}
