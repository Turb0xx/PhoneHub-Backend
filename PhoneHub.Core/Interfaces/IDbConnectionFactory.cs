using PhoneHub.Core.Enum;
using System.Data;

namespace PhoneHub.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        DataBaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
