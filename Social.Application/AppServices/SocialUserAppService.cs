using AutoMapper;
using Framework.Core;
using Social.Application.Dto;
using Social.Application.Dto.UserInfo;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using Social.Infrastructure.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Application.AppServices
{
    public interface ISocialUserAppService
    {
        Task<UserInfoDto> FindPublicInfoAsync(int socialUserId);
        IList<ConversationDto> FindConversations(int socialUserId);
    }
    public class SocialUserAppService : AppService, ISocialUserAppService
    {
        private ISocialUserService _domainService;
        private IConversationAppService _conversationService;
        private IFbClient _fbClient;
        private ITwitterClient _twitterClient;

        public SocialUserAppService(
            ISocialUserService domainService,
            IConversationAppService conversationService,
            IFbClient fbClient,
            ITwitterClient twitterClient
            )
        {
            _domainService = domainService;
            _conversationService = conversationService;
            _fbClient = fbClient;
            _twitterClient = twitterClient;
        }

        public async Task<UserInfoDto> FindPublicInfoAsync(int socialUserId)
        {
            UserInfoDto userInfoDto = new UserInfoDto { Id = socialUserId };
            SocialUser user = _domainService.FindAllWithDeleted().FirstOrDefault(t => t.Id == socialUserId);
            if (user == null)
            {
                return userInfoDto;
            }

            if (user.Source == SocialUserSource.Facebook)
            {
                FbUser facebookInfo = await _fbClient.GetUserInfo(user.OriginalId);
                userInfoDto.Source = SocialUserSource.Facebook;
                userInfoDto.Name = facebookInfo.name;
                userInfoDto.Email = facebookInfo.email;
                userInfoDto.Link = facebookInfo.link;
                userInfoDto.OriginalId = facebookInfo.id;
                userInfoDto.Avatar = facebookInfo.pic;

                if (user.Avatar != userInfoDto.Avatar || user.Email != userInfoDto.Email)
                {
                    user.Avatar = userInfoDto.Avatar;
                    user.Email = userInfoDto.Email;
                    _domainService.Update(user);
                }
            }
            else if (user.Source == SocialUserSource.Twitter)
            {
                var account = _domainService.FindAll().Where(t => t.Type == SocialUserType.IntegrationAccount && t.Source == SocialUserSource.Twitter).Select(t => t.SocialAccount).FirstOrDefault();
                if (account != null)
                {
                    IUser twitterUserInfo = _twitterClient.GetUser(account.Token, account.TokenSecret, long.Parse(user.OriginalId));
                    if (twitterUserInfo != null)
                    {
                        userInfoDto.Source = SocialUserSource.Twitter;
                        userInfoDto.Name = twitterUserInfo.Name;
                        userInfoDto.OriginalId = twitterUserInfo.Id.ToString();
                        userInfoDto.Avatar = twitterUserInfo.ProfileImageUrl;
                        userInfoDto.Link = TwitterHelper.GetUserUrl(twitterUserInfo.ScreenName);
                        userInfoDto.ScreenName = twitterUserInfo.ScreenName;
                        userInfoDto.Location = twitterUserInfo.Location;
                        userInfoDto.FollowersCount = twitterUserInfo.FollowersCount;
                        userInfoDto.FriendsCount = twitterUserInfo.FriendsCount;
                        userInfoDto.StatusesCount = twitterUserInfo.StatusesCount;
                        userInfoDto.Description = twitterUserInfo.Description;
                        userInfoDto.JoinedDate = twitterUserInfo.CreatedAt;
                        if (twitterUserInfo.Entities.Website != null)
                        {
                            userInfoDto.Website = twitterUserInfo.Entities.Website.Urls.First().DisplayedURL;
                        }
                        if (user.Avatar != userInfoDto.Avatar || user.Email != userInfoDto.Email)
                        {
                            user.Avatar = userInfoDto.Avatar;
                            user.Email = userInfoDto.Email;
                            _domainService.Update(user);
                        }
                    }
                }
            }

            return userInfoDto;
        }

        public IList<ConversationDto> FindConversations(int socialUserId)
        {
            SocialUser user = _domainService.Find(socialUserId);
            if (user == null)
            {
                throw SocialExceptions.SocialUserIdNotExists(socialUserId);
            }

            ConversationSearchDto searchDto = new ConversationSearchDto();
            if (user != null)
            {
                searchDto.UserId = user.Id;
                return _conversationService.Find(searchDto);
            }
            return null;
        }
    }
}
