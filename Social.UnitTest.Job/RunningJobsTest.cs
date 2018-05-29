using Moq;
using Quartz;
using Social.Domain.Entities;
using Social.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.Job
{
    public class RunningJobsTest
    {
        [Fact]
        public void ShouldScheduleJob()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);

            // Assert
            scheduleManagerMock.Verify(t => t.ScheduleAsync<TestJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), configuerTriggerAction, siteSocialAccount));
            Assert.True(runningJobs.IsRunning<TestJob>(10000, "123"));
        }

        [Fact]
        public void ShouldNotScheduleJobIfJobIsRunning()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);

            // Assert
            scheduleManagerMock.Verify(mock => mock.ScheduleAsync<TestJob, SiteSocialAccount>(It.IsAny<Action<JobBuilder>>(), configuerTriggerAction, siteSocialAccount), Times.Once);
        }

        [Fact]
        public void ShouldUpdateLastScheduleTime()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            DateTime firstScheduleTime = runningJobs.Get<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId).LastScheduleTime;
            Thread.Sleep(100);
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            DateTime secondScheduleTime = runningJobs.Get<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId).LastScheduleTime;

            // Assert
            Assert.NotEqual(firstScheduleTime, secondScheduleTime);
            Assert.True(secondScheduleTime > firstScheduleTime);
        }

        [Fact]
        public void ShouldGetRunningJob()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            var runningJob = runningJobs.Get<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId);

            // Assert
            Assert.NotNull(runningJob);
            Assert.Equal(10000, runningJob.SiteId);
            Assert.Equal("123", runningJob.OriginalAccountId);
            Assert.Equal("TestJob - SiteId(10000) - OriginalId(123)", runningJob.JobKey);
            Assert.Equal("TestJob", runningJob.JobGroup);
        }

        [Fact]
        public void ShouldRemoveRunningJob()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            runningJobs.Remove<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId);
            var runningJob = runningJobs.Get<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId);

            // Assert
            Assert.Null(runningJob);
        }

        [Fact]
        public void ShouldStopTimeoutRunningJobs()
        {
            // Arrange
            RunningJobs runningJobs = new RunningJobs();
            var scheduleManagerMock = new Mock<IScheduleJobManager>();
            var siteSocialAccount = new SiteSocialAccount { SiteId = 10000, FacebookPageId = "123" };
            Action<TriggerBuilder> configuerTriggerAction = t => { };
            var schedulerMock = new Mock<IScheduler>();
            schedulerMock.Setup(t => t.GetJobDetail(It.IsAny<JobKey>())).Returns(new Mock<IJobDetail>().Object);

            // Act
            runningJobs.Schedule<TestJob>(scheduleManagerMock.Object, siteSocialAccount, configuerTriggerAction);
            var runningJob = runningJobs.Get<TestJob>(siteSocialAccount.SiteId, siteSocialAccount.FacebookPageId);
            runningJob.LastScheduleTime = DateTime.UtcNow.AddSeconds(-301);
            runningJobs.StopTimeoutJobs(schedulerMock.Object);

            // Assert
            schedulerMock.Verify(t => t.DeleteJob(It.Is<JobKey>(r => r.Name == "TestJob - SiteId(10000) - OriginalId(123)")));
            Assert.False(runningJobs.IsRunning<TestJob>(10000, "123"));

        }

        public class TestJob : JobBase
        {
            protected override Task ExecuteJob(IJobExecutionContext context)
            {
                return Task.FromResult<object>(null);
            }
        }
    }
}
