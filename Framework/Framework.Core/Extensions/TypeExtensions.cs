using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttr>(this Type type) where TAttr : Attribute
        {
            return type.GetCustomAttributes(true).Any(t => typeof(TAttr).IsInstanceOfType(t));
        }

        public static TAttr GetAttribute<TAttr>(this Type type) where TAttr : Attribute
        {
            return type.GetCustomAttributes(true).Where(t => typeof(TAttr).IsInstanceOfType(t)).Cast<TAttr>().FirstOrDefault();
        }

        public static List<TAttr> GetAttributes<TAttr>(this Type type) where TAttr : Attribute
        {
            return type.GetCustomAttributes(true).Where(t => typeof(TAttr).IsInstanceOfType(t)).Cast<TAttr>().ToList();
        }
    }
}
