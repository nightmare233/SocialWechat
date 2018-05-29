using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Social.WebApi.Models.Account
{
    public class Me
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? SideId { get; set; }
    }
}