using System;

namespace BackOfficeBase.Domain.Entities.Auditing
{
    public interface IHasDeletionTime
    {
        DateTime? DeletionTime { get; set; }
    }
}