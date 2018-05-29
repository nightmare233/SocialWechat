using Framework.Core;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Social.Domain.DomainServices
{
    public interface IDepartmentService
    {
        string GetDisplayName(int? departmentId);
        int[] GetMyDepartmentIds(int userId);
        int[] GetMyDepartmentMembers(int userId);
        int[] GetMatchedStatusDepartments(int status);
        IList<Department> FindAll();
        Department Find(int id);
        IList<Department> Find(IEnumerable<int> ids);
    }

    public class DepartmentService : ServiceBase, ITransient, IDepartmentService
    {
        private IRepository<Department> _departmentRepo;
        private ICpanelConfigOptionRepositiory _configRepo;

        public DepartmentService(IRepository<Department> departmentRepo,
            ICpanelConfigOptionRepositiory configRepo)
        {
            _departmentRepo = departmentRepo;
            _configRepo = configRepo;
        }

        private IQueryable<Department> FindAllUnDeleted()
        {
            //return new List<Department>
            //{
            //    new Department{Id=1,Name="Test"}
            //}.AsQueryable();

            return _departmentRepo.FindAll().Where(t => t.IfDeleted == false);
        }
        public IList<Department> FindAll()
        {
            var departments = FindAllUnDeleted().ToList();
            return departments;
        }

        public Department Find(int id)
        {
            return FindAllUnDeleted().FirstOrDefault(t => t.Id == id);
        }

        public IList<Department> Find(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<Department>();
            }

            return FindAllUnDeleted().Where(t => ids.Contains(t.Id)).ToList();
        }


        public string GetDisplayName(int? departmentId)
        {
            if (departmentId == null)
            {
                return "N/A";
            }

            var department = Find(departmentId.Value);
            return department == null ? "N/A" : department.Name;
        }

        public int[] GetMyDepartmentIds(int userId)
        {
            var departmentIds = FindAllUnDeleted()
                 .Where(t => t.Members.Any(m => m.RelatedId == userId && m.Type == 0))
                 .Select(t => t.Id);

            return departmentIds.ToArray();
        }

        public int[] GetMyDepartmentMembers(int userId)
        {
            return FindAllUnDeleted()
                .Where(t => t.Members.Any(m => m.RelatedId == userId && m.Type == 0))
                .SelectMany(t => t.Members)
                .Where(t => t.RelatedId != userId)
                .Select(t => t.RelatedId)
                .Distinct().
                ToArray();
        }

        public int[] GetOfflineMembers(int[] agents)
        {
            return new int[] { };
        }

        public int[] GetMatchedStatusDepartments(int status)
        {
            return new int[] { };
        }
    }
}
