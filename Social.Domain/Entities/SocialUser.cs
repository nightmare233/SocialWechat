using Framework.Core;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Social_User")]
    public class SocialUser : EntityWithSite, ISoftDelete
    {
        public SocialUser()
        {
        }

        [Required]
        [MaxLength(200)]
        public string OriginalId { get; set; }

        [MaxLength(500)]
        public string OriginalLink { get; set; }

        [Required]
        public SocialUserSource Source { get; set; }

        [Required]
        public SocialUserType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string ScreenName { get; set; }

        public string ScreenNameOrNormalName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ScreenName) ? ScreenName : Name;
            }
        }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(256)]
        public string Avatar { get; set; }

        public bool IsDeleted { get; set; }

        public virtual SocialAccount SocialAccount { get; set; }

        public bool IsIntegrationAccount { get { return Type == SocialUserType.IntegrationAccount; } }
        public bool IsCustomer { get { return Type == SocialUserType.Customer; } }
    }
}
