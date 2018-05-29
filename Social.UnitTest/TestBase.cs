using AutoMapper;
using Framework.Core.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Social.UnitTest
{
    public class TestBase
    {
        static TestBase()
        {
            Mapper.Initialize(cfg => cfg.AddProfiles(new[] { "Social.Application" }));
        }

        protected IUnitOfWorkManager MakeFakeUnitOfWorkManager()
        {
            var uowManager = new Mock<IUnitOfWorkManager>();
            var uow = new Mock<IUnitOfWork>();
            uowManager.Setup(t => t.Current).Returns(uow.Object);
            uowManager.Setup(t => t.Begin(It.IsAny<TransactionScopeOption>())).Returns(uow.Object);

            return uowManager.Object;
        }
    }
}
