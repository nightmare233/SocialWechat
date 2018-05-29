using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.UnitTest.DomainServices.Facebook
{
    public class PullJobServiceTestBase
    {
        protected FbPagingData<FbPost> MakeSinglePostPagingData()
        {
            return new FbPagingData<FbPost>
            {
                data = new List<FbPost>
                {
                    new FbPost{
                        id ="post_1",
                        from = new FbUser{id="user_1"},
                        created_time =new DateTime(2000,1,1,1,1,1,DateTimeKind.Utc),
                        message="test_content"
                        }
                }
            };
        }

        protected FbPagingData<FbPost> MakePostWithCommentsPaingData()
        {
            var post = MakeSinglePostPagingData();
            post.data.First().comments = new FbPagingData<FbComment>
            {
                data = new List<FbComment>
                {
                    new FbComment
                    {
                        id="comment_1",
                        PostId="post_1",
                        from = new FbUser{id="user_1"},
                        message = "test_comment_content1",
                        created_time = new DateTime(2000,1,2,1,1,1,DateTimeKind.Utc)
                    }
                }
            };

            return post;
        }

        protected FbPagingData<FbPost> MakePostWithReplyCommentsPaingData()
        {
            var postWithComments = MakePostWithCommentsPaingData();
            // set reply comments.
            postWithComments.data.First().comments.data.First().comments =
                new FbPagingData<FbComment>
                {
                    data = new List<FbComment>
                    {
                        new FbComment
                        {
                            id="reply_comment_1",
                            PostId="post_1",
                            from = new FbUser{id="user_1"},
                            message = "test_reply_comment_content1",
                            created_time = new DateTime(2000,1,2,1,1,1,DateTimeKind.Utc),
                            parent = postWithComments.data.First().comments.data.First()
                        }
                    }
                };

            return postWithComments;
        }

        protected Mock<ISocialUserService> MockSocialUserService(SocialAccount account)
        {
            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateSocialUsers(account.Token, It.IsAny<List<FbUser>>()))
                .ReturnsAsync(
                    new List<SocialUser> {
                        new SocialUser { Id = 1, OriginalId = "user_1" }
                    }
                );

            return socialUserServiceMock;
        }

        protected Mock<IFbClient> MockFbClient(string pageId, string token, FbPagingData<FbPost> data)
        {
            var fbClient = new Mock<IFbClient>();
            fbClient.Setup(t => t.GetTaggedVisitorPosts(pageId, token)).ReturnsAsync(data);
            fbClient.Setup(t => t.GetVisitorPosts(pageId, token)).ReturnsAsync(data);
            return fbClient;
        }

        protected SocialAccount MakeSocialAccount()
        {
            return new SocialAccount
            {
                Id = 888,
                Token = "test_token",
                SocialUser = new SocialUser
                {
                    Id = 888,
                    Source = SocialUserSource.Facebook,
                    Type = SocialUserType.IntegrationAccount,
                    OriginalId = "test_page_id"
                },
                IfConvertVisitorPostToConversation = true,
                IfConvertWallPostToConversation = true
            };
        }
    }
}
