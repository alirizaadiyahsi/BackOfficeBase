using System.Threading.Tasks;
using System.Web;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Domain.AppConstants.Configuration;
using Microsoft.Extensions.Configuration;

namespace BackOfficeBase.Modules.Authentication.Helpers
{
    public class EmailSenderHelper
    {
        public static async Task SendRegistrationConfirmationMail(IEmailSender emailSender, IConfiguration configuration, UserOutput userOutput, string confirmationToken)
        {
            var callbackUrl = $"{configuration[AppConfig.App_ClientUrl]}/account/confirm-email?email={userOutput.Email}&token={HttpUtility.UrlEncode(confirmationToken)}";
            var subject = "Confirm your email.";
            var message = $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>{callbackUrl}</a>";
            await emailSender.SendEmailAsync(userOutput.Email, subject, message);
        }

        public static async Task SendForgotPasswordMail(IEmailSender emailSender, IConfiguration configuration, string resetToken, UserOutput userOutput)
        {
            var callbackUrl =
                $"{configuration[AppConfig.App_ClientUrl]}/account/reset-password?token={HttpUtility.UrlEncode(resetToken)}";
            var subject = "Reset your password.";
            var message = $"Please reset your password by clicking this link: <a href='{callbackUrl}'>{callbackUrl}</a>";

            await emailSender.SendEmailAsync(userOutput.Email, subject, message);
        }
    }
}
