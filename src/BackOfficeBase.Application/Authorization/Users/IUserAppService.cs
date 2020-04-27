using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Domain.Entities.Authorization;

namespace BackOfficeBase.Application.Authorization.Users
{
    public interface IUserAppService : ICrudAppService<User, UserOutput, UserListOutput, CreateUserInput, UpdateUserInput>
    {

    }
}
