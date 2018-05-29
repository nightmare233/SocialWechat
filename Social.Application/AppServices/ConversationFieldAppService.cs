using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IConversationFieldAppService
    {
        List<ConversationFieldDto> FindAll();
        ConversationFieldDto Find(int id);
    }
    public class ConversationFieldAppService : AppService, IConversationFieldAppService
    {
        private IConversationFieldService _domainService;

        public ConversationFieldAppService(IConversationFieldService domainService)
        {
            _domainService = domainService;
        }

        public List<ConversationFieldDto> FindAll()
        {
            return _domainService.FindAllAndFillOptions().Select(t => Mapper.Map<ConversationFieldDto>(t)).ToList();
        }

        public ConversationFieldDto Find(int id)
        {
            return Mapper.Map<ConversationFieldDto>(_domainService.Find(id));
        }
    }
}
