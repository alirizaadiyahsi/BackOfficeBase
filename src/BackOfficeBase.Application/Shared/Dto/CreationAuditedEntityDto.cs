using System;
using BackOfficeBase.Domain.Entities.Auditing;

namespace BackOfficeBase.Application.Shared.Dto
{
    public abstract class CreationAuditedEntityDto : EntityDto, ICreationAudited
    {
        public virtual DateTime CreationTime { get; set; }

        public virtual Guid? CreatorUserId { get; set; }
    }
}