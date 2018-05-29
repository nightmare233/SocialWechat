using Framework.Core;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IFilterConditonAppService
    {
      //  List<FilterConditonDto> FindAll();
    }
    class FilterConditonAppService//: IFilterAppService,AppService
    {
        private IDomainService<FilterCondition> _domainForConditonService;
        //public List<FilterConditonDto> FindAll()
        //{
        //    return _domainForConditonService.FindAll();
        //}
    }
}
