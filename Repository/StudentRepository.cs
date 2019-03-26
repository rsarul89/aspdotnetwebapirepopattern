using Contracts;
using Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository<Student>
    {
        public StudentRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
        public IEnumerable<Student> GetAllStudents()
        {
            return GetAll();
        }

        public Student GetStudentById(Guid studentId)
        {
            return FindByCondition(s => s.StudentID == studentId).FirstOrDefault();
        }
    }
}
