
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}
