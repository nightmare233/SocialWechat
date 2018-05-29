using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public abstract class Entity : Entity<int>
    {
    }

    public abstract class Entity<T>
    {
        public T Id { get; set; }
    }
}
