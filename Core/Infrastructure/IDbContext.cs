using Core.Entity;
using System.Data;

namespace Core.Infrastructure
{
    public interface IDbContext<T> where T : IAggregateRoot
    {
        public IDbConnection Connection { get; }
        public IDbTransaction? Transaction { get; set; }
        public IDbConnection CreateConnection(string connectionString);
    }
}
