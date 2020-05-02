using System;
using System.Collections.Generic;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Utilities.Collections;

namespace BackOfficeBase.Application.OrganizationUnits.Dto
{
    public class UpdateOrganizationUnit : EntityDto
    {
        public Guid? ParentId { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<RoleOutput> SelectedRoles { get; set; }

        public PagedListResult<UserOutput> SelectedUsers { get; set; }

        public IEnumerable<RoleOutput> AllRoles { get; set; }

        public IEnumerable<UserOutput> AllUsers { get; set; }
    }
}