using Framework.Core.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public abstract class ServiceBase
    {
        public IUserContext UserContext { get; set; }

        public IDependencyResolver DependencyResolver { get; set; }

        private IUnitOfWorkManager _unitOfWorkManager;
        public virtual IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new InvalidOperationException("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }

        protected IUnitOfWork CurrentUnitOfWork { get { return UnitOfWorkManager.Current; } }
    }
}
