using Framework.Core;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain
{
    public interface IFilterService : IDomainService<Filter>
    {
        Filter FindFilterInlucdeConditions(int id);
        IQueryable<Filter> FindAllFiltersInlucdeConditions();
        IQueryable<Filter> FindFiltersInlucdeConditions(int userId);
        void UpdateFilter(Filter filter, FilterCondition[] contiditons);
        void DeleteConditons(Filter updateFilter);
        int GetConversationNum(Filter filter);
        void CheckFieldIdExist(List<FilterCondition> filterConditons);
        void CheckFieldValue(List<FilterCondition> filterConditons);
        bool HasConversation(Filter filter, int conversationId);
    }
    public class FilterService : DomainService<Filter>, IFilterService
    {
        private IRepository<Filter> _filterRepo;
        private IRepository<FilterCondition> _filterConditionRepo;
        private IRepository<ConversationField> _conversationFieldRepo;
        private IRepository<Conversation> _conversationService;
        private IRepository<SocialUser> _userRepo;
        private IConversationFieldService _conversationFieldOptionService;
        private IConversationService _conversation;
        private IConfigRepository _configRepository;

        public FilterService(
            IRepository<FilterCondition> filterConditionRepo,
            IRepository<Filter> filterRepo,
            IRepository<Conversation> conversationService,
            IRepository<SocialUser> userRepo, IConversationService conversation,
            IRepository<ConversationField> conversationFieldRepo,
            IConversationFieldService conversationFieldOptionService,
            IConfigRepository configRepository)
        {
            _filterConditionRepo = filterConditionRepo;
            _filterRepo = filterRepo;
            _conversationService = conversationService;
            _conversation = conversation;
            _userRepo = userRepo;
            _conversationFieldRepo = conversationFieldRepo;
            _conversationFieldOptionService = conversationFieldOptionService;
            _configRepository = configRepository;
        }

        public Filter FindFilterInlucdeConditions(int id)
        {
            return this.FindAll().Include(t => t.Conditions.Select(r => r.Field)).FirstOrDefault(t => t.Id == id);
        }

        public IQueryable<Filter> FindAllFiltersInlucdeConditions()
        {
            return this.FindAll().Include(t => t.Conditions.Select(r => r.Field));
        }

        public IQueryable<Filter> FindFiltersInlucdeConditions(int userId)
        {
            var filters = this.FindAll()
                .Include(t => t.Conditions.Select(r => r.Field))
                .Where(t => t.IfPublic || t.CreatedBy == userId).ToList();
            int siteId = CurrentUnitOfWork.GetSiteId().HasValue ? CurrentUnitOfWork.GetSiteId().Value : -1;
            bool ifDepartmentEnable = false;
            UnitOfWorkManager.RunWithNewTransaction(null, () =>
            {
                ifDepartmentEnable = _configRepository.FindAll().Where(t => t.Id == siteId).First().IfDepartmentEnable;
            });
            if (!ifDepartmentEnable)
            {
                filters.RemoveAll(t => t.Name.Contains("Department"));
            }
            return filters.AsQueryable();
        }

        public void DeleteConditons(Filter updateFilter)
        {
            if (updateFilter.Conditions.Count() > 0)
            {
                List<FilterCondition> filterConditons = new List<FilterCondition>();
                filterConditons = updateFilter.Conditions.ToList();
                _filterConditionRepo.DeleteMany(filterConditons.ToArray());
            }
        }

        public void UpdateFilter(Filter filter, FilterCondition[] contiditons)
        {
            CheckFieldIdExist(contiditons.ToList());
            CheckFieldValue(contiditons.ToList());
            foreach (var condition in contiditons)
            {
                _filterConditionRepo.Insert(filter.Conditions[0]);
            }
            _filterRepo.Update(filter);

        }

        public bool HasConversation(Filter filter, int conversationId)
        {
            return _conversation.FindAll(filter).Where(t => t.Id == conversationId).Any();
        }

        public int GetConversationNum(Filter filter)
        {
            return _conversation.FindAll(filter).Count();
        }

        public void CheckFieldIdExist(List<FilterCondition> filterConditons)
        {
            List<int> fieldIds = new List<int>();

            foreach (var filterCondition in filterConditons)
            {
                fieldIds.Add(filterCondition.FieldId);
            }
            fieldIds.RemoveAll(a => _conversationFieldRepo.FindAll().Where(t => fieldIds.Contains(t.Id)).Select(t => t.Id).ToList().Contains(a));
            if (fieldIds.Count != 0)
            {
                throw SocialExceptions.BadRequest($"FieldId '{fieldIds[0]}' not exists");
            }

        }

        public void CheckFieldValue(List<FilterCondition> filterConditons)
        {
            var conversationField = _conversationFieldOptionService.FindAllAndFillOptions().Where(t => filterConditons.Select(f => f.FieldId).Contains(t.Id)).ToList();
            foreach (var filterConditon in filterConditons)
            {
                if (conversationField.Where(t => t.Id == filterConditon.FieldId).FirstOrDefault().DataType == FieldDataType.DateTime)
                {
                    if (conversationField.Where(t => t.Id == filterConditon.FieldId && t.Options.Any(o => o.Value == filterConditon.Value)).Count() == 0)
                    {
                        if (filterConditon.MatchType == ConditionMatchType.Between)
                        {
                            string[] value = filterConditon.Value.Split('|');
                            DateTime DateTime1;
                            if (!DateTime.TryParse(value[0], out DateTime1))
                            {
                                throw SocialExceptions.BadRequest("The value of date time is invalid");
                            }
                            DateTime DateTime2;
                            if (!DateTime.TryParse(value[1], out DateTime2))
                            {
                                throw SocialExceptions.BadRequest("The value of date time is invalid");
                            }
                        }
                        else
                        {
                            DateTime date;
                            if (!DateTime.TryParse(filterConditon.Value, out date))
                            {
                                throw SocialExceptions.BadRequest($"The value's type is not DateTime : '{filterConditon.Value}' ");
                            }
                        }
                    }
                }
                else if (conversationField.Where(t => t.Id == filterConditon.FieldId).FirstOrDefault().DataType == FieldDataType.Number)
                {
                    int number;
                    if (!int.TryParse(filterConditon.Value, out number))
                    {
                        throw SocialExceptions.BadRequest($"The value's type is not Number : '{filterConditon.Value}' ");
                    }
                }
                else if (conversationField.Where(t => t.Id == filterConditon.FieldId).FirstOrDefault().DataType == FieldDataType.Option)
                {
                    if (conversationField.Where(t => t.Id == filterConditon.FieldId && t.Options.Any(o => o.Value == filterConditon.Value)).Count() == 0)
                    {
                        if ((!departmentAddOptions.Contains(filterConditon.Value) || !departmentFieldNames.Contains(conversationField.Where(t => t.Id == filterConditon.FieldId).FirstOrDefault().Name))
                            && (!agentAddOptions.Contains(filterConditon.Value) || !agentsFieldNames.Contains(conversationField.Where(t => t.Id == filterConditon.FieldId).FirstOrDefault().Name)))
                            throw SocialExceptions.BadRequest($"The value's type is not Option : '{filterConditon.Value}' ");
                    }
                }
            }
        }

        private List<string> departmentAddOptions = new List<string> { "@My Department", "Blank" };
        private List<string> agentAddOptions = new List<string> { "@Me", "Blank", "@My Department Member" };
        private List<string> agentsFieldNames = new List<string> { "Agent Assignee", "Replied Agent", "Last Replied Agent" };
        private List<string> departmentFieldNames = new List<string> { "Department Assignee" };

    }
}
