using System.Threading.Tasks;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    public interface IOrganizationUnitAppService : ICrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnitInput, UpdateOrganizationUnitInput>
    {
        Task<OrganizationUnitOutput> AddUsersToOrganizationUnitAsync(AddUsersToOrganizationUnitInput input);
    }
}
