﻿using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Dto;
using BackOfficeBase.Utilities.Collections;

namespace BackOfficeBase.Application.Crud
{
    public interface ICrudAppService<TEntity, TGetOutputDto, TGetListOutput, in TCreateInput, in TUpdateInput>
    {
        Task<TGetOutputDto> GetAsync(Guid id);
        Task<IPagedListResult<TGetListOutput>> GetListAsync(PagedListInput input);
        Task<TGetOutputDto> CreateAsync(TCreateInput input);
        TGetOutputDto Update(TUpdateInput input);
        Task<TGetOutputDto> DeleteAsync(Guid id);
    }
}
