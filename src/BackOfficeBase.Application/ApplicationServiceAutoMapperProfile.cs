using System.Linq;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application
{
    public class ApplicationServiceAutoMapperProfile : Profile
    {
        public ApplicationServiceAutoMapperProfile()
        {
            CreateMap<User, UserOutput>()
                .ForMember(dest => dest.SelectedRoleIds,
                    opt => opt.MapFrom(src => src.UserRoles.Select(x => x.RoleId)))
                .ForMember(dest => dest.SelectedPermissions,
                    opt => opt.MapFrom(src => src.UserClaims.Where(uc => uc.ClaimType == CustomClaimTypes.Permission).Select(uc => uc.ClaimValue)));
            CreateMap<CreateUserInput, User>();
            CreateMap<UpdateUserInput, User>();
            CreateMap<UserOutput, User>();

            CreateMap<Role, RoleOutput>()
                .ForMember(dest => dest.SelectedPermissions,
                    opt => opt.MapFrom(src => src.RoleClaims.Where(uc => uc.ClaimType == CustomClaimTypes.Permission).Select(uc => uc.ClaimValue)));
            CreateMap<CreateRoleInput, Role>();
            CreateMap<UpdateRoleInput, Role>();

            CreateMap<OrganizationUnit, OrganizationUnitOutput>()
                .ForMember(dest => dest.SelectedRoles,
                    opt => opt.MapFrom(src => src.OrganizationUnitRoles.Select(x => new RoleOutput
                    {
                        Id = x.RoleId,
                        Name = x.Role.Name
                    })))
                .ForMember(dest => dest.SelectedUsers,
                    opt => opt.MapFrom(src => src.OrganizationUnitUsers.Select(x => new UserOutput
                    {
                        Id = x.UserId,
                        Email = x.User.Email,
                        FirstName = x.User.FirstName,
                        LastName = x.User.LastName,
                        Phone = x.User.Phone,
                        ProfileImageUrl = x.User.ProfileImageUrl,
                        UserName = x.User.UserName
                    })));
        }
    }
}
