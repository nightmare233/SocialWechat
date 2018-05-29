using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class PendingAddFacebookPagesDto
    {
        public FacebookSignInAsDto SignInAs { get; set; }
        public List<PendingAddFacebookPageDto> Pages { get; set; }
    }
}
