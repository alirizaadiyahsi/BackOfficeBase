using BackOfficeBase.Application.Shared.Services.Crud;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    public interface IOrganizationUnitAppService : ICrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnit, UpdateOrganizationUnit>
    {
    }
}
