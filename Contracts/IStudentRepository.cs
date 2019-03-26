using Entities;
using System;
using System.Collections.Generic;

namespace Contracts
{
    public interface IStudentRepository<T> where T : BaseEntity
    {
        T GetStudentById(Guid studentId);
        IEnumerable<T> GetAllStudents();
    }
}
