using System;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Domain.Entities.Authorization
{
    public class UserClaim : IdentityUserClaim<Guid>
    {
        public virtual User User { get; set; }
    }
}