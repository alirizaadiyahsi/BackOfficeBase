using AutoMapper;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Application.Authorization.Users
{
    public class UserAppService : CrudAppService<User, UserOutput, UserListOutput, CreateUserInput, UpdateUserInput>, IUserAppService
    {
        public UserAppService(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}