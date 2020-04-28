using System;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Authorization.Users.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Application.Authorization.Users
{
    // TODO: Write test
    public class UserAppService : CrudAppService<User, UserOutput, UserListOutput, CreateUserInput, UpdateUserInput>, IUserAppService
    {
        public UserAppService(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override Task<UserOutput> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
    }
}