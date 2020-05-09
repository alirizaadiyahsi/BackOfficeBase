using System.Threading.Tasks;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    // TODO: Write OrganizationUnitController api
    public interface IOrganizationUnitAppService : ICrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnitInput, UpdateOrganizationUnitInput>
    {
        Task AddUsersToOrganizationUnitAsync(AddOrRemoveUsersToOrganizationUnitInput input);
        void RemoveUsersFromOrganizationUnit(AddOrRemoveUsersToOrganizationUnitInput input);
        Task AddRolesToOrganizationUnitAsync(AddOrRemoveRolesToOrganizationUnitInput input);
        void RemoveRolesFromOrganizationUnit(AddOrRemoveRolesToOrganizationUnitInput input);
    }
}
