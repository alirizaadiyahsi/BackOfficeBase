using AutoMapper;
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
        public UserAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
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

        public override Task<UserOutput> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
    }
}