using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.UnitOfWork
{
    internal class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        public const string DidNotCallCompleteMethodExceptionMessage = "Did not call Complete method of a unit of work.";

        private volatile bool _isCompleteCalled;
        private volatile bool _isDisposed;

        public void Complete()
        {
            _isCompleteCalled = true;
        }

        public Task CompleteAsync()
        {
            _isCompleteCalled = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (!_isCompleteCalled)
            {
                if (HasException())
                {
                    return;
                }

                throw new InvalidOperationException(DidNotCallCompleteMethodExceptionMessage);
            }
        }

        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
