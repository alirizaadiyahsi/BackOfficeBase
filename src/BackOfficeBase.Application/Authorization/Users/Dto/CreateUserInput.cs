using System;
using System.Collections.Generic;
using BackOfficeBase.Application.Shared.Dto;

namespace BackOfficeBase.Application.Authorization.Users.Dto
{
    public class CreateUserInput : EntityDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string ProfileImageUrl { get; set; }

        public IEnumerable<Guid> SelectedRoleIds { get; set; }

        public IEnumerable<string> SelectedPermissions { get; set; }
    }
}