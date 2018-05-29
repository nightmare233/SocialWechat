using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Site_SocialAccount")]
    public class SiteSocialAccount : Entity
    {
        public int SiteId { get; set; }
        public string FacebookPageId { get; set; }
        public string TwitterUserId { get; set; }
    }
}
