using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Framework.Core;

namespace Framework.Core.EntityFramework
{
    public class EfRepository<TDbContext, TEntity> : EfRepository<TDbContext, TEntity, int>
        where TEntity : Entity<int>
        where TDbContext : DbContext
    {
    }

    public class EfRepository<TDbContext, TEntity, TKey> : ServiceBase, IRepository<TDbContext, TEntity, TKey>
        where TEntity : Entity<TKey>
        where TDbContext : DbContext
    {
        protected IDbSet<TEntity> DataSet
        {
            get
            {
                return _dbContext.Set<TEntity>();
            }
        }

        private TDbContext _dbContext
        {
            get
            {
                return CurrentUnitOfWork.GetOrCreateDbContext<TDbContext>();
            }
        }

        public EfRepository()
        {
        }

        public IQueryable<TEntity> FindAsNoTracking()
        {
            return DataSet.AsNoTracking();
        }

        public IQueryable<TEntity> FindAll()
        {
            return DataSet;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll().Where(predicate);
        }

        public TEntity Find(TKey id)
        {
            return DataSet.Find(id);
        }

        public void Insert(TEntity entity)
        {
            DataSet.Add(entity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            DataSet.Add(entity);
        }

        public void InsertMany(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                DataSet.Add(entity);
            }
        }

        public async Task InsertManyAsync(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                DataSet.Add(entity);
            }
        }

        public void Delete(TKey id)
        {
            var entity = Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = Find(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            SoftDelete(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            SoftDelete(entity);
        }

        public void DeleteMany(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                SoftDelete(entity);
            }
        }

        private void SoftDelete(TEntity entity)
        {
            var softDeleteEntity = entity as ISoftDelete;
            if (softDeleteEntity != null)
            {
                softDeleteEntity.IsDeleted = true;
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                DataSet.Remove(entity);
            }
        }

        public async Task DeleteManyAsync(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                DataSet.Remove(entity);
            }
        }

        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateMany(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public async Task UpdateManyAsync(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll().Where(predicate).Count();
        }
    }
}
