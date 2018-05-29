using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Cache
{
    public class FilterCacheItem
    {
        public FilterCacheItem()
        {
            Conditions = new List<FilterConditionCache>();
        }

        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public bool IfPublic { get; set; }
        public FilterType Type { get; set; }
        public string LogicalExpression { get; set; }
        public int Index { get; set; }

        public IList<FilterConditionCache> Conditions { get; set; }

        public override bool Equals(object obj)
        {
            FilterCacheItem objFilter = obj as FilterCacheItem;
            if (objFilter == null)
            {
                return false;
            }

            return Id.Equals(objFilter.Id) && SiteId.Equals(objFilter.SiteId);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + SiteId.GetHashCode();
                return hash;
            }
        }
    }

    public class FilterConditionCache
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int FilterId { get; set; }
        public int FieldId { get; set; }
        public int Index { get; set; }
        public ConditionMatchType MatchType { get; set; }
        public string Value { get; set; }

        public ConversationFieldCache Field { get; set; }
    }

    public class ConversationFieldCache
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public bool IfSystem { get; set; }
        public FieldDataType DataType { get; set; }
        public string Name { get; set; }
    }
}
