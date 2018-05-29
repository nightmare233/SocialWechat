using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public interface IDomainService<TEntity> where TEntity : Entity
    {
        TEntity Find(int id);
        IQueryable<TEntity> FindAll();
        void Delete(int id);
        void Delete(TEntity entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(TEntity entity);
        void Update(TEntity entity);
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}
