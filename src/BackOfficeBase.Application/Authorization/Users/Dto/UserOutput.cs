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

        // TODO: Map following props in automapper profile
        public Guid[] SelectedRoleIds { get; set; }

        public Guid[] SelectedClaimIds { get; set; }

        public List<RoleOutput> AllRoles { get; set; }
    }
}