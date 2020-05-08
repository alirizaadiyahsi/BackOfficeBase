using System.Threading.Tasks;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    public interface IOrganizationUnitAppService : ICrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnitInput, UpdateOrganizationUnitInput>
    {
        Task<OrganizationUnitOutput> AddUsersToOrganizationUnitAsync(AddOrRemoveUsersToOrganizationUnitInput input);
        // TODO: Add Async suffix
        Task<OrganizationUnitOutput> RemoveUsersFromOrganizationUnit(AddOrRemoveUsersToOrganizationUnitInput input);
        Task<OrganizationUnitOutput> AddRolesToOrganizationUnitAsync(AddOrRemoveRolesToOrganizationUnitInput input);
        // TODO: Add Async suffix
        Task<OrganizationUnitOutput> RemoveRolesFromOrganizationUnit(AddOrRemoveRolesToOrganizationUnitInput input);
    }
}
