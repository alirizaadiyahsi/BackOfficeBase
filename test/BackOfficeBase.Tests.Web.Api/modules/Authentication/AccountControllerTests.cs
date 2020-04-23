using System.Threading;
using BackOfficeBase.Application.Authentication;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BackOfficeBase.Tests.Web.Api.modules.Authentication
{
    public class AccountControllerTests : WebApiTestBase
    {
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly User _testUser = GetTestUser();
        private readonly Role _testRole = GetTestRole();

        public AccountControllerTests()
        {
            AddUserToRole(_testUser, _testRole);
            var mockUserStore = SetupMockUserStore(_testUser);
            var userManager = new UserManager<User>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            _authenticationAppService = new AuthenticationAppService(userManager);
        }

        private static Mock<IUserStore<User>> SetupMockUserStore(User testUser)
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            mockUserStore.Setup(x => x.FindByNameAsync(testUser.UserName, CancellationToken.None)).ReturnsAsync(testUser);
            mockUserStore.As<IUserEmailStore<User>>().Setup(x => x.FindByEmailAsync(testUser.NormalizedEmail, CancellationToken.None))
                .ReturnsAsync(testUser);

            return mockUserStore;
        }
    }
}
