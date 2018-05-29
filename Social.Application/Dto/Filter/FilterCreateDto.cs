using Social.Domain.Entities;
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
    public class FilterCreateDto
    {
        public FilterCreateDto()
        {
            Conditions = new List<FilterConditionCreateDto>();
        }
        //   public int ConversationId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public int Index { get; set; }
        public bool IfPublic { get; set; }
        [Enum]
        [Required]
        public FilterType? Type { get; set; }
        [MaxLength(200)]
        public string LogicalExpression { get; set; }

        // public virtual Conversation Conversation { get; set; }
        public virtual IList<FilterConditionCreateDto> Conditions { get; set; }
    }
}
