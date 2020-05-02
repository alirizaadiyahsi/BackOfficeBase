using AutoMapper;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Application.Shared.Services.Crud;
using BackOfficeBase.Domain.AppConstants.Authorization;
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
}