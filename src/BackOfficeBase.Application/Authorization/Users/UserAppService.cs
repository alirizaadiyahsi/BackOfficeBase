using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConsts.Authorization;
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
            var userOutput = await base.GetAsync(id, opts =>
            {
                opts.Items["UserId"] = id;
            });
            userOutput.AllRoles = _mapper.Map<IEnumerable<RoleOutput>>(_dbContext.Roles);
            userOutput.AllPermissions = AppPermissions.GetAll();

            return userOutput;
        }

        public override async Task<AppServiceResult<UserOutput>> CreateAsync(CreateUserInput input)
        {
            var appServiceResult = await base.CreateAsync(input);
            if (!appServiceResult.Success) return appServiceResult;

            AddRolesToUser(input.SelectedRoleIds, appServiceResult.Data.Id);
            AddClaimsToUser(input.SelectedPermissions, appServiceResult.Data.Id);

            return appServiceResult;
        }

        public override AppServiceResult<UserOutput> Update(UpdateUserInput input)
        {
            var appServiceResult = base.Update(input);
            if (!appServiceResult.Success) return appServiceResult;

            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(x => x.UserId == input.Id));
            _dbContext.UserClaims.RemoveRange(_dbContext.UserClaims.Where(x => x.UserId == input.Id && x.ClaimType == CustomClaimTypes.Permission));
            _dbContext.SaveChanges();

            AddRolesToUser(input.SelectedRoleIds, appServiceResult.Data.Id);
            AddClaimsToUser(input.SelectedPermissions, appServiceResult.Data.Id);

            return appServiceResult;
        }

        private void AddClaimsToUser(IEnumerable<string> selectedPermissions, Guid userId)
        {
            foreach (var permission in selectedPermissions)
            {
                _dbContext.UserClaims.Add(new UserClaim
                {
                    UserId = userId,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = permission
                });
            }
        }

        private void AddRolesToUser(IEnumerable<Guid> selectedRoleIds, Guid userId)
        {
            foreach (var selectedRoleId in selectedRoleIds)
            {
                _dbContext.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = selectedRoleId
                });
            }
        }
    }
}