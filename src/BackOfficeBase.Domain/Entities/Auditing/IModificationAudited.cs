using System;

namespace BackOfficeBase.Domain.Entities.Auditing
{
    public interface IModificationAudited : IHasModificationTime
    {
        Guid? ModifierUserId { get; set; }
    }

    public interface IModificationAudited<TUser> : IModificationAudited
    {
        TUser ModifierUser { get; set; }
    }
}