using AutoMapper;
using BackOfficeBase.Application.Shared.Services.Crud;
using BackOfficeBase.Domain.Entities.OrganizationUnits;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Application.OrganizationUnits
{
    // TODO: Implement app service methods
    public class OrganizationUnitAppService : CrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnit, UpdateOrganizationUnit>, IOrganizationUnitAppService
    {
        public OrganizationUnitAppService(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }

    public class UpdateOrganizationUnit
    {
    }

    public class CreateOrganizationUnit
    {
    }

    public class OrganizationUnitListOutput
    {
    }

    public class OrganizationUnitOutput
    {
    }
}