using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_LiveChat_DepartmentMember")]
    public class DepartmentMember:EntityWithSite
    {
        public int DepartmentId { get; set; }
        public int RelatedId { get; set; }
        public short Type { get; set; }
        public virtual Department Department { get; set; }
    }
}
