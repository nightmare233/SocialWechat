using Social.Application.AppServices;
using Social.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/departments")]
    public class DepartmentController : ApiController
    {
        private IDepartmentAppService _DepartmentService;

        public DepartmentController(IDepartmentAppService DepartmentService)
        {
            _DepartmentService = DepartmentService;
        }

        [Route()]
        public List<DepartmentDto> GetDepartments()
        {
            return _DepartmentService.FindAll();
        }

        [Route("{id}", Name = "GetDepartment")]
        public DepartmentDto GetDepartment(int id)
        {
            return _DepartmentService.Find(id);
        }
    }
}