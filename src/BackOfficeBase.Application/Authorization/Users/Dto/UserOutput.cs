using System;
using System.Collections.Generic;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Application.Authorization.Users.Dto
{
    public class UserOutput : EntityDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string ProfileImageUrl { get; set; }

        public Guid[] SelectedRoleIds { get; set; }

        public int[] SelectedClaimIds { get; set; }

        public IEnumerable<RoleOutput> AllRoles { get; set; }
    }
}