using BackOfficeBase.Application.Authorization.Users;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Domain.Entities.Authorization;
using BackOfficeBase.Web.Core.Controllers;

namespace BackOfficeBase.Modules.Authorization.Controllers
{
    public class UsersController : CrudController<IUserAppService,User,UserOutput, UserListOutput,CreateUserInput, UpdateUserInput>
    {
        public UsersController(IUserAppService appService) : base(appService)
        {
        }
    }
}
