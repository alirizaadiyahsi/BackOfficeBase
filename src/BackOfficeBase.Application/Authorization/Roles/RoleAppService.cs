using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Roles
{
    // TODO: Write test
    public class RoleAppService :CrudAppService<Role, RoleOutput, RoleListOutput, CreateRoleInput, UpdateRoleInput>, IRoleAppService
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        private readonly IMapper _mapper;

        public RoleAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}