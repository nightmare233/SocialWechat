using EntityFramework.DynamicFilters;
using Framework.Core;
using Framework.Core.UnitOfWork;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Core.EntityFramework
{
    public class DataContext : DbContext
    {
        private const string SITE_FILTER_NAME = "SiteIdFilter";

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProver { get; set; }
        public IUserContext UserContext { get; set; }

        protected DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected DataContext()
        {
            InitializeDbContext();
        }

        protected void InitializeDbContext()
        {
            //to-do
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Filter(SITE_FILTER_NAME, (IHaveSite t, int siteId) => t.SiteId == siteId, () =>
            {
                int? siteId = GetSiteId();
                return siteId.HasValue ? siteId.Value : 0;
            });
            modelBuilder.EnableFilter(SITE_FILTER_NAME, () => GetSiteId() != null);
        }

        public virtual void Initialize()
        {
            Database.Initialize(false);
            this.SetFilterScopedParameterValue(SITE_FILTER_NAME, "siteId", GetSiteId());
        }

        public override int SaveChanges()
        {
            ApplyChangeConventions();

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw BeautifyDbEntityValidationException(ex);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ApplyChangeConventions();

            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException ex)
            {
                throw BeautifyDbEntityValidationException(ex);
            }
        }

        private DbEntityValidationException BeautifyDbEntityValidationException(DbEntityValidationException ex)
        {
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .SelectMany(x => x.ErrorMessage);

            var fullErrorMessage = string.Join(";", errorMessages);
            var exceptionMessage = string.Concat(ex.Message, " The validation errors are : ", fullErrorMessage);
            return new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
        }

        protected virtual void ApplyChangeConventions()
        {
            var entries = ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetSite(entry.Entity);
                        SetCreation(entry.Entity);
                        SetCreatedTime(entry.Entity);
                        break;
                    case EntityState.Modified:
                        SetSite(entry.Entity);
                        SetModification(entry.Entity);
                        SetModifiedTime(entry.Entity);
                        CheckSiteWhenModification(entry.Entity);
                        break;
                    case EntityState.Deleted:
                        //to do
                        break;
                }
            }
        }

        private void SetSite(object entityObj)
        {
            var entity = entityObj as IHaveSite;
            if (entity?.SiteId != 0)
            {
                return;
            }

            var siteId = GetSiteId();
            if (siteId != null)
            {
                entity.SiteId = siteId.Value;
            }
            else
            {
                //throw new NotSupportedException("Can not set SiteId.");
            }
        }

        private void SetCreation(object entityObj)
        {
            if (!(entityObj is IHaveCreation))
            {
                return;
            }

            var entity = (IHaveCreation)entityObj;
            var userId = GetUserId();
            if (userId != null)
            {
                entity.CreatedTime = DateTime.UtcNow;
                entity.CreatedBy = userId.Value;
            }
        }

        private void SetCreatedTime(object entityObj)
        {
            if (!(entityObj is IHaveCreatedTime))
            {
                return;
            }

            var entity = (IHaveCreatedTime)entityObj;
            entity.CreatedTime = DateTime.UtcNow;
        }

        private void SetModification(object entityObj)
        {
            if (!(entityObj is IHaveModification))
            {
                return;
            }

            var entity = (IHaveModification)entityObj;
            var userId = GetUserId();
            if (userId != null)
            {
                if (entity.ModifiedTime != null)
                {
                    entity.ModifiedTime = DateTime.Now;
                }
                if (entity.ModifiedBy != null)
                {
                    entity.ModifiedBy = userId.Value;
                }
            }
        }

        private void SetModifiedTime(object entityObj)
        {
            if (!(entityObj is IHaveModifiedTime))
            {
                return;
            }

            var entity = (IHaveModifiedTime)entityObj;
            entity.ModifiedTime = DateTime.UtcNow;
        }

        private void CheckSiteWhenModification(object entityObj)
        {
            if (!(entityObj is IHaveSite))
            {
                return;
            }

            var entity = (IHaveSite)entityObj;
            var siteId = GetSiteId();
            if (siteId != null)
            {
                if (entity.SiteId != siteId.Value)
                {
                    //throw new UnauthorizedAccessException("Unauthorized user owner.");
                }
            }
        }

        private int? GetSiteId()
        {
            return CurrentUnitOfWorkProver.Current.GetSiteId();
        }

        private int? GetUserId()
        {
            return UserContext.UserId;
        }
    }
}
