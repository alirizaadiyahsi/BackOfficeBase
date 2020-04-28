using System;
using System.Linq;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application
{
    public class ApplicationServiceAutoMapperProfile : Profile
    {
        public ApplicationServiceAutoMapperProfile()
        {
            CreateMap<User, UserOutput>()
                .ForMember(dest => dest.SelectedRoleIds,
                    opt => opt.MapFrom((entity, dto, _, context) =>
                    {
                        return entity.UserRoles.Where(ur => ur.UserId == Guid.Parse(context.Items["UserId"].ToString())).Select(ur => ur.RoleId);
                    }))
                .ForMember(dest => dest.SelectedPermissions,
                    opt => opt.MapFrom((entity, dto, _, context) =>
                    {
                        return entity.UserClaims.Where(uc => uc.UserId == Guid.Parse(context.Items["UserId"].ToString()) && uc.ClaimType == CustomClaimTypes.Permission).Select(uc => uc.ClaimValue);
                    }));

            CreateMap<User, UserOutput>()
                .ForMember(dest => dest.SelectedRoleIds,
                    opt => opt.MapFrom(src => src.UserRoles.Select(x => x.RoleId)))
                .ForMember(dest => dest.SelectedPermissions,
                    opt => opt.MapFrom(src => src.UserClaims.Where(uc => uc.ClaimType == CustomClaimTypes.Permission).Select(uc => uc.ClaimValue)));

            CreateMap<CreateUserInput, User>();
            CreateMap<UpdateUserInput, User>();

            CreateMap<Role, RoleOutput>();
        }
    }
}
