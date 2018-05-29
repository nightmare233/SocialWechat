using Framework.Core.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Framework.Core
{
    public class ShardingTableInterceptor : IDbCommandInterceptor
    {
        private List<string> _shardingBySiteTables = new List<string>();

        public ShardingTableInterceptor(Assembly[] assemblies)
        {
            Initialize(assemblies);
        }

        private void Initialize(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                //var entities = typeof(ShardingTableInterceptor).Assembly.GetExportedTypes().Where(t => typeof(Entity).IsAssignableFrom(t));
                var entities = assembly.GetExportedTypes().Where(t => typeof(Entity).IsAssignableFrom(t));
                foreach (var entity in entities)
                {
                    if (typeof(IShardingBySiteId).IsAssignableFrom(entity))
                    {
                        var tableAttribute = entity.GetAttribute<TableAttribute>();
                        if (tableAttribute != null && !string.IsNullOrWhiteSpace(tableAttribute.Name))
                        {
                            if (!_shardingBySiteTables.Contains(tableAttribute.Name))
                            {
                                _shardingBySiteTables.Add(tableAttribute.Name);
                            }
                        }
                    }
                }
            }
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            ReplaceTableName(command);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            ReplaceTableName(command);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ReplaceTableName(command);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ReplaceTableName(command);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ReplaceTableName(command);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ReplaceTableName(command);
        }

        private void ReplaceTableName(DbCommand command)
        {
            CurrentUnitOfWorkProvider provider = new CurrentUnitOfWorkProvider();
            int? siteId = provider.Current.GetSiteId();
            if (siteId == null)
            {
                return;
            }

            command.CommandText = GetSql(siteId.Value, command.CommandText);
        }

        public List<string> GetShardingBySiteTables()
        {
            return _shardingBySiteTables;
        }

        public string GetSql(int siteId, string orignalSql)
        {
            string sql = orignalSql;
            foreach (var shardingTable in _shardingBySiteTables)
            {
                bool hasShardingTable = IsMatch(sql, shardingTable);
                bool isReplaced = IsMatch(sql, shardingTable + siteId);

                if (hasShardingTable && !isReplaced)
                {
                    sql = ReplaceTableName(sql, shardingTable, siteId);
                }
            }
            return sql;
        }

        private string ReplaceTableName(string input, string tableName, int siteId)
        {
            string sql = input;
            sql = Regex.Replace(sql, $"{tableName}$", tableName + siteId, RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, $"{tableName}\\s", tableName + siteId + " ", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, $"\\[{tableName}\\]", "[" + tableName + siteId + "]", RegexOptions.IgnoreCase);
            return sql;
        }

        private bool IsMatch(string sql, string tableName)
        {
            return Regex.IsMatch(sql, $"({tableName}$|{tableName}\\s|\\[{tableName}\\])", RegexOptions.IgnoreCase);
        }
    }
}
