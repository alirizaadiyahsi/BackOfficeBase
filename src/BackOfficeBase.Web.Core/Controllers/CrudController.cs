using System;
using System.Threading.Tasks;
using BackOfficeBase.Application.Shared.Dto;
using BackOfficeBase.Application.Shared.Services;
using BackOfficeBase.Web.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeBase.Web.Core.Controllers
{
    // TODO: Write test
    public class CrudController<TAppService, TEntity, TGetOutput, TGetListOutput, TCreateInput, TUpdateInput> : AuthorizedController
        where TAppService : ICrudAppService<TEntity, TGetOutput, TGetListOutput, TCreateInput, TUpdateInput>
    {
        private readonly TAppService _appService;

        public CrudController(TAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public virtual async Task<ActionResult<TGetOutput>> Get(Guid id)
        {
            var entityDto = await _appService.GetAsync(id);
            if (entityDto == null) return NotFound(Messages.EntityNotFound);

            return Ok(entityDto);
        }

        [HttpGet]
        public virtual async Task<ActionResult<TGetOutput>> Get(PagedListInput input)
        {
            var pagedListResult = await _appService.GetListAsync(input);
            return Ok(pagedListResult);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TGetOutput>> Post(TCreateInput input)
        {
            var entityDto = await _appService.CreateAsync(input);
            return Ok(entityDto);
        }

        [HttpPut]
        public virtual async Task<ActionResult<TGetOutput>> Put(TUpdateInput input)
        {
            var entityDto = _appService.Update(input);
            return Ok(entityDto);
        }

        [HttpDelete]
        public virtual async Task<ActionResult<TGetOutput>> Delete(Guid id)
        {
            var entityDto = await _appService.DeleteAsync(id);
            return Ok(entityDto);
        }
    }
}
