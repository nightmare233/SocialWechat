using Framework.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Framework.Core.UnitOfWork
{
    public class TransactionStrategy : ITransactionStrategy, ITransient
    {
        protected TransactionScope CurrentTransaction { get; set; }

        protected List<DbContext> DbContexts { get; }

        private IDependencyResolver _dependencyResolver;

        public TransactionStrategy(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            DbContexts = new List<DbContext>();
        }

        public DbContext CreateDbContext<TDbContext>(string nameOrConnectionString)
         where TDbContext : DbContext
        {
            IDbContextResolver dbContextResolver = _dependencyResolver.Resolve<IDbContextResolver>();
            var dbContext = dbContextResolver.Resolve<TDbContext>(nameOrConnectionString);
            DbContexts.Add(dbContext);
            return dbContext;
        }

        public void StartTransaction(UnitOfWorkOptions options)
        {
            options = options ?? new UnitOfWorkOptions();

            if (CurrentTransaction != null)
            {
                return;
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadCommitted)
            };

            if (options.Timeout.HasValue)
            {
                transactionOptions.Timeout = options.Timeout.Value;
            }

            CurrentTransaction = new TransactionScope(
                options.Scope,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled
            );
        }

        public void Commit()
        {
            if (CurrentTransaction == null)
            {
                return;
            }

            CurrentTransaction.Complete();

            CurrentTransaction.Dispose();
            CurrentTransaction = null;
        }

        public void Dispose()
        {
            foreach (var dbContext in DbContexts)
            {
                _dependencyResolver.Release(dbContext);
            }

            DbContexts.Clear();

            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }
}
