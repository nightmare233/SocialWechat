using System.Data.Entity;

namespace Framework.Core.UnitOfWork
{
    public interface ITransactionStrategy
    {
        void Commit();
        DbContext CreateDbContext<TDbContext>(string connectionString) where TDbContext : DbContext;
        void Dispose();
        void StartTransaction(UnitOfWorkOptions options);
    }
}