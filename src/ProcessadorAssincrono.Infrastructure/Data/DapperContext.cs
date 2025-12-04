using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using System.Data;


namespace ProcessadorAssincrono.Infrastructure.Data
{
    public sealed class DapperContext : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not configured.");
        }

        public IDbConnection CreateConnection() =>
            new SqlConnection(_connectionString);
    }
}
