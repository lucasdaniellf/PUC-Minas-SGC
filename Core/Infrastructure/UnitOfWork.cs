using Core.Entity;
using System.Data;
using System.Data.Common;

namespace Core.Infrastructure
{
    public sealed class UnitOfWork<T> : IUnitOfWork<T> where T : IAggregateRoot
    {
        private readonly IDbContext<T> _context;
        public UnitOfWork(IDbContext<T> context)
        {
            _context = context;
        }

        public void Begin(bool beginTransaction = false)
        {
            _context.Connection.Open();
            //if (beginTransaction)
            //{
            //    _context.Transaction = _context.Connection.BeginTransaction();
            //}
        }

        public void BeginTransaction()
        {
            if(_context.Connection.State == ConnectionState.Open)
            {
                _context.Transaction = _context.Connection.BeginTransaction();
            }
        }

        public bool Commit()
        {
            var success = false;
            try
            {
                _context.Transaction?.Commit();
                CloseConnection();
                success = true;

            }
            catch (DbException e)
            {
                Rollback();
                Console.WriteLine(e.Message);
            }

            return success;
        }


        public void Rollback()
        {
            _context.Transaction?.Rollback();
            CloseConnection();
        }

        public void CloseConnection()
        {
            _context.Transaction?.Dispose();
            _context.Connection.Close();
            _context.Transaction = null;

        }
    }
}
