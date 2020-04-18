using System;

namespace BackOfficeBase.Domain.Entities.Auditing
{
    public interface IHasModificationTime
    {
        DateTime? ModificationTime { get; set; }
    }
}