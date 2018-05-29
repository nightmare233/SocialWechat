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
    [Table("t_Social_TwitterAuth")]
    public class TwitterAuth : Entity
    {
        [MaxLength(50)]
        public string AuthorizationId { get; set; }
        [MaxLength(200)]
        public string AuthorizationKey { get; set; }
        [MaxLength(200)]
        public string AuthorizationSecret { get; set; }
    }
}
