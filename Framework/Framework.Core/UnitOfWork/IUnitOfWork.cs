using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Framework.Core.UnitOfWork
{
    public interface IUnitOfWork : IUnitOfWorkCompleteHandle
    {
        string Id { get; }
        IUnitOfWork Outer { get; set; }
        bool IsDisposed { get; }
        void Begin(UnitOfWorkOptions options);

        IReadOnlyList<DbContext> GetAllActiveDbContexts();
        TDbContext GetOrCreateDbContext<TDbContext>() where TDbContext : DbContext;

        void SaveChanges();
        Task SaveChangesAsync();

        event EventHandler Completed;
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        event EventHandler Disposed;

        IDisposable SetSiteId(int? siteId);
        IDisposable UseGeneralDB();
        int? GetSiteId();
    }
}