using System;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Domain.Entities.Authorization
{
    public class UserToken : IdentityUserToken<Guid>
    {
        public virtual User User { get; set; }
    }
}