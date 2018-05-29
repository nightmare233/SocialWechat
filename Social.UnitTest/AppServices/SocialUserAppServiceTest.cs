using Framework.Core;
using Moq;
using Social.Application.AppServices;
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
using Tweetinvi.Models.Entities;
using Xunit;

namespace Social.UnitTest.AppServices
{
    public class SocialUserAppServiceTest
    {
        [Fact]
        public async void ShouldFindPublicFacebookInfoAsync()
        {
            //Arrange
            var domainService =new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();

            domainService.Setup(t => t.Find(1)).Returns(new SocialUser {Id = 1,OriginalId = "1",Source = SocialUserSource.Facebook });
            fbClient.Setup(t => t.GetUserInfo("1")).ReturnsAsync(MakeFbUserEntity());
            SocialUserAppService socialUserAppService = new SocialUserAppService(domainService.Object,
                null, fbClient.Object, null);
            //Act
            UserInfoDto userInfoDto = await socialUserAppService.FindPublicInfoAsync(1);
            //Assert
            Assert.Equal(SocialUserSource.Facebook, userInfoDto.Source);
            AssertDtoEqualToEntity(MakeFbUserEntity(),userInfoDto);

        }

        [Fact]
        public async void ShouldFindPublicTwitterInfoAsync()
        {
            //Arrange
            var domainService = new Mock<ISocialUserService>();
            var conversationService = new Mock<IConversationAppService>();
            var fbClient = new Mock<IFbClient>();
            var twitterClient = new Mock<ITwitterClient>();
            var iUser = new Mock<IUser>();
            var iUserEntities = new Mock<IUserEntities>();
            var iWebsiteEntity = new Mock<IWebsiteEntity>();
            var iUrlEntity = new Mock<IUrlEntity>();
            iUrlEntity.Setup(t => t.DisplayedURL).Returns("website");
            iWebsiteEntity.Setup(t => t.Urls).Returns(new List<IUrlEntity> { iUrlEntity.Object }.AsEnumerable()); 
            iUserEntities.Setup(t => t.Website).Returns(iWebsiteEntity.Object);
            iUser.Setup(t => t.Name).Returns("name");
            iUser.Setup(t => t.Id).Returns(1);
            iUser.Setup(t => t.ProfileImageUrl).Returns("profileImageUrl");
            iUser.Setup(t => t.ScreenName).Returns("screenName");
            iUser.Setup(t => t.Location).Returns("location");
            iUser.Setup(t => t.FollowersCount).Returns(1);
            iUser.Setup(t => t.FriendsCount).Returns(1);
            iUser.Setup(t => t.StatusesCount).Returns(1);
            iUser.Setup(t => t.Description).Returns("description");
            iUser.Setup(t => t.CreatedAt).Returns(DateTime.UtcNow);
            iUser.Setup(t => t.Entities).Returns(iUserEntities.Object);
            domainService.Setup(t => t.Find(1)).Returns(new SocialUser { Id = 1, OriginalId = "1", Source = SocialUserSource.Twitter });
            domainService.Setup(t => t.FindAll()).Returns(new List<SocialUser>
            {
                new SocialUser { Type = SocialUserType.IntegrationAccount,Source = SocialUserSource.Twitter,SocialAccount  =new SocialAccount{ Token  ="token",TokenSecret = "tokenSecret"} }
            }.AsQueryable());
            twitterClient.Setup(t => t.GetUser("token", "tokenSecret", 1)).Returns(iUser.Object);
            SocialUserAppService socialUserAppService = new SocialUserAppService(domainService.Object,
                conversationService.Object, fbClient.Object, twitterClient.Object);
            //Act
            UserInfoDto userInfoDto = await socialUserAppService.FindPublicInfoAsync(1);
            //Assert
            Assert.Equal(SocialUserSource.Twitter, userInfoDto.Source);
            AssertDtoEqualToEntity(iUser.Object, userInfoDto);
            Assert.Equal("website", userInfoDto.Website);
        }

        [Fact]
        public async void ShouldFindPublicInfoAsyncWhenUserIsNotFound()
        {
            //Arrange
            var domainService = new Mock<ISocialUserService>();

            domainService.Setup(t => t.Find(1)).Returns<SocialUser>(null);
            SocialUserAppService socialUserAppService = new SocialUserAppService(domainService.Object,
                null, null, null);
            //Act
           Func<Task> action = ()=> socialUserAppService.FindPublicInfoAsync(1);
            //Assert
            Assert.ThrowsAsync<ExceptionWithCode>(action);

        }

        [Fact]
        public void ShouldFindConversations()
        {
            //Arrange
            var domainService = new Mock<ISocialUserService>();
            var conversationService = new Mock<IConversationAppService>();

            domainService.Setup(t => t.Find(1)).Returns(new SocialUser { Id = 1 });
            conversationService.Setup(t => t.Find(It.Is<ConversationSearchDto>(r => r.UserId ==1))).Returns(new List<ConversationDto>
            {
                new ConversationDto{ Id = 1}
            });
            SocialUserAppService socialUserAppService = new SocialUserAppService(domainService.Object,
                conversationService.Object, null, null);
            //Act
            IList<ConversationDto> conversationDtos =  socialUserAppService.FindConversations(1);
            //Assert
            Assert.True(conversationDtos.Any());
            Assert.Equal(1, conversationDtos.FirstOrDefault().Id);
        }

        [Fact]
        public void ShouldFindConversationsWhenUserIsNotFound()
        {
            //Arrange
            var domainService = new Mock<ISocialUserService>();
            var conversationService = new Mock<IConversationAppService>();

            domainService.Setup(t => t.Find(1)).Returns<SocialUser>(null);
            SocialUserAppService socialUserAppService = new SocialUserAppService(domainService.Object,
                conversationService.Object, null, null);
            //Act
           Action action =() => socialUserAppService.FindConversations(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }


        private FbUser MakeFbUserEntity()
        {
            return new FbUser
            {
                name = "name",
                email = "email",
                link = "link",
                id = "originalId",
                pic = "avatar"
            };
        }

        private void AssertDtoEqualToEntity(FbUser entity, UserInfoDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.name,dto.Name);
            Assert.Equal(entity.email, dto.Email);
            Assert.Equal(entity.link, dto.Link);
            Assert.Equal(entity.id, dto.OriginalId);
            Assert.Equal(entity.pic, dto.Avatar);
        }
        private void AssertDtoEqualToEntity(IUser entity, UserInfoDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Id.ToString(), dto.OriginalId);
            Assert.Equal(entity.ProfileImageUrl, dto.Avatar);
            Assert.Equal(entity.ScreenName, dto.ScreenName);
            Assert.Equal(entity.Location, dto.Location);
            Assert.Equal(entity.FollowersCount, dto.FollowersCount);
            Assert.Equal(entity.FriendsCount, dto.FriendsCount);
            Assert.Equal(entity.StatusesCount, dto.StatusesCount);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.CreatedAt, dto.JoinedDate);
            Assert.Equal(TwitterHelper.GetUserUrl(entity.ScreenName), dto.Link);
        }
    }
}
