using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Roles.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.AppConstants.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Application.Authorization.Roles
{
    public class RoleAppService : CrudAppService<Role, RoleOutput, RoleListOutput, CreateRoleInput, UpdateRoleInput>, IRoleAppService
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        private readonly IMapper _mapper;

        public RoleAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public override async Task<RoleOutput> GetAsync(Guid id)
        {
            var roleOutput = await base.GetAsync(id);
            roleOutput.AllPermissions = AppPermissions.GetAll();

            return roleOutput;
        }

        public override async Task<RoleOutput> CreateAsync(CreateRoleInput input)
        {
            var roleOutput = await base.CreateAsync(input);

            AddPermissionsToRole(input.SelectedPermissions, roleOutput.Id);
            SetSelectedNavigationProperties(input.SelectedPermissions, roleOutput);

            return roleOutput;
        }

        public override RoleOutput Update(UpdateRoleInput input)
        {
            var roleOutput = base.Update(input);

            _dbContext.RoleClaims.RemoveRange(_dbContext.RoleClaims.Where(x => x.RoleId == input.Id && x.ClaimType == CustomClaimTypes.Permission));
            _dbContext.SaveChanges();

            AddPermissionsToRole(input.SelectedPermissions, roleOutput.Id);
            SetSelectedNavigationProperties(input.SelectedPermissions, roleOutput);

            return roleOutput;
        }

        // TODO: Write test
        public async Task<RoleOutput> FindByNameAsync(string name)
        {
            var roleOutput = _mapper.Map<RoleOutput>(await _dbContext.Roles.FirstAsync(x => x.Name == name));
            roleOutput.AllPermissions = AppPermissions.GetAll();

            return roleOutput;
        }

        private static void SetSelectedNavigationProperties(IEnumerable<string> selectedPermissions, RoleOutput roleOutput)
        {
            roleOutput.SelectedPermissions = selectedPermissions;
        }

        private void AddPermissionsToRole(IEnumerable<string> selectedPermissions, Guid roleId)
        {
            foreach (var permission in selectedPermissions)
            {
                _dbContext.RoleClaims.Add(new RoleClaim
                {
                    RoleId = roleId,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = permission
                });
            }
        }
    }
}