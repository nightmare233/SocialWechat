using Social.Domain.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Social.Infrastructure;
using Social.Infrastructure.Facebook;
using Framework.Core.UnitOfWork;
using System.Transactions;
using Framework.Core;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.DomainServices.Twitter;

namespace Social.UnitTest
{
    public class DependencyResolverBuilder
    {
        private IConversationService _conversationService = new Mock<IConversationService>().Object;
        private IMessageService _messageService = new Mock<IMessageService>().Object;
        private INotificationManager _notificationManager = new Mock<INotificationManager>().Object;
        private ISocialUserService _socialUserService = new Mock<ISocialUserService>().Object;
        private IFbClient _fbClient = new Mock<IFbClient>().Object;
        private IUserContext _userContext = new Mock<IUserContext>().Object;
        private IUnitOfWorkManager _uowMnager;

        public DependencyResolverBuilder()
        {
            _uowMnager = MockUnitOfWorkManager().Object;
        }

        public DependencyResolverBuilder WithConversationService(IConversationService service)
        {
            _conversationService = service;
            return this;
        }

        public DependencyResolverBuilder WithMessageService(IMessageService service)
        {
            _messageService = service;
            return this;
        }

        public DependencyResolverBuilder WithSocialUserService(ISocialUserService service)
        {
            _socialUserService = service;
            return this;
        }

        public DependencyResolverBuilder WithFacebookClient(IFbClient service)
        {
            _fbClient = service;
            return this;
        }


        public IDependencyResolver Build()
        {
            var dependencyResolver = new DependencyResolver();
            dependencyResolver.RegisterTransient(_conversationService);
            dependencyResolver.RegisterTransient(_messageService);
            dependencyResolver.RegisterTransient(_socialUserService);
            dependencyResolver.RegisterTransient(_notificationManager);
            dependencyResolver.RegisterTransient(_fbClient);
            dependencyResolver.RegisterTransient(MockUnitOfWorkManager().Object);
            dependencyResolver.RegisterTransient(_userContext);

            dependencyResolver.RegisterTransient<PullJobService>();
            dependencyResolver.RegisterTransient<TwitterDirectMessageService>();

            return dependencyResolver;
        }

        public static Mock<IUnitOfWorkManager> MockUnitOfWorkManager()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            uowManagerMock.Setup(t => t.Begin()).Returns(uowMock.Object);
            uowManagerMock.Setup(t => t.Begin(It.IsAny<TransactionScopeOption>())).Returns(uowMock.Object);
            return uowManagerMock;
        }
    }
}
