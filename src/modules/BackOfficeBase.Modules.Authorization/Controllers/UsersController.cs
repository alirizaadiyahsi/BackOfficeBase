using System.Threading.Tasks;
using BackOfficeBase.Web.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Modules.Authorization.Controllers
{
    public class UsersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<object> GetUsers()
        {
            return Ok();
        }
    }
}
