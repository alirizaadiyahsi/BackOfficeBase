using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authentication;
using BackOfficeBase.Application.Authentication.Dto;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Domain.AppConstants;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Modules.Authentication.Helpers;
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
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AccountController(
            IAuthenticationAppService authenticationAppService,
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _authenticationAppService = authenticationAppService;
            _configuration = configuration;
            _emailSender = emailSender;
            _jwtTokenConfiguration = jwtTokenConfiguration.Value;
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<LoginOutput>> Login([FromBody]LoginInput input)
        {
            var userToVerify = await IdentityHelper.CreateClaimsIdentityAsync(_authenticationAppService, input.UserNameOrEmail, input.Password);
            if (userToVerify == null)
            {
                return NotFound(UserFriendlyMessages.UserNameOrPasswordNotFound);
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
            var user = await _authenticationAppService.FindUserByEmailAsync(input.Email);
            if (user != null) return Conflict(UserFriendlyMessages.EmailAlreadyExist);

            user = await _authenticationAppService.FindUserByUserNameAsync(input.UserName);
            if (user != null) return Conflict(UserFriendlyMessages.UserNameAlreadyExist);

            var applicationUser = new User
            {
                UserName = input.UserName,
                Email = input.Email
            };

            var result = await _authenticationAppService.CreateUserAsync(applicationUser, input.Password);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            }

            var confirmationToken = await _authenticationAppService.GenerateEmailConfirmationTokenAsync(applicationUser);
            await EmailSenderHelper.SendRegistrationConfirmationMail(_emailSender, _configuration, applicationUser, confirmationToken);

            return Ok(new RegisterOutput { ResetToken = confirmationToken });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailInput input)
        {
            var user = await _authenticationAppService.FindUserByEmailAsync(input.Email);
            if (user == null) return NotFound(UserFriendlyMessages.EmailIsNotFound);

            var result = await _authenticationAppService.ConfirmEmailAsync(user, input.Token);
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

            var user = await _authenticationAppService.FindUserByUserNameAsync(User.Identity.Name);
            var result = await _authenticationAppService.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            if (!result.Succeeded) return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Ok();
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult<ForgotPasswordOutput>> ForgotPassword([FromBody] ForgotPasswordInput input)
        {
            var user = await _authenticationAppService.FindUserByEmailAsync(input.Email);
            if (user == null) return NotFound(UserFriendlyMessages.UserIsNotFound);

            var resetToken = await _authenticationAppService.GeneratePasswordResetTokenAsync(user);
            await EmailSenderHelper.SendForgotPasswordMail(_emailSender, _configuration, resetToken, user);

            return Ok(new ForgotPasswordOutput { ResetToken = resetToken });
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var user = await _authenticationAppService.FindUserByUserNameOrEmailAsync(input.UserNameOrEmail);
            if (user == null) return NotFound(UserFriendlyMessages.UserIsNotFound);

            var result = await _authenticationAppService.ResetPasswordAsync(user, input.Token, input.Password);
            if (!result.Succeeded) return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return Ok();
        }
    }
}