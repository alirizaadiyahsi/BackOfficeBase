using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Application.Shared;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    public interface IOrganizationUnitAppService : ICrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnit, UpdateOrganizationUnit>
    {
    }
}
