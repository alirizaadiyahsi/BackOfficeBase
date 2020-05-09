using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Application.Identity;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.Domain.AppConstants.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Modules.Authentication.Controllers;
using BackOfficeBase.Web.Core.Configuration;
using Microsoft.AspNetCore.Http;
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
        private readonly User _testUser;
        private readonly IOptions<JwtTokenConfiguration> _jwtTokenConfiguration = Options.Create(new JwtTokenConfiguration());
        private readonly Mock<IConfiguration> _configurationMock = SetupMockConfiguration();
        private readonly Mock<IEmailSender> _emailSenderMock = new Mock<IEmailSender>();

        public AccountControllerTests()
        {
            _testUser = GetTestUser();
        }

        [Fact]
        public async Task Should_Login_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByUserNameOrEmailAsync(It.IsAny<string>())).ReturnsAsync(_testUser);
            identityAppServiceMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            var accountController = new AccountController(identityAppServiceMock.Object, _jwtTokenConfiguration, _configurationMock.Object, _emailSenderMock.Object);
            var actionResult = await accountController.Login(new LoginInput
            {
                Password = "123qwe",
                UserNameOrEmail = _testUser.UserName
            });

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var loginOutput = Assert.IsType<LoginOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(!string.IsNullOrEmpty(loginOutput.Token));
        }

        [Fact]
        public async Task Should_Register_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            identityAppServiceMock.Setup(x => x.FindUserByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            identityAppServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            identityAppServiceMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync(Guid.NewGuid().ToString);

            var accountController = new AccountController(identityAppServiceMock.Object, _jwtTokenConfiguration, _configurationMock.Object, _emailSenderMock.Object);
            var actionResult = await accountController.Register(new RegisterInput
            {
                UserName = _testUser.UserName,
                Email = _testUser.Email,
                Password = "123qwe"
            });

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            var registerOutput = Assert.IsType<RegisterOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(!string.IsNullOrEmpty(registerOutput.ResetToken));
        }

        [Fact]
        public async Task Should_Confirm_Email_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(_testUser);
            identityAppServiceMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var accountController = new AccountController(identityAppServiceMock.Object, _jwtTokenConfiguration, _configurationMock.Object, _emailSenderMock.Object);
            var actionResult = await accountController.ConfirmEmail(new ConfirmEmailInput
            {
                Token = Guid.NewGuid().ToString(),
                Email = _testUser.Email
            });

            var okResult = Assert.IsType<OkResult>(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task Should_Change_Password_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByUserNameAsync(It.IsAny<string>())).ReturnsAsync(_testUser);
            identityAppServiceMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var accountController = new AccountController(
                identityAppServiceMock.Object,
                _jwtTokenConfiguration,
                _configurationMock.Object,
                _emailSenderMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.Name, _testUser.UserName) }, "TestAuthTypeName"))
                    }
                }
            };

            var actionResult = await accountController.ChangePassword(new ChangePasswordInput
            {
                CurrentPassword = "123qwe",
                NewPassword = "123qwe123qwe",
                PasswordRepeat = "123qwe123qwe"
            });

            var okResult = Assert.IsType<OkResult>(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task Should_Forgot_Password_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(_testUser);
            identityAppServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<User>())).ReturnsAsync(Guid.NewGuid().ToString);

            var accountController = new AccountController(identityAppServiceMock.Object, _jwtTokenConfiguration, _configurationMock.Object, _emailSenderMock.Object);
            var actionResult = await accountController.ForgotPassword(new ForgotPasswordInput
            {
                Email = _testUser.Email
            });

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var forgotPasswordOutput = Assert.IsType<ForgotPasswordOutput>(okObjectResult.Value);

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.True(!string.IsNullOrEmpty(forgotPasswordOutput.ResetToken));
        }

        [Fact]
        public async Task Should_Reset_Password_Async()
        {
            var identityAppServiceMock = new Mock<IIdentityAppService>();
            identityAppServiceMock.Setup(x => x.FindUserByUserNameOrEmailAsync(It.IsAny<string>())).ReturnsAsync(_testUser);
            identityAppServiceMock.Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var accountController = new AccountController(identityAppServiceMock.Object, _jwtTokenConfiguration, _configurationMock.Object, _emailSenderMock.Object);
            var actionResult = await accountController.ResetPassword(new ResetPasswordInput
            {
                Token = Guid.NewGuid().ToString(),
                Password = "123qwe",
                UserNameOrEmail = _testUser.UserName
            });

            var okResult = Assert.IsType<OkResult>(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        private static Mock<IConfiguration> SetupMockConfiguration()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x[AppConfig.App_ClientUrl]).Returns("http://localhost:8080");

            return configurationMock;
        }
    }
}
