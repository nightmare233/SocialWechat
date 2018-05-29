using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class ConversationFieldDto
    {
        public int Id { get; set; }
        public FieldDataType DataType { get; set; }
        public string Name { get; set; }

        public virtual IList<ConversationFieldOptionDto> Options { get; set; }
    }
}
