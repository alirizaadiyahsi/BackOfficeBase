using System;
using BackOfficeBase.Domain.Entities.Auditing;

namespace BackOfficeBase.Application.Dto
{
    public abstract class FullAuditedEntityDto : AuditedEntityDto, IFullAudited
    {
        public virtual DateTime? DeletionTime { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual Guid? DeleterUserId { get; set; }
    }
}