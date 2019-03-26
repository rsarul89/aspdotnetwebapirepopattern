using Entities;
using System;
using System.Collections.Generic;

namespace Contracts
{
    public interface IStandardRepository<T> where T : BaseEntity
    {
        T GetStandardById(Guid standardId);
        IEnumerable<T> GetAllStandards();
    }
}
