using Framework.Core;
using Moq;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.AppServices
{
    public class FacebookAccountAppServiceTest : TestBase
    {
        [Fact]
        public void ShouldGetFacebookDirectMessages()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var fbClient = new Mock<IFbClient>();

            socialAccountService.Setup(t => t.FindAll()).Returns(new List<SocialAccount> { new SocialAccount { SocialUser = new SocialUser { Source = SocialUserSource.Facebook } } }.AsQueryable());
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            IList<FacebookPageListDto> facebookPageListDtos = conversationMessageAppService.GetPages();
            //Assert
            Assert.True(facebookPageListDtos.Any());
        }

        [Fact]
        public void ShouldGetPendingAddPagesAsync()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();
            fbClient.Setup(t => t.GetUserToken("a", "b")).Returns("1");
            fbClient.Setup(t => t.GetMe("1")).ReturnsAsync(new FbUser { name = "a", pic = "123" });
            fbClient.Setup(t => t.GetPages("1")).ReturnsAsync(new List<FbPage>
            {
                new FbPage{ Id ="1",Name = "a",Category = "b",AccessToken = "d",Link = "e"}
            });
            fbClient.Setup(t => t.GetUserInfo("1", "1")).ReturnsAsync(new FbUser { pic = "c" });
            socialAccountService.Setup(t => t.FindAll()).Returns(new List<SocialAccount>
            {
                new SocialAccount{ Id =1,SocialUser = new SocialUser{ OriginalId = "1",Source = SocialUserSource.Facebook} }
            }.AsQueryable());
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            Task<PendingAddFacebookPagesDto> pendingAddFacebookPagesDto = conversationMessageAppService.GetPendingAddPagesAsync("a", "b");
            //Assert
            Assert.NotNull(pendingAddFacebookPagesDto);
            Assert.Equal("a", pendingAddFacebookPagesDto.Result.SignInAs.Name);
            Assert.Equal("123", pendingAddFacebookPagesDto.Result.SignInAs.Avatar);
            Assert.Equal("1", pendingAddFacebookPagesDto.Result.Pages.FirstOrDefault().FacebookId);
            Assert.Equal(true, pendingAddFacebookPagesDto.Result.Pages.FirstOrDefault().IsAdded);
        }

        [Fact]
        public void ShouldGetPageById()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns(new SocialAccount { Id = 1 });
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            FacebookPageDto facebookPageListDtos = conversationMessageAppService.GetPage(1);
            //Assert
            Assert.NotNull(facebookPageListDtos);
            Assert.Equal(1, facebookPageListDtos.Id);
        }

        [Fact]
        public void ShouldGetPageByIdWhenFacebookPageNotExists()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns<SocialAccount>(null);
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            Action action = () => conversationMessageAppService.GetPage(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldDeletePageAsync()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();
            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns(new SocialAccount { Id = 1, SocialUser = new SocialUser { OriginalId = "1" }, Token = "123" });
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            conversationMessageAppService.DeletePageAsync(1);
            //Assert
            socialAccountService.Verify(t => t.DeleteAsync(It.Is<SocialAccount>(r => r.Id == 1)));
            fbClient.Verify(t => t.UnSubscribeApp(It.Is<string>(s => s == "1"), It.Is<string>(k => k == "123")));
        }

        [Fact]
        public void ShouldDeletePageAsyncWhenAccountIsNotFound()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns<SocialAccount>(null);
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, null);

            //Act
            Action action = () => conversationMessageAppService.DeletePageAsync(1).Start();
            //Assert
            Assert.NotNull(conversationMessageAppService.DeletePageAsync(1).Exception);
            // Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldUpdatePage()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();
            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns(new SocialAccount { Id = 1, SocialUser = new SocialUser { OriginalId = "1" }, Token = "123" });
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);

            //Act
            FacebookPageDto facebookPageDto = conversationMessageAppService.UpdatePage(1, new UpdateFacebookPageDto { });
            //Assert
            Assert.NotNull(facebookPageDto);
            socialAccountService.Verify(t => t.Update(It.Is<SocialAccount>(r => r.Id == 1)));
        }

        [Fact]
        public void ShouldUpdatePageWhenAccountIsNotFound()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();
            var socialUserService = new Mock<ISocialUserService>();
            var fbClient = new Mock<IFbClient>();
            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Facebook)).Returns<SocialAccount>(null);
            FacebookAccountAppService conversationMessageAppService = new FacebookAccountAppService(socialAccountService.Object, fbClient.Object);
            //Act
            Action action = () => conversationMessageAppService.UpdatePage(1, new UpdateFacebookPageDto { });
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }
    }
}
