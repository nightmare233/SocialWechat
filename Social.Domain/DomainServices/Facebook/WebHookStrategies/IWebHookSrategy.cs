using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.Facebook
{
    public interface IWebHookSrategy : ITransient
    {
        Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change);
        bool IsMatch(FbHookChange change);
    }
}
