using System.Threading.Tasks;
using System.Web;
using BackOfficeBase.Application.Email;
using BackOfficeBase.Domain.AppConstants.Configuration;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.Extensions.Configuration;

namespace BackOfficeBase.Modules.Authentication.Helpers
{
    public class EmailSenderHelper
    {
        public static async Task SendRegistrationConfirmationMail(IEmailSender emailSender, IConfiguration configuration, User applicationUser, string confirmationToken)
        {
            var callbackUrl = $"{configuration[AppConfig.App_ClientUrl]}/account/confirm-email?email={applicationUser.Email}&token={HttpUtility.UrlEncode(confirmationToken)}";
            var subject = "Confirm your email.";
            var message = $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>{callbackUrl}</a>";
            await emailSender.SendEmailAsync(applicationUser.Email, subject, message);
        }

        public static async Task SendForgotPasswordMail(IEmailSender emailSender, IConfiguration configuration, string resetToken, User user)
        {
            var callbackUrl =
                $"{configuration[AppConfig.App_ClientUrl]}/account/reset-password?token={HttpUtility.UrlEncode(resetToken)}";
            var subject = "Reset your password.";
            var message = $"Please reset your password by clicking this link: <a href='{callbackUrl}'>{callbackUrl}</a>";

            await emailSender.SendEmailAsync(user.Email, subject, message);
        }
    }
}
