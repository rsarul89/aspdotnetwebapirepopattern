using Contracts;
using Entities;
using System.Collections.Generic;
using System.Linq;
using Data;
using System;
using System.Data.Entity;

namespace Repository
{
    public class StandardRepository : GenericRepository<Standard>, IStandardRepository<Standard>
    {
        public StandardRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
        public IEnumerable<Standard> GetAllStandards()
        {
            return GetAll().Include("Students").AsEnumerable();
        }

        public Standard GetStandardById(Guid standardId)
        {
            return FindByCondition(s => s.StandardId == standardId).FirstOrDefault();
        }
    }
}
