using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public static class EnumExtensions
    {
        public static string GetName(this Enum enumObj)
        {
            var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            var displayAttr = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayAttr != null)
            {
                return ((DisplayAttribute)displayAttr).Name;
            }
            return Enum.GetName(enumObj.GetType(), enumObj);
        }
    }
}
