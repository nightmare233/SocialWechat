using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Social_ConversationFieldOption")]
    public class ConversationFieldOption : EntityWithSite
    {
        public int FieldId { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Value { get; set; }
        public int Index { get; set; }

        public virtual ConversationField Field { get; set; }
    }
}
