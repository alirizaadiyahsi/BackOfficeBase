using Microsoft.AspNetCore.Authorization;

namespace BackOfficeBase.Web.Core.Controllers
{
    [Authorize]
    public class AuthorizedController : ApiControllerBase
    {
    }
}