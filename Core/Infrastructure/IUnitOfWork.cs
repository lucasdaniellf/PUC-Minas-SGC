using Core.Entity;

namespace Core.Infrastructure
{
    public interface IUnitOfWork<T> where T : IAggregateRoot
    {
        void Begin(bool beginTransaction = false);
        void BeginTransaction();
        public bool Commit();
        public void Rollback();
        public void CloseConnection();
    }
}
