using System;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Utilities.Collections;

namespace BackOfficeBase.Application.Shared.Services
{
    public interface ICrudAppService<TEntity, TGetOutputDto, TGetListOutput, in TCreateInput, in TUpdateInput>
    {
        Task<TGetOutputDto> GetAsync(Guid id);
        Task<TGetOutputDto> GetAsync(Guid id, Action<IMappingOperationOptions> mappingOperationOptions);
        Task<IPagedListResult<TGetListOutput>> GetListAsync(PagedListInput input);
        Task<AppServiceResult<TGetOutputDto>> CreateAsync(TCreateInput input);
        AppServiceResult<TGetOutputDto> Update(TUpdateInput input);
        Task<AppServiceResult<TGetOutputDto>> DeleteAsync(Guid id);
    }
}
