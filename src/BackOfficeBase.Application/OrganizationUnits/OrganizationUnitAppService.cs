using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    // TODO: Write unit tests
    public class OrganizationUnitAppService : CrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnitInput, UpdateOrganizationUnitInput>, IOrganizationUnitAppService
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        private readonly IMapper _mapper;

        public OrganizationUnitAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<OrganizationUnitOutput> AddUsersToOrganizationUnitAsync(AddUsersToOrganizationUnitInput input)
        {
            foreach (var selectedUserId in input.SelectedUserIds)
            {
                await _dbContext.OrganizationUnitUsers.AddAsync(new OrganizationUnitUser
                {
                    UserId = selectedUserId,
                    OrganizationUnitId = input.OrganizationUnitId
                });
            }

            return await base.GetAsync(input.OrganizationUnitId);
        }

        // TODO: Implement following methods
        // RemoveUserFromOU
        // AddRoleToOU
        // RemoveRoleFromOU
    }
}