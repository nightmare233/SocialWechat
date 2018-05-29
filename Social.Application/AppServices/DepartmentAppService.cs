using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IDepartmentAppService
    {
        List<DepartmentDto> FindAll();
        DepartmentDto Find(int id);
    }

    public class DepartmentAppService : AppService, IDepartmentAppService
    {
        private IDepartmentService _departmentService;
        public DepartmentAppService(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public List<DepartmentDto> FindAll()
        {
            return _departmentService.FindAll().Select(t => Mapper.Map<DepartmentDto>(t)).ToList();
        }

        public DepartmentDto Find(int id)
        {
            return Mapper.Map<DepartmentDto>(_departmentService.Find(id));
        }

    }
}
