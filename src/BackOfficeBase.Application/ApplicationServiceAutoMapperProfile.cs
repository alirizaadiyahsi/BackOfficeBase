using System;
using System.Linq;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application
{
    public class ApplicationServiceAutoMapperProfile : Profile
    {
        public ApplicationServiceAutoMapperProfile()
        {
            // user types
            CreateMap<User, UserOutput>()
                .ForMember(dest => dest.SelectedRoleIds,
                    opt => opt.MapFrom((entity, dto, _, context) =>
                    {
                        return entity.UserRoles.Where(ur => ur.UserId == Guid.Parse(context.Items["UserId"].ToString())).Select(ur => ur.RoleId);
                    }))
                .ForMember(dest => dest.SelectedClaimIds,
                    opt => opt.MapFrom((entity, dto, _, context) =>
                    {
                        return entity.UserClaims.Where(uc => uc.UserId == Guid.Parse(context.Items["UserId"].ToString())).Select(uc => uc.UserId);
                    }));
        }
    }
}
