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
    }
}