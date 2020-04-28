using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Users
{
    // TODO: Write test
    public class UserAppService : CrudAppService<User, UserOutput, UserListOutput, CreateUserInput, UpdateUserInput>, IUserAppService
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        private readonly IMapper _mapper;
        public UserAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public override async Task<UserOutput> GetAsync(Guid id)
        {
            var userOutput = await base.GetAsync(id);
            userOutput.AllRoles = _mapper.Map<IEnumerable<RoleOutput>>(_dbContext.Roles);

            return userOutput;
        }

        public override async Task<AppServiceResult<UserOutput>> CreateAsync(CreateUserInput input)
        {
            var appServiceResult = await base.CreateAsync(input);
            if (!appServiceResult.Success) return appServiceResult;

            foreach (var selectedRoleId in input.SelectedRoleIds)
            {
                await _dbContext.UserRoles.AddAsync(new UserRole(appServiceResult.Data.Id, selectedRoleId));
            }

            return appServiceResult;
        }
    }
}