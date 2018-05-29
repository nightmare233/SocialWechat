using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class AddFaceboookPageDto
    {
        [Required]
        [MaxLength(200)]
        public string FacebookId { get; set; }
        [Required]
        [MaxLength(2000)]
        public string AccessToken { get; set; }
        [MaxLength(256)]
        public string Avatar { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Category { get; set; }
        [MaxLength(200)]
        public string SignInAs { get; set; }
        [MaxLength(500)]
        public string Link { get; set; }
    }
}
