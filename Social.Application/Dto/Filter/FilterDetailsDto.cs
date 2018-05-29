using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class FilterDetailsDto : IHaveCreatedBy
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public int Index { get; set; }
        public bool IfPublic { get; set; }
        public FilterType Type { get; set; }
        public string LogicalExpression { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual IList<FilterConditionDto> Conditions { get; set; }
    }
}
