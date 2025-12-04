using System.Data;

namespace ProcessadorAssincrono.Infrastructure.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
