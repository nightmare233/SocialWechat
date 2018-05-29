using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Social.WebApi.Models.Account
{
    public class AuthenticationModel
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        public string SessionId { get; set; }
    }
}