using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Core;

namespace Social.Application.Dto
{
    public class FilterConditionCreateDto
    {
        [Required]
        public int? FieldId { get; set; }
        [Enum]
        [Required]
        public ConditionMatchType? MatchType { get; set; }
        [Required]
        [MaxLength(200)]
        public string Value { get; set; }
        public int Index { get; set; }
    }
}
