﻿using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Roles
{
    public interface IRoleAppService: ICrudAppService<Role, RoleOutput, RoleListOutput, CreateRoleInput, UpdateRoleInput>
    {
    }
}
