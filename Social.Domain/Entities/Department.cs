using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_LiveChat_Department")]
    public class Department : EntityWithSite
    {
        public Department()
        {
            Members = new List<DepartmentMember>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BackupDepartmentId { get; set; }
        public bool IfDeleted { get; set; }

        public virtual IList<DepartmentMember> Members { get; set; }

    }
}
