using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.UnitOfWork
{
    public interface ICurrentUnitOfWorkProvider
    {
        IUnitOfWork Current { get; set; }
    }
}
