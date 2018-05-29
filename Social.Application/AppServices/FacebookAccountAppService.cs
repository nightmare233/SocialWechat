using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IFacebookAccountAppService
    {
        FacebookPageDto GetPage(int id);
        Task<PendingAddFacebookPagesDto> GetPendingAddPagesAsync(string code, string redirectUri);
        Task<FacebookPageDto> AddPageAsync(AddFaceboookPageDto dto);
        Task DeletePageAsync(int id);
        IList<FacebookPageListDto> GetPages();
        FacebookPageDto UpdatePage(int id, UpdateFacebookPageDto dto);
        FacebookPageDto MarkAsEnable(int id, bool? ifEnable = true);
    }

    public class FacebookAccountAppService : AppService, IFacebookAccountAppService
    {
        private ISocialAccountService _socialAccountService;
        private IFbClient _fbClient;

        public FacebookAccountAppService(
            ISocialAccountService socialAccountService,
            IFbClient fbClient
            )
        {
            _socialAccountService = socialAccountService;
            _fbClient = fbClient;
        }

        public IList<FacebookPageListDto> GetPages()
        {
            return _socialAccountService.FindAll()
                .Where(t => t.SocialUser.Type == SocialUserType.IntegrationAccount && t.SocialUser.Source == SocialUserSource.Facebook)
                .ProjectTo<FacebookPageListDto>().ToList();
        }

        public async Task<PendingAddFacebookPagesDto> GetPendingAddPagesAsync(string code, string redirectUri)
        {
            string userToken = _fbClient.GetUserToken(code, redirectUri);
            FbUser me = await _fbClient.GetMe(userToken);
            IList<FbPage> pages = await _fbClient.GetPages(userToken);
            List<string> pageIds = pages.Select(t => t.Id).ToList();
            var facebookAccounts = _socialAccountService.FindAll()
                .Where(t => t.SocialUser.Type == SocialUserType.IntegrationAccount
                && t.SocialUser.Source == SocialUserSource.Facebook && pageIds.Contains(t.SocialUser.OriginalId))
                .ToList();

            var result = new PendingAddFacebookPagesDto
            {
                SignInAs = new FacebookSignInAsDto
                {
                    Name = me.name,
                    Avatar = me.pic
                },
                Pages = pages.Select(t => new PendingAddFacebookPageDto
                {
                    FacebookId = t.Id,
                    Name = t.Name,
                    Category = t.Category,
                    Avatar = t.Avatar,
                    AccessToken = t.AccessToken,
                    Link = t.Link,
                    IsAdded = facebookAccounts.Any(m => m.SocialUser.OriginalId == t.Id)
                }).ToList()
            };

            return result;
        }

        public FacebookPageDto GetPage(int id)
        {
            var entity = _socialAccountService.FindAccount(id, SocialUserSource.Facebook);
            if (entity == null)
            {
                throw SocialExceptions.FacebookPageNotExists(id);
            }
            return Mapper.Map<FacebookPageDto>(entity);
        }

        public async Task<FacebookPageDto> AddPageAsync(AddFaceboookPageDto dto)
        {
            var socialAccount = Mapper.Map<SocialAccount>(dto);
            socialAccount.SocialUser = Mapper.Map<SocialUser>(dto);

            socialAccount = await _socialAccountService.AddFacebookPageAsync(socialAccount, dto.FacebookId);
            return Mapper.Map<FacebookPageDto>(socialAccount);
        }

        public async Task DeletePageAsync(int id)
        {
            var entity = _socialAccountService.FindAccount(id, SocialUserSource.Facebook);
            if (entity == null)
            {
                throw SocialExceptions.FacebookPageNotExists(id);
            }

            await _socialAccountService.DeleteAsync(entity);
            await _fbClient.UnSubscribeApp(entity.SocialUser.OriginalId, entity.Token);
        }

        public FacebookPageDto UpdatePage(int id, UpdateFacebookPageDto dto)
        {
            var socialAccount = _socialAccountService.FindAccount(id, SocialUserSource.Facebook);
            if (socialAccount == null)
            {
                throw SocialExceptions.FacebookPageNotExists(id);
            }

            socialAccount = Mapper.Map(dto, socialAccount);
            _socialAccountService.Update(socialAccount);

            return Mapper.Map<FacebookPageDto>(socialAccount);
        }

        public FacebookPageDto MarkAsEnable(int id, bool? ifEnable = true)
        {
            var socialAccount = _socialAccountService.MarkAsEnable(id, ifEnable);
            return Mapper.Map<FacebookPageDto>(socialAccount);
        }
    }
}
