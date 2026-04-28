using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PhoneHub.Core.Enum;
using PhoneHub.Core.Interfaces;
using System.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _mySqlConn;
        public DataBaseProvider Provider { get; }

        public DbConnectionFactory(IConfiguration config)
        {
            _mySqlConn = config.GetConnectionString("ConnectionMySql") ?? string.Empty;
            Provider = DataBaseProvider.MySql;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_mySqlConn);
        }
    }
}
