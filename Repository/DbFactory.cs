using Data;
using System.Configuration;

namespace Repository
{
    public class DbFactory : IDbFactory
    {
        WebApiDbContext dbContext;

        public WebApiDbContext Init()
        {
            return dbContext ?? (dbContext = new WebApiDbContext());

            //string connectionString = ConfigurationManager.ConnectionStrings["OLSDbConnection"].ToString();
            //MySqlConnection connection = new MySqlConnection(connectionString);
            //if (connection.State == System.Data.ConnectionState.Closed)
            //    connection.Open();
            //else if (connection.State == System.Data.ConnectionState.Open)
            //    connection.Close();
            //return dbContext ?? (dbContext = new OLSDbContext(connection, false));
        }
    }
}
