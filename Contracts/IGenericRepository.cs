using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddAll(IEnumerable<T> entity);
        void Delete(T entity);
        void DeleteAll(IEnumerable<T> entity);
        void Update(T entity);
        void UpdateAll(IEnumerable<T> entity);
        void Save();
        bool Any();
    }
}
