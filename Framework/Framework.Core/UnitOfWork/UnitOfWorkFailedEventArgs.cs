using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.UnitOfWork
{
    public class UnitOfWorkFailedEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
