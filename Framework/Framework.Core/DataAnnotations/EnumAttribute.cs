using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class EnumAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var enumType = value.GetType();
                bool valid = Enum.IsDefined(enumType, value);
                if (!valid)
                {
                    return new ValidationResult($"{value} is not valid for type {enumType.Name}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
