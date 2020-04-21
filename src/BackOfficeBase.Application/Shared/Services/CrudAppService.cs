using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Utilities.Collections;
using BackOfficeBase.Utilities.Collections.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeBase.Application.Shared.Services
{
    public abstract class CrudAppService<TEntity, TGetOutputDto, TGetListOutput, TCreateInput, TUpdateInput>
        : ICrudAppService<TEntity, TGetOutputDto, TGetListOutput, TCreateInput, TUpdateInput>
        where TEntity : class
    {
        private readonly BackOfficeBaseDbContext _dbContext;
        private readonly IMapper _mapper;

        protected CrudAppService(BackOfficeBaseDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<TGetOutputDto> GetAsync(Guid id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);

            return _mapper.Map<TGetOutputDto>(entity);
        }

        public async Task<IPagedList<TGetListOutput>> GetListAsync(PagedListInput input)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            foreach (var (field, condition) in input.Filters)
            {
                query = query.Where($"{field} {condition}");
            }

            IOrderedQueryable<TEntity> orderedQuery = null;
            foreach (var (field, sortBy) in input.Sorts)
            {
                orderedQuery = orderedQuery == null
                    ? query.OrderBy($"{field} {sortBy}")
                    : orderedQuery.ThenBy($"{field} {sortBy}");
            }

            var count = await orderedQuery.CountAsync();
            var pagedList = query.PagedBy(input.PageIndex, input.PageSize).ToList();
            var pagedListOutput = _mapper.Map<List<TGetListOutput>>(pagedList);

            return pagedListOutput.ToPagedList(count);
        }

        public async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            var entity = _mapper.Map<TEntity>(input);
            var result = await _dbContext.AddAsync(entity);

            return _mapper.Map<TGetOutputDto>(result.Entity);
        }

        public TGetOutputDto Update(TUpdateInput input)
        {
            var entity = _mapper.Map<TEntity>(input);
            var result = _dbContext.Update(entity);

            return _mapper.Map<TGetOutputDto>(result.Entity);
        }

        public async Task<TGetOutputDto> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            var result = _dbContext.Remove(entity);

            return _mapper.Map<TGetOutputDto>(result.Entity);
        }
    }
}