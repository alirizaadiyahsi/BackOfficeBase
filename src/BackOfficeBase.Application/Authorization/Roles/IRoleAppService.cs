using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Shared;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Roles
{
    public interface IRoleAppService: ICrudAppService<Role, RoleOutput, RoleListOutput, CreateRoleInput, UpdateRoleInput>
    {
    }
}
