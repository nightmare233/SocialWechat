using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using Social.Domain.Repositories;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Social.Application.AppServices
{
    public interface IConversationAppService
    {
        ConversationDto Find(int id);
        IList<ConversationDto> Find(ConversationSearchDto searchDto);
        ConversationDto Insert(ConversationCreateDto createDto);
        void Delete(int id);
        Task<ConversationDto> UpdateAsync(int id, ConversationUpdateDto updateDto);
        IList<ConversationLogDto> GetLogs(int converationId);
        IList<ConversationLogDto> GetNewLogs(int conversationId, int oldMaxLogId);
        Task<ConversationDto> TakeAsync(int conversationId);
        Task<ConversationDto> CloseAsync(int conversationId);
        Task<ConversationDto> ReopenAsync(int conversationId);
        Task<ConversationDto> MarkAsReadAsync(int conversationId);
        Task<ConversationDto> MarkAsUnReadAsync(int conversationId);
        int GetUnReadConversationCount();
        bool IfCanReopen(int conversationId);
    }

    public class ConversationAppService : AppService, IConversationAppService
    {
        private IConversationService _conversationService;
        private IMessageService _messageService;

        private IAgentService _agentService;
        private IDepartmentService _departmentService;
        private IDomainService<ConversationLog> _logService;
        private INotificationManager _notificationManager;
        private IConfigRepository _configRepository;

        public ConversationAppService(
            IConversationService conversationService,
            IMessageService messageService,
            IAgentService agentService,
            IDepartmentService departmentService,
            IDomainService<ConversationLog> logService,
            INotificationManager notificationManager,
            IConfigRepository configRepository
            )
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _agentService = agentService;
            _departmentService = departmentService;
            _logService = logService;
            _notificationManager = notificationManager;
            _configRepository = configRepository;
        }

        public IList<ConversationDto> Find(ConversationSearchDto dto)
        {
            //if (dto.Since == null && dto.Until == null)
            //{
            //    dto.Until = DateTime.UtcNow;
            //    dto.Since = DateTime.UtcNow.AddMonths(-3);
            //}

            if (dto.MaxNumberOfDataRetrieve <= 0)
            {
                dto.MaxNumberOfDataRetrieve = 100;
            }

            DateTime lastMessageLessThan = DateTime.MinValue;
            if (dto.LastMessageSendTimeLessThan.HasValue)
            {
                lastMessageLessThan = DateTimeExtensions.FromUnixTimeSeconds(dto.LastMessageSendTimeLessThan.Value);
            }

            var conversations = _conversationService.FindAll().Where(t => t.IsHidden == false);
            conversations = conversations.WhereIf(dto.SinceId != null, t => t.Id > dto.SinceId);
            conversations = conversations.WhereIf(dto.MaxId != null, t => t.Id <= dto.MaxId);
            conversations = conversations.WhereIf(dto.LastMessageSendTimeLessThan != null, t => t.LastMessageSentTime < lastMessageLessThan);
            conversations = _conversationService.ApplyFilter(conversations, dto.FilterId);
            conversations = _conversationService.ApplyKeyword(conversations, dto.Keyword);
            conversations = _conversationService.ApplySenderOrReceiverId(conversations, dto.UserId);

            List<ConversationDto> conversationDtos = conversations.
                OrderByDescending(t => t.LastMessageSentTime)
                .Take(dto.MaxNumberOfDataRetrieve)
                .ProjectTo<ConversationDto>().ToList();

            FillFiledsForDtoList(conversationDtos);

            return conversationDtos;
        }

        private void FillFiledsForDtoList(IList<ConversationDto> conversationDtos)
        {
            var allMessages = _messageService.FindAllByConversationIds(conversationDtos.Select(t => t.Id).ToArray()).ToList();
            int siteId = CurrentUnitOfWork.GetSiteId().HasValue ? CurrentUnitOfWork.GetSiteId().Value : -1;
            bool? ifDepartmentEnable = true;
            UnitOfWorkManager.RunWithNewTransaction(null, () =>
            {
                ifDepartmentEnable = _configRepository.FindAll().Where(t => t.Id == siteId).FirstOrDefault()?.IfDepartmentEnable;
            });
            if (ifDepartmentEnable.GetValueOrDefault() == false)
            {
                foreach (var conversationDto in conversationDtos)
                {
                    conversationDto.DepartmentId = null;
                    conversationDto.DepartmentName = null;
                }
            }
            foreach (var conversationDto in conversationDtos)
            {
                var messages = allMessages.Where(t => t.ConversationId == conversationDto.Id).ToList();
                FillFields(conversationDto, messages);
            }
        }

        public ConversationDto Find(int id)
        {
            var conversation = _conversationService.CheckIfExists(id);
            var conversationDto = Mapper.Map<ConversationDto>(conversation);
            FillFields(conversationDto);
            return conversationDto;

        }

        public int GetUnReadConversationCount()
        {
            var filters = DependencyResolver.Resolve<IFilterService>().FindFiltersInlucdeConditions(UserContext.UserId).ToList();
            return _conversationService.GetUnReadConversationCount(filters);
        }

        public ConversationDto Insert(ConversationCreateDto createDto)
        {
            var conversation = Mapper.Map<Conversation>(createDto);
            conversation = _conversationService.Insert(conversation);
            CurrentUnitOfWork.SaveChanges();
            var conversationDto = Mapper.Map<ConversationDto>(conversation);
            FillFields(conversationDto);
            return conversationDto;
        }

        public void Delete(int id)
        {
            var conversation = _conversationService.CheckIfExists(id);
            _conversationService.Delete(conversation);
        }

        public async Task<ConversationDto> UpdateAsync(int id, ConversationUpdateDto updateDto)
        {
            ConversationDto conversationDto;
            int oldMaxLogId;
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                Conversation conversation = _conversationService.CheckIfExists(id);
                oldMaxLogId = conversation.Logs.Select(t => t.Id).DefaultIfEmpty().Max();
                Mapper.Map(updateDto, conversation);
                _conversationService.UpdateAndWriteLog(conversation);
                conversationDto = Mapper.Map<ConversationDto>(conversation);
                FillFields(conversationDto);
                uow.Complete();
            }
            await _notificationManager.NotifyUpdateConversation(CurrentUnitOfWork.GetSiteId().GetValueOrDefault(), id, oldMaxLogId);
            return conversationDto;
        }

        public IList<ConversationLogDto> GetLogs(int converationId)
        {
            var logs = _logService.FindAll()
                .Where(t => t.ConversationId == converationId)
                .ProjectTo<ConversationLogDto>()
                .ToList();

            return logs.OrderByDescending(t => t.CreatedTime).ToList();
        }

        public IList<ConversationLogDto> GetNewLogs(int conversationId, int oldMaxLogId)
        {
            return _logService.FindAll().Where(t => t.ConversationId == conversationId && t.Id > oldMaxLogId)
                .ProjectTo<ConversationLogDto>()
                .ToList();
        }

        public async Task<ConversationDto> TakeAsync(int conversationId)
        {
            return await UpdateConversation(conversationId, _conversationService.Take);
        }

        public async Task<ConversationDto> CloseAsync(int conversationId)
        {
            return await UpdateConversation(conversationId, _conversationService.Close);
        }

        public async Task<ConversationDto> ReopenAsync(int conversationId)
        {
            return await UpdateConversation(conversationId, _conversationService.Reopen);
        }

        public bool IfCanReopen(int conversationId)
        {
            var conversation = _conversationService.CheckIfExists(conversationId);
            if (conversation.Status != ConversationStatus.Closed)
            {
                return false;
            }

            return _conversationService.IfCanReopen(conversation);
        }

        public async Task<ConversationDto> MarkAsReadAsync(int conversationId)
        {
            return await UpdateConversation(conversationId, _conversationService.MarkAsRead);
        }

        public async Task<ConversationDto> MarkAsUnReadAsync(int conversationId)
        {
            return await UpdateConversation(conversationId, _conversationService.MarkAsUnRead);
        }

        private async Task<ConversationDto> UpdateConversation(int conversationId, Func<Conversation, Conversation> updateFunc)
        {
            int maxLogId;
            ConversationDto conversationDto;
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var conversation = _conversationService.CheckIfExists(conversationId);
                maxLogId = conversation.Logs.Select(t => t.Id).DefaultIfEmpty().Max();
                var entity = updateFunc(conversation);

                conversationDto = Mapper.Map<ConversationDto>(entity);
                FillFields(conversationDto);
                uow.Complete();
            }
            await _notificationManager.NotifyUpdateConversation(CurrentUnitOfWork.GetSiteId().GetValueOrDefault(), conversationId, maxLogId);
            return conversationDto;
        }

        private void FillFields(ConversationDto conversationDto)
        {
            var messages = _messageService
                .FindAllByConversationId(conversationDto.Id).ToList();

            FillFields(conversationDto, messages);
        }

        private void FillFields(ConversationDto dto, IList<Message> messages)
        {
            if (dto == null)
            {
                return;
            }

            if (messages != null && messages.Any())
            {
                messages = messages.OrderByDescending(t => t.Id).ToList();
                dto.LastMessage = messages.First().Content;
                dto.OriginalLink = messages.Last().OriginalLink;

                var lastMessageSendByCustomer = messages.FirstOrDefault(t => t.Sender.IsCustomer);
                if (lastMessageSendByCustomer != null)
                {
                    dto.CustomerId = lastMessageSendByCustomer.SenderId;
                    dto.CustomerName = lastMessageSendByCustomer.Sender.ScreenNameOrNormalName;
                    dto.CustomerAvatar = lastMessageSendByCustomer.Sender.Avatar;
                }

                var lastMessageByIntegrationAccount = messages.FirstOrDefault(t => t.IntegrationAccount != null);
                if (lastMessageByIntegrationAccount != null)
                {
                    dto.LastIntegrationAccountId = lastMessageByIntegrationAccount.IntegrationAccountId;
                    dto.LastIntegrationAccountName = lastMessageByIntegrationAccount.IntegrationAccount.ScreenNameOrNormalName;
                    dto.LastIntegrationAccountAvatar = lastMessageByIntegrationAccount.IntegrationAccount.Avatar;
                }
            }
        }
    }
}
