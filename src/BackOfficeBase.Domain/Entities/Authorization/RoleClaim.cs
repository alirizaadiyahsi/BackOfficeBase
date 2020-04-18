using System;
using Microsoft.AspNetCore.Identity;

namespace BackOfficeBase.Domain.Entities.Authorization
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual Role Role { get; set; }
    }
}