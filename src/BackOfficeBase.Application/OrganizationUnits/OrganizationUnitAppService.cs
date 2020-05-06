using AutoMapper;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Application.Shared;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    // TODO: Implement app service methods
    public class OrganizationUnitAppService : CrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnit, UpdateOrganizationUnit>, IOrganizationUnitAppService
    {
        public OrganizationUnitAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}