using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Services.Authorization;

namespace BackOfficeBase.Modules.Authentication.Helpers
{
    public class IdentityHelper
    {
        public static async Task<ClaimsIdentity> CreateClaimsIdentityAsync(IAuthorizationAppService authorizationAppService, string userNameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(userNameOrEmail) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var userToVerify = await authorizationAppService.FindUserByUserNameOrEmailAsync(userNameOrEmail);
            if (userToVerify == null)
            {
                return null;
            }

            if (!await authorizationAppService.CheckPasswordAsync(userToVerify, password)) return null;
            var claims = CreateUserClaims(userToVerify);
            claims = CreateRoleClaims(authorizationAppService, userToVerify.UserName, claims);

            return new ClaimsIdentity(new ClaimsIdentity(new GenericIdentity(userNameOrEmail, "Token"), claims));
        }

        private static List<Claim> CreateRoleClaims(IAuthorizationAppService authorizationAppService, string userName, List<Claim> claims)
        {
            var roles = authorizationAppService.GetRolesByUserName(userName);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims;
        }

        private static List<Claim> CreateUserClaims(UserOutput userToVerify)
        {
            var claims = new List<Claim>(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userToVerify.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", userToVerify.Id.ToString())
            });

            return claims;
        }
    }
}
