using Framework.Core;
using Framework.Core.UnitOfWork;
using Moq;
using Quartz;
using Social.Domain.Core;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Job;
using Social.Job.Jobs;
using Social.Job.Jobs.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.Job
{
    public class SchedulerJobTest
    {
        [Fact]
        public void ShouldSchduleFacebookJobs()
        {
            // Arrange
            var schedulerJobManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccountServiceMock = new Mock<ISiteSocialAccountService>();
            var facebookAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            siteSocialAccountServiceMock.Setup(t => t.GetFacebookSiteAccountsAsync()).ReturnsAsync(new List<SiteSocialAccount>
            {
                facebookAccount
            });

            SchedulerJob schedulerJob = new SchedulerJob(schedulerJobManagerMock.Object, siteSocialAccountServiceMock.Object);

            var jobExecutionContextMock = new Mock<IJobExecutionContext>();
            var jobDetailMock = new Mock<IJobDetail>();
            jobDetailMock.Setup(t => t.Key).Returns(new JobKey("TestJobKey"));
            jobExecutionContextMock.Setup(t => t.JobDetail).Returns(jobDetailMock.Object);

            // Act
            schedulerJob.Execute(jobExecutionContextMock.Object);

            // Assert
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<PullMessagesJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), facebookAccount), "Should schedule facebook pull message job.");
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<PullTaggedVisitorPostsJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), facebookAccount), "Should schedule facebook pull tagged visitor post job.");
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<PullVisitorPostsFromFeedJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), facebookAccount), "Should schedule facebook pull feed post job.");
        }

        [Fact]
        public void ShouldSchduleTwitterJobs()
        {
            // Arrange
            var schedulerJobManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccountServiceMock = new Mock<ISiteSocialAccountService>();
            var twitterAccount = new SiteSocialAccount { SiteId = 10000, TwitterUserId = "abc" };
            siteSocialAccountServiceMock.Setup(t => t.GetTwitterSiteAccountsAsync()).ReturnsAsync(new List<SiteSocialAccount>
            {
                twitterAccount
            });

            SchedulerJob schedulerJob = new SchedulerJob(schedulerJobManagerMock.Object, siteSocialAccountServiceMock.Object);

            var jobExecutionContextMock = new Mock<IJobExecutionContext>();
            var jobDetailMock = new Mock<IJobDetail>();
            jobDetailMock.Setup(t => t.Key).Returns(new JobKey("TestJobKey"));
            jobExecutionContextMock.Setup(t => t.JobDetail).Returns(jobDetailMock.Object);

            // Act
            schedulerJob.Execute(jobExecutionContextMock.Object);

            // Assert
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<TwitterUserStreamJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), twitterAccount), "Should schedule twitter user stream job.");
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<TwitterPullTweetsJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), twitterAccount), "Should schedule twitter pull tweets job.");
            schedulerJobManagerMock.Verify(t => t.ScheduleAsync<TwitterPullDirectMessagesJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), It.IsAny<Action<TriggerBuilder>>(), twitterAccount), "Should schedule facebook pull direct message job.");
        }
    }
}
