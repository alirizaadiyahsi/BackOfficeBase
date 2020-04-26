using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authentication;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Modules.Authentication.Helpers
{
    public class IdentityHelper
    {
        public static async Task<ClaimsIdentity> CreateClaimsIdentityAsync(IAuthenticationAppService authenticationAppService, string userNameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(userNameOrEmail) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var userToVerify = await authenticationAppService.FindUserByUserNameOrEmailAsync(userNameOrEmail);
            if (userToVerify == null)
            {
                return null;
            }

            if (!await authenticationAppService.CheckPasswordAsync(userToVerify, password)) return null;
            var claims = CreateUserClaims(userToVerify);
            claims = CreateRoleClaims(userToVerify, claims);

            return new ClaimsIdentity(new ClaimsIdentity(new GenericIdentity(userNameOrEmail, "Token"), claims));
        }

        private static List<Claim> CreateUserClaims(User userToVerify)
        {
            var claims = new List<Claim>(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userToVerify.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", userToVerify.Id.ToString())
            });

            return claims;
        }

        private static List<Claim> CreateRoleClaims(User userToVerify, List<Claim> claims)
        {
            // add roles to roleClaim to use build-in User.IsInRole method
            var roleNames = userToVerify.UserRoles.Select(ur => ur.Role.Name);
            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            return claims;
        }
    }
}
