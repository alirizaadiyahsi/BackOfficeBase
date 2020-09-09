using System;
using System.Globalization;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Tests.IntegrationTests.AuthorizationTests.UsersControllerTests.DataBuilder
{
    public class TestDataBuilderForUsers
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        public static User TestUserForGet;
        public static User TestUserForUpdate;
        public static User TestUserForDelete;

        public TestDataBuilderForUsers(BackOfficeBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            CreateTestUserForGet();
            CreateTestUserForUpdate();
            CreateTestUserForDelete();
        }

        private void CreateTestUserForGet()
        {
            TestUserForGet = new User
            {
                Id = Guid.NewGuid(),
                UserName = "GetUserName_" + Guid.NewGuid(),
                Email = "GetUserEmail_" + Guid.NewGuid(),
                PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            TestUserForGet.NormalizedEmail = TestUserForGet.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
            TestUserForGet.NormalizedUserName = TestUserForGet.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Users.Add(TestUserForGet);
            _dbContext.SaveChanges();
        }

        private void CreateTestUserForUpdate()
        {
            TestUserForUpdate = new User
            {
                Id = Guid.NewGuid(),
                UserName = "GetUserName_" + Guid.NewGuid(),
                Email = "GetUserEmail_" + Guid.NewGuid(),
                PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            TestUserForUpdate.NormalizedEmail = TestUserForUpdate.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
            TestUserForUpdate.NormalizedUserName = TestUserForUpdate.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Users.Add(TestUserForUpdate);
            _dbContext.SaveChanges();
        }

        private void CreateTestUserForDelete()
        {
            TestUserForDelete = new User
            {
                Id = Guid.NewGuid(),
                UserName = "GetUserName_" + Guid.NewGuid(),
                Email = "GetUserEmail_" + Guid.NewGuid(),
                PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            TestUserForDelete.NormalizedEmail = TestUserForDelete.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
            TestUserForDelete.NormalizedUserName = TestUserForDelete.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Users.Add(TestUserForDelete);
            _dbContext.SaveChanges();
        }
    }
}
