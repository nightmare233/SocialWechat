using Framework.Core;
using Moq;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Xunit;

namespace Social.UnitTest.AppServices
{
    public class TwitterAccountAppServiceTest : TestBase
    {
        [Fact]
        public void ShouldGetAccounts()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAll()).Returns(new List<SocialAccount>
            {
                new SocialAccount{ Id =1, SocialUser = new SocialUser{ Source = SocialUserSource.Twitter} }
            }.AsQueryable());
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            IList<TwitterAccountListDto> twitterAccountListDtos = TwitterAccountAppService.GetAccounts();
            //Assert
            Assert.True(twitterAccountListDtos.Any());
            Assert.Equal(1, twitterAccountListDtos.FirstOrDefault().Id);
        }

        [Fact]
        public void ShouldGetAccount()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns(
                new SocialAccount { Id = 1 });
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            TwitterAccountDto twitterAccountDto = TwitterAccountAppService.GetAccount(1);
            //Assert
            Assert.NotNull(twitterAccountDto);
            Assert.Equal(1, twitterAccountDto.Id);
        }

        [Fact]
        public void ShouldGetAccountWhenSocialAccountIsNotFound()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns<SocialAccount>(null);
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            Action action = () => TwitterAccountAppService.GetAccount(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldDeleteAccountAsync()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns(
                new SocialAccount { Id = 1 });
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            TwitterAccountAppService.DeleteAccountAsync(1);
            //Assert
            socialAccountService.Verify(t => t.DeleteAsync(It.Is<SocialAccount>(r => r.Id == 1)));
        }

        [Fact]
        public void ShouldDeleteAccountAsyncWhenSocialAccountIsNotFound()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns<SocialAccount>(null);
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            Func<Task> action = () => TwitterAccountAppService.DeleteAccountAsync(1);
            //Assert
            Assert.ThrowsAsync<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldUpdateAccount()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns(
                new SocialAccount { Id = 1 });
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            TwitterAccountDto twitterAccountDto = TwitterAccountAppService.UpdateAccount(1, new UpdateTwitterAccountDto { IfEnable = true });
            //Assert
            Assert.NotNull(twitterAccountDto);
            socialAccountService.Verify(t => t.Update(It.Is<SocialAccount>(r => r.Id == 1 && r.IfEnable == true)));
            Assert.Equal(1, twitterAccountDto.Id);
            Assert.Equal(true, twitterAccountDto.IfEnable);
        }

        [Fact]
        public void ShouldUpdateAccountWhenSocialAccountIsNotFound()
        {
            //Arrange
            var socialAccountService = new Mock<ISocialAccountService>();

            socialAccountService.Setup(t => t.FindAccount(1, SocialUserSource.Twitter)).Returns<SocialAccount>(null);
            TwitterAccountAppService TwitterAccountAppService = new TwitterAccountAppService(socialAccountService.Object, null);
            //Act
            Action action = () => TwitterAccountAppService.UpdateAccount(1, new UpdateTwitterAccountDto { IfEnable = true });
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }
    }
}
