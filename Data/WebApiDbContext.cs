using Entities;
using System.Data.Common;
using System.Data.Entity;

namespace Data
{
    public class WebApiDbContext : DbContext
    {
        public WebApiDbContext() : base("DefaultDbConnection")
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }
        //Constructor to use on a DbConnection that is already opened
        public WebApiDbContext(DbConnection existingConnection, bool contextOwnsConnection)
          : base(existingConnection, contextOwnsConnection)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
        }

        public DbSet<Standard> Standards { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}