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

        public TestDataBuilderForUsers(BackOfficeBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            CreateTestUserForGet();
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

        //private void CreateUserForResetPassword()
        //{
        //    TestUserForResetPassword = new User
        //    {
        //        Id = Guid.NewGuid(),
        //        UserName = "TestUserName_" + Guid.NewGuid(),
        //        Email = "TestUserEmail_" + Guid.NewGuid(),
        //        PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
        //        EmailConfirmed = true,
        //        SecurityStamp = Guid.NewGuid().ToString()
        //    };
        //    TestUserForResetPassword.NormalizedEmail = TestUserForResetPassword.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
        //    TestUserForResetPassword.NormalizedUserName = TestUserForResetPassword.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

        //    _dbContext.Users.Add(TestUserForResetPassword);
        //    _dbContext.SaveChanges();
        //}
    }
}
