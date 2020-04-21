using System;

namespace BackOfficeBase.Application.Shared.Dto
{
    public abstract class EntityDto
    {
        public virtual Guid Id { get; set; }
    }
}