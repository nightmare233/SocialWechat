using Framework.Core;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public class ExpressionBuildOptions
    {
        private IDependencyResolver _dependencyResolver;

        public ExpressionBuildOptions()
        {
        }

        public ExpressionBuildOptions(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }


        public int UserId { get; set; }
        public int[] MyDepartmentMembers { get; set; }
        public int[] MyDepartments { get; set; }
        public IList<DepartmentStatus> DepartmentStatuses { get; set; }


        public int GetUserId()
        {
            if (UserId > 0)
            {
                return UserId;
            }
            else
            {
                Checker.NotNull(_dependencyResolver, nameof(_dependencyResolver));
                var userContext = _dependencyResolver.Resolve<IUserContext>();
                return userContext.UserId;
            }
        }

        public int[] GetMyDepartmentMembers()
        {
            if (MyDepartmentMembers != null)
            {
                return MyDepartmentMembers;
            }
            else
            {
                Checker.NotNull(_dependencyResolver, nameof(_dependencyResolver));
                var departmentService = _dependencyResolver.Resolve<IDepartmentService>();
                return departmentService.GetMyDepartmentMembers(GetUserId());
            }
        }

        public int[] GetMyDepartments()
        {
            if (MyDepartments != null)
            {
                return MyDepartments;
            }
            else
            {
                Checker.NotNull(_dependencyResolver, nameof(_dependencyResolver));
                var departmentService = _dependencyResolver.Resolve<IDepartmentService>();
                return departmentService.GetMyDepartmentIds(GetUserId());
            }
        }
    }
}
