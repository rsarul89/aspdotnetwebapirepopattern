using Contracts;
using Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private WebApiDbContext dbContext;
        private readonly IDbSet<T> _dbset;
        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected WebApiDbContext DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            _dbset = DbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void AddAll(IEnumerable<T> entity)
        {
            foreach (var ent in entity)
            {
                _dbset.Add(ent);
            }
        }

        public virtual bool Any()
        {
            return _dbset.Any();
        }

        public virtual void Delete(T entity)
        {
            var entry = DbContext.Entry(entity);
            entry.State = EntityState.Deleted;
            _dbset.Remove(entity);
        }

        public virtual void DeleteAll(IEnumerable<T> entity)
        {
            foreach (var ent in entity)
            {
                var entry = DbContext.Entry(ent);
                entry.State = EntityState.Deleted;
                _dbset.Remove(ent);
            }
        }

        public virtual IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _dbset.Where(expression);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbset;
        }

        public virtual void Save()
        {
            DbContext.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            var entry = DbContext.Entry(entity);
            _dbset.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public virtual void UpdateAll(IEnumerable<T> entity)
        {
            foreach (var ent in entity)
            {
                var entry = DbContext.Entry(ent);
                _dbset.Attach(ent);
                entry.State = EntityState.Modified;
            }
        }
    }
}
