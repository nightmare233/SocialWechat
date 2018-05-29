using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.EntityFramework
{
    internal class EntityDateTimePropertiesInfo
    {
        public List<PropertyInfo> DateTimePropertyInfos { get; set; }

        public List<string> ComplexTypePropertyPaths { get; set; }

        public EntityDateTimePropertiesInfo()
        {
            DateTimePropertyInfos = new List<PropertyInfo>();
            ComplexTypePropertyPaths = new List<string>();
        }
    }
}
