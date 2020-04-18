using System;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Domain.Entities.Authorization
{
    public class UserLogin : IdentityUserLogin<Guid>
    {
        public virtual User User { get; set; }
    }
}