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

            CreateMap<OrganizationUnit, OrganizationUnitOutput>();
        }
    }
}
