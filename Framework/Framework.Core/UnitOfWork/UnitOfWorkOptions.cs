using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Framework.Core.UnitOfWork
{
    public class UnitOfWorkOptions
    {
        public TransactionScopeOption Scope { get; set; } = TransactionScopeOption.Required;
        public bool IsTransactional { get; set; } = true;
        public TimeSpan? Timeout { get; set; }
        public IsolationLevel? IsolationLevel { get; set; }

        public UnitOfWorkOptions()
        {
        }
    }
}
