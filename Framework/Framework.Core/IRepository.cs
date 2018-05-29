using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.Core
{
    public interface IRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TDbContext : DbContext
        where TEntity : Entity<TKey>
    {
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int>
        where TEntity : Entity<int>
    {
    }

    public interface IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        void Delete(TKey id);
        Task DeleteAsync(TKey id);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void DeleteMany(TEntity[] entities);
        Task DeleteManyAsync(TEntity[] entities);
        TEntity Find(TKey id);
        IQueryable<TEntity> FindAsNoTracking();
        IQueryable<TEntity> FindAll();
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        void InsertMany(TEntity[] entities);
        Task InsertManyAsync(TEntity[] entities);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void UpdateMany(TEntity[] entities);
        Task UpdateManyAsync(TEntity[] entities);
        int Count(Expression<Func<TEntity, bool>> predicate);
    }
}