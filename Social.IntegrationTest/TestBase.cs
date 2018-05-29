using Framework.Core;
using Framework.Core.UnitOfWork;
using Social.Application;
using Social.Domain.Core;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using Social.Infrastructure.Enum;
using System;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Social.IntegrationTest
{
    public class TestBase : IDisposable
    {
        protected static DependencyResolver DependencyResolver;
        protected SocialAccount TestFacebookAccount;
        protected SocialAccount TestTwitterAccount;
        protected IUnitOfWorkManager UnitOfWorkManager { get; set; }
        protected IUnitOfWork CurrentUnitOfWork { get { return UnitOfWorkManager.Current; } }

        static TestBase()
        {
            DependencyResolver = new DependencyResolver();
            DependencyResolver.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            new ApplicationServicesRegistrar(DependencyResolver).RegisterServices();
        }

        private IUnitOfWork _unitOfWork;

        public TestBase()
        {
            UnitOfWorkManager = DependencyResolver.Resolve<IUnitOfWorkManager>();
            _unitOfWork = UnitOfWorkManager.Begin() as IUnitOfWork;
            CreateTestFacebookAccount();
            CreateTestTwitterAccount();
        }

        private void CreateTestTwitterAccount()
        {
            string testUserId = "855320911989194753";
            int testSiteId = 10000;

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (CurrentUnitOfWork.SetSiteId(testSiteId))
                {
                    var socailUserRepo = DependencyResolver.Resolve<IRepository<SocialUser>>();
                    TestTwitterAccount = socailUserRepo.FindAll().Where(t => t.OriginalId == testUserId).Select(t => t.SocialAccount).FirstOrDefault();

                    if (TestTwitterAccount == null)
                    {
                        SocialUser socialUser = new SocialUser
                        {
                            Name = "Twitter Test User",
                            OriginalId = testUserId,
                            Source = SocialUserSource.Twitter,
                            Type = SocialUserType.IntegrationAccount,
                            SocialAccount = new SocialAccount
                            {
                                Token = "855320911989194753-25EU8AmKqJw8HhPJYdCUcje2mat9UxV",
                                TokenSecret = "HQYSviXLSEFHZkF2xqj8R9KxWRtIHG3Tp4yBdjpEutUa3",
                                IfConvertMessageToConversation = true,
                                IfConvertTweetToConversation = true,
                                IfEnable = true,
                            }
                        };

                        socailUserRepo.Insert(socialUser);
                        TestTwitterAccount = socialUser.SocialAccount;
                    }

                    uow.Complete();
                }
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (CurrentUnitOfWork.SetSiteId(null))
                {
                    var siteSocialAccountRepo = DependencyResolver.Resolve<ISiteSocialAccountRepository>();
                    siteSocialAccountRepo.Insert(new SiteSocialAccount
                    {
                        TwitterUserId = testUserId,
                        SiteId = testSiteId
                    });

                    uow.Complete();
                }
            }
        }

        private void CreateTestFacebookAccount()
        {
            string testPageId = "1974003879498745";
            int testSiteId = 10000;

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (CurrentUnitOfWork.SetSiteId(testSiteId))
                {
                    var socailUserRepo = DependencyResolver.Resolve<IRepository<SocialUser>>();
                    TestFacebookAccount = socailUserRepo.FindAll().Where(t => t.OriginalId == testPageId).Select(t => t.SocialAccount).FirstOrDefault();

                    if (TestFacebookAccount == null)
                    {
                        SocialUser socialUser = new SocialUser
                        {
                            Name = "Facebook Test Page",
                            OriginalId = testPageId,
                            Source = SocialUserSource.Facebook,
                            Type = SocialUserType.IntegrationAccount,
                            SocialAccount = new SocialAccount
                            {
                                Token = "EAAR8yzs1uVQBAEBWQbsXb8HBP7cEbkTZB7CuqvuQlU1lx0ZCmlZCoy25HsxahMcCGfi8PirSyv5ZA62rvnm21EdZC3PZBK4FXfSti6cc8zIPKMb06fdR15sJqteOW2cIzTV64ZBZBZAnDLBwkNvYszc497CafdqAZCNRaip8w5SjmZCBwZDZD",
                                IfConvertMessageToConversation = true,
                                IfConvertVisitorPostToConversation = true,
                                IfConvertWallPostToConversation = true,
                                IfEnable = true,
                            }
                        };

                        socailUserRepo.Insert(socialUser);
                        TestFacebookAccount = socialUser.SocialAccount;
                    }

                    uow.Complete();
                }
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (CurrentUnitOfWork.SetSiteId(null))
                {
                    var siteSocialAccountRepo = DependencyResolver.Resolve<ISiteSocialAccountRepository>();
                    siteSocialAccountRepo.Insert(new SiteSocialAccount
                    {
                        FacebookPageId = testPageId,
                        SiteId = testSiteId
                    });

                    uow.Complete();
                }
            }
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
