using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Framework.Core.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle Begin();

        IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope);

        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);

        IUnitOfWork Current { get; }

        Task Run(TransactionScopeOption scope, int? siteId, Func<Task> func);
        Task Run(UnitOfWorkOptions options, int? siteId, Func<Task> func);
        Task RunWithoutTransaction(int? siteId, Func<Task> func);
        void RunWithoutTransaction(int? siteId, Action action);
        Task RunWithNewTransaction(int? siteId, Func<Task> func);
        void RunWithNewTransaction(int? siteId, Action action);
    }
}
