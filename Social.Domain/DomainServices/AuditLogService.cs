using Framework.Core;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public interface IAuditLogService
    {
        void Audit(int action, string content, object oldObj, object newObj);
    }

    public class AuditLogService : ServiceBase, IAuditLogService
    {
        private IRepository<AuditLog> _auditRepository;

        public AuditLogService(IRepository<AuditLog> auditRepository)
        {
            _auditRepository = auditRepository;
        }
        public void Audit(int action, string content, object oldObj, object newObj)
        {
            IList<AuditLogField> auditFields = new List<AuditLogField>();
            if (oldObj == null && newObj != null)
            {
                auditFields = GetLogFields(newObj);
            }
            else if(oldObj != null && newObj == null)
            {
                auditFields = GetLogFields(oldObj);
            }
            else
            {
                auditFields = DiffObject(oldObj, newObj);
            }
            Audit(action, content, auditFields);
        }

        private void Audit(int action, string content, IList<AuditLogField> auditFields)
        {
            if(auditFields.Count() > 0)
            {
                var auditLog = new AuditLog
                {
                    ApplicationType = 1,
                    ActionType = action,
                    ActionDetail = content,
                    OperatorId = UserContext.UserId,
                    ActionTime = DateTime.UtcNow,
                    ActionDetailData = Newtonsoft.Json.JsonConvert.SerializeObject(auditFields)
                };
                try
                {
                    UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(),() => { _auditRepository.Insert(auditLog); });
                }
                catch(Exception ex)
                {
                    Social.Infrastructure.Logger.Error(ex);
                }
            }
        }

        private IList<AuditLogField> DiffObject(object oldObj, object newObj)
        {
            Type oldType = oldObj.GetType();
            Type newType = newObj.GetType();
            IList<AuditLogField> logFields = new List<AuditLogField>();
            if(oldType == newType)
            {
                PropertyInfo[] properties = oldType.GetProperties();
                foreach(PropertyInfo pi in properties)
                {
                    if(!ShouldAuditLog(pi.PropertyType))
                    {
                        continue;
                    }
                    string fieldName;
                    bool ifIgnore;

                    CheckProPertyAttribute(pi, out ifIgnore, out fieldName);

                    if (ifIgnore)
                        continue;

                    object oldValue = oldType.GetProperty(pi.Name).GetValue(oldObj, null);
                    Type oldValueType = oldType.GetProperty(pi.Name).PropertyType;

                    object newValue = newType.GetProperty(pi.Name).GetValue(newObj, null);

                    if(oldValue != null && oldValue.Equals(newValue))
                    {
                        continue;
                    }
                    else if(oldValue == null && newValue == null)
                    {
                        continue;
                    }
                    else
                    {
                        if(ShouldAuditLog(oldValueType))
                        {
                            logFields.Add(new AuditLogField
                            {
                                FieldName = fieldName,
                                OldValue = oldValue == null ? "" : FieldValue(oldValue),
                                NewValue = newValue == null ? "" : FieldValue(newValue)
                            });
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }
            return logFields;
        }

        private IList<AuditLogField> GetLogFields(object logObj)
        {
            Type objType = logObj.GetType();
            IList<AuditLogField> logFields = new List<AuditLogField>();
            PropertyInfo[] properties = objType.GetProperties();
            foreach(PropertyInfo pi in properties)
            {
                if(!ShouldAuditLog(pi.PropertyType))
                {
                    continue;
                }

                string fieldName;
                bool ifIgnore;

                CheckProPertyAttribute(pi, out ifIgnore, out fieldName);

                if (ifIgnore)
                    continue;

                object objValue = objType.GetProperty(pi.Name).GetValue(logObj, null);

                logFields.Add(new AuditLogField
                {
                    FieldName = fieldName,
                    OldValue = "",
                    NewValue = objValue == null ? "" : FieldValue(objValue)
                });
            }
            return logFields;
        }

        private string FieldValue(object val)
        {
            Type t = val.GetType();
            string str = val.ToString();
            if(t.IsEnum)
            {
                str = string.Join(", ", str.Split(',').Select(s =>
                 {
                     s = s.Trim();
                     if (s.StartsWith("enum", StringComparison.OrdinalIgnoreCase))
                         s = s.Substring(4);
                     return s;
                 }).ToArray());
            }
            else if(t == typeof(bool))
            {
                str = str.ToLower();
            }
            else if(t == typeof(TimeSpan))
            {
                str = ((TimeSpan)val).ToString();
            }
            return str;
        }

        private void CheckProPertyAttribute(PropertyInfo pi, out bool ifIgnore, out string fieldName)
        {
            ifIgnore = false;
            fieldName = pi.Name;
            object[] arrAttrs = pi.GetCustomAttributes(true);

            foreach(object o in arrAttrs)
            {
                if(o.GetType() == typeof(AuditLogAttribute))
                {
                    AuditLogAttribute att = (AuditLogAttribute)o;
                    if(!string.IsNullOrEmpty(att.Name))
                    {
                        fieldName = att.Name;
                    }
                    ifIgnore = att.Ignore;
                    break;
                }
            }
        }

        private bool ShouldAuditLog(Type type)
        {
            if(type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(short) ||
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(bool) ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(TimeSpan) ||
                type == typeof(int?) ||
                type == typeof(long?) ||
                type == typeof(short?) ||
                type == typeof(double?) ||
                type == typeof(float?) ||
                type == typeof(bool?) ||
                type == typeof(DateTime?) ||
                type == typeof(TimeSpan?) ||
                type.IsEnum
                )
            {
                return true;
            }

            return false;
        }
    }

    internal class AuditLogAttribute : Attribute
    {
        public string Name { get; set; }

        public bool Ignore { get; set; }
    }

    internal class AuditLogField
    {
        public AuditLogField()
        {
            FieldName = "";
            OldValue = "";
            NewValue = "";
        }

        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public AuditLogField(string fieldName,string oldValue,string newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
