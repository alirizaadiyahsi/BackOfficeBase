using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Tests.Shared;

namespace BackOfficeBase.Tests.Application
{
    public class AppServiceTestBase : TestBase
    {
        public static void AddUserToRole(User testUser, Role testRole)
        {
            var testUserRole = new UserRole
            {
                User = testUser,
                Role = testRole
            };

            testUser.UserRoles.Add(testUserRole);
            testRole.UserRoles.Add(testUserRole);
        }

        public static Role GetTestRole(string roleName = "TestRoleName")
        {
            var testRole = new Role
            {
                Name = roleName
            };

            return testRole;
        }

        public static User GetTestUser(string userName = "TestUserName")
        {
            var testUser = new User
            {
                UserName = userName
            };

            return testUser;
        }
    }
}
