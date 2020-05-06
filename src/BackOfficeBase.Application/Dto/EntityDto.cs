using System;

namespace BackOfficeBase.Application.Dto
{
    public abstract class EntityDto
    {
        public virtual Guid Id { get; set; }
    }
}