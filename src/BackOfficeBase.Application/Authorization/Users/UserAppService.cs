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
using BackOfficeBase.Domain.AppConstants;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Users
{
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
            userOutput.AllPermissions = AppPermissions.GetAll();

            return userOutput;
        }

        public override async Task<AppServiceResult<UserOutput>> CreateAsync(CreateUserInput input)
        {
            var appServiceResult = CheckIfUserExist(input.UserName, input.Email);
            if (!appServiceResult.Success) return appServiceResult;

            appServiceResult = await base.CreateAsync(input);
            if (!appServiceResult.Success) return appServiceResult;

            AddRolesToUser(input.SelectedRoleIds, appServiceResult.Data.Id);
            AddPermissionsToUser(input.SelectedPermissions, appServiceResult.Data.Id);

            SetSelectedNavigationProperties(input.SelectedRoleIds, input.SelectedPermissions, appServiceResult);

            return appServiceResult;
        }

        public override AppServiceResult<UserOutput> Update(UpdateUserInput input)
        {
            var appServiceResult = CheckIfUserExist(input.UserName, input.Email);
            if (!appServiceResult.Success) return appServiceResult;

            appServiceResult = base.Update(input);
            if (!appServiceResult.Success) return appServiceResult;

            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(x => x.UserId == input.Id));
            _dbContext.UserClaims.RemoveRange(_dbContext.UserClaims.Where(x => x.UserId == input.Id && x.ClaimType == CustomClaimTypes.Permission));
            _dbContext.SaveChanges();

            AddRolesToUser(input.SelectedRoleIds, appServiceResult.Data.Id);
            AddPermissionsToUser(input.SelectedPermissions, appServiceResult.Data.Id);

            SetSelectedNavigationProperties(input.SelectedRoleIds, input.SelectedPermissions, appServiceResult);

            return appServiceResult;
        }

        private static void SetSelectedNavigationProperties(IEnumerable<Guid> selectedRoleIds, IEnumerable<string> selectedPermissions, AppServiceResult<UserOutput> appServiceResult)
        {
            appServiceResult.Data.SelectedRoleIds = selectedRoleIds;
            appServiceResult.Data.SelectedPermissions = selectedPermissions;
        }

        private void AddPermissionsToUser(IEnumerable<string> selectedPermissions, Guid userId)
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

        private AppServiceResult<UserOutput> CheckIfUserExist(string userName, string email)
        {
            var isUserExist = _dbContext.Users.Any(x => x.UserName == userName);
            if (isUserExist)
            {
                return AppServiceResult<UserOutput>.Failed(new List<string>
                {
                    new string(UserFriendlyMessages.UserNameAlreadyExist)
                });
            }

            isUserExist = _dbContext.Users.Any(x => x.Email == email);
            if (isUserExist)
            {
                return AppServiceResult<UserOutput>.Failed(new List<string>
                {
                    new string(UserFriendlyMessages.EmailAlreadyExist)
                });
            }

            return AppServiceResult<UserOutput>.Succeed(null);
        }
    }
}