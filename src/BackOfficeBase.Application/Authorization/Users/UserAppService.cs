using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.DataAccess;
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

        public override async Task<UserOutput> CreateAsync(CreateUserInput input)
        {
            var userOutput = await base.CreateAsync(input);

            AddRolesToUser(input.SelectedRoleIds, userOutput.Id);
            AddPermissionsToUser(input.SelectedPermissions, userOutput.Id);

            SetSelectedNavigationProperties(input.SelectedRoleIds, input.SelectedPermissions, userOutput);

            return userOutput;
        }

        public override UserOutput Update(UpdateUserInput input)
        {
            var userOutput  = base.Update(input);

            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(x => x.UserId == input.Id));
            _dbContext.UserClaims.RemoveRange(_dbContext.UserClaims.Where(x => x.UserId == input.Id && x.ClaimType == CustomClaimTypes.Permission));
            _dbContext.SaveChanges();

            AddRolesToUser(input.SelectedRoleIds, userOutput.Id);
            AddPermissionsToUser(input.SelectedPermissions, userOutput.Id);

            SetSelectedNavigationProperties(input.SelectedRoleIds, input.SelectedPermissions, userOutput);

            return userOutput;
        }

        private static void SetSelectedNavigationProperties(IEnumerable<Guid> selectedRoleIds, IEnumerable<string> selectedPermissions, UserOutput userOutput)
        {
            userOutput.SelectedRoleIds = selectedRoleIds;
            userOutput.SelectedPermissions = selectedPermissions;
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
    }
}