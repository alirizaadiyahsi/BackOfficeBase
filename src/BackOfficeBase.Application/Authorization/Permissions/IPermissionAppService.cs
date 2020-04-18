using System.Threading.Tasks;

namespace BackOfficeBase.Application.Authorization.Permissions
{
    public interface IPermissionAppService
    {
        Task<bool> IsUserGrantedToPermissionAsync(string userName, string permission);
    }
}
