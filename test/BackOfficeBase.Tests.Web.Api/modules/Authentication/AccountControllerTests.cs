using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authentication;
using BackOfficeBase.Application.Authentication.Dto;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Domain.AppConsts.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Modules.Authentication.Controllers;
using BackOfficeBase.Web.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Web.Api.modules.Authentication
{
    public class AccountControllerTests : WebApiTestBase
    {
        private readonly AccountController _accountController;
        private readonly User _testUser = GetTestUser();
        private readonly Role _testRole = GetTestRole();

        public AccountControllerTests()
        {
            var jwtTokenConfiguration = Options.Create(new JwtTokenConfiguration());
            var mockAuthenticationService = SetupMockAuthenticationService();
            var mockConfiguration = SetupMockConfiguration();
            var mockEmailSender = new Mock<IEmailSender>();
            _accountController = new AccountController(mockAuthenticationService.Object, jwtTokenConfiguration, mockConfiguration.Object, mockEmailSender.Object);
        }

        [Fact]
        public async Task Should_Login()
        {
            var actionResult = await _accountController.Login(new LoginInput
            {
                Password = "123qwe",
                UserNameOrEmail = _testUser.UserName
            });

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var loginOutput = Assert.IsType<LoginOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(!string.IsNullOrEmpty(loginOutput.Token));
        }

        private static Mock<IConfiguration> SetupMockConfiguration()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x[AppConfig.App_ClientUrl]).Returns("http://localhost:8080");
            
            return mockConfiguration;
        }

        private Mock<IAuthenticationAppService> SetupMockAuthenticationService()
        {
            var mockAuthenticationService = new Mock<IAuthenticationAppService>();
            mockAuthenticationService.Setup(x => x.FindUserByUserNameOrEmailAsync(_testUser.UserName)).ReturnsAsync(_testUser);
            mockAuthenticationService.Setup(x => x.CheckPasswordAsync(_testUser, "123qwe")).ReturnsAsync(true);
            
            return mockAuthenticationService;
        }
    }
}
