using System;
using System.Globalization;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Tests.IntegrationTests.DataBuilder
{
    public class TestUsersDataBuilder
    {
        private readonly BackOfficeBaseDbContext _dbContext;

        public static User TestUserForChangePassword;
        public static User TestUserForResetPassword;

        public TestUsersDataBuilder(BackOfficeBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedData()
        {
            CreateUserForChangePassword();
            CreateUserForResetPassword();
        }

        private void CreateUserForChangePassword()
        {
            TestUserForChangePassword = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUserName_" + Guid.NewGuid(),
                Email = "TestUserEmail_" + Guid.NewGuid(),
                PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            TestUserForChangePassword.NormalizedEmail = TestUserForChangePassword.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
            TestUserForChangePassword.NormalizedUserName = TestUserForChangePassword.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Users.Add(TestUserForChangePassword);
            _dbContext.SaveChanges();
        }

        private void CreateUserForResetPassword()
        {
            TestUserForResetPassword = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUserName_" + Guid.NewGuid(),
                Email = "TestUserEmail_" + Guid.NewGuid(),
                PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            TestUserForResetPassword.NormalizedEmail = TestUserForResetPassword.Email.ToUpper(CultureInfo.GetCultureInfo("en-US"));
            TestUserForResetPassword.NormalizedUserName = TestUserForResetPassword.UserName.ToUpper(CultureInfo.GetCultureInfo("en-US"));

            _dbContext.Users.Add(TestUserForResetPassword);
            _dbContext.SaveChanges();
        }
    }
}
