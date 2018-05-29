using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class FilterConditionDto
    {
        public int Id { get; set; }
        public int FilterId { get; set; }
        public int FieldId { get; set; }
        public ConditionMatchType MatchType { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
    }
}
