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
    public class FilterListDto : IHaveCreatedBy
    {
        public int Id { get; set; }
        public int ConversationNum { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int Index { get; set; }
        public bool IfPublic { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
