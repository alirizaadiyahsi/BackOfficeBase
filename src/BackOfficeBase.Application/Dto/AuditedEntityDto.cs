using System;
using BackOfficeBase.Domain.Entities.Auditing;

namespace BackOfficeBase.Application.Dto
{
    public abstract class AuditedEntityDto : CreationAuditedEntityDto, IAudited
    {
        public virtual DateTime? ModificationTime { get; set; }
    
        public virtual Guid? ModifierUserId { get; set; }
    }
}