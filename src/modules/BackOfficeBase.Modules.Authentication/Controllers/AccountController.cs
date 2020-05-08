using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Application.Identity;
using BackOfficeBase.Application.Identity.Dto;
using BackOfficeBase.Modules.Authentication.Helpers;
using BackOfficeBase.Web.Core;
using BackOfficeBase.Web.Core.Configuration;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BackOfficeBase.Modules.Authentication.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly IIdentityAppService _identityAppService;
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AccountController(
            IIdentityAppService identityAppService,
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _identityAppService = identityAppService;
            _configuration = configuration;
            _emailSender = emailSender;
            _jwtTokenConfiguration = jwtTokenConfiguration.Value;
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<LoginOutput>> Login([FromBody]LoginInput input)
        {
            var userToVerify = await IdentityHelper.CreateClaimsIdentityAsync(_identityAppService, input.UserNameOrEmail, input.Password);
            if (userToVerify == null)
            {
                return BadRequest(UserFriendlyMessages.UserNameOrPasswordNotFound);
            }

            var token = new JwtSecurityToken
            (
                issuer: _jwtTokenConfiguration.Issuer,
                audience: _jwtTokenConfiguration.Audience,
                claims: userToVerify.Claims,
                expires: _jwtTokenConfiguration.EndDate,
                notBefore: _jwtTokenConfiguration.StartDate,
                signingCredentials: _jwtTokenConfiguration.SigningCredentials
            );

            return Ok(new LoginOutput
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> Register([FromBody]RegisterInput input)
        {
            var userOutput = await _identityAppService.FindUserByEmailAsync(input.Email);
            if (userOutput != null) return Conflict(UserFriendlyMessages.EmailAlreadyExist);

            userOutput = await _identityAppService.FindUserByUserNameAsync(input.UserName);
            if (userOutput != null) return Conflict(UserFriendlyMessages.UserNameAlreadyExist);

            var applicationUser = new UserOutput
            {
                UserName = input.UserName,
                Email = input.Email
            };

            var result = await _identityAppService.CreateUserAsync(applicationUser, input.Password);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            }

            var confirmationToken = await _identityAppService.GenerateEmailConfirmationTokenAsync(applicationUser);
            await EmailSenderHelper.SendRegistrationConfirmationMail(_emailSender, _configuration, applicationUser, confirmationToken);

            return Ok(new RegisterOutput { ResetToken = confirmationToken });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailInput input)
        {
            var userOutput = await _identityAppService.FindUserByEmailAsync(input.Email);
            if (userOutput == null) return NotFound(UserFriendlyMessages.EmailIsNotFound);

            var result = await _identityAppService.ConfirmEmailAsync(userOutput, input.Token);
            if (!result.Succeeded) return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Ok();
        }

        [HttpPost("/api/[action]")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordInput input)
        {
            if (input.NewPassword != input.PasswordRepeat)
            {
                return BadRequest(UserFriendlyMessages.PasswordsAreNotMatched);
            }

            var userOutput = await _identityAppService.FindUserByUserNameAsync(User.Identity.Name);
            var result = await _identityAppService.ChangePasswordAsync(userOutput, input.CurrentPassword, input.NewPassword);
            if (!result.Succeeded) return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Ok();
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<ForgotPasswordOutput>> ForgotPassword([FromBody] ForgotPasswordInput input)
        {
            var userOutput = await _identityAppService.FindUserByEmailAsync(input.Email);
            if (userOutput == null) return NotFound(UserFriendlyMessages.UserIsNotFound);

            var resetToken = await _identityAppService.GeneratePasswordResetTokenAsync(userOutput);
            await EmailSenderHelper.SendForgotPasswordMail(_emailSender, _configuration, resetToken, userOutput);

            return Ok(new ForgotPasswordOutput { ResetToken = resetToken });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var user = await _identityAppService.FindUserByUserNameOrEmailAsync(input.UserNameOrEmail);
            if (user == null) return NotFound(UserFriendlyMessages.UserIsNotFound);

            var result = await _identityAppService.ResetPasswordAsync(user, input.Token, input.Password);
            if (!result.Succeeded) return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Ok();
        }
    }
}