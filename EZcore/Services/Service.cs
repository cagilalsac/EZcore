#nullable disable

using EZcore.DAL;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EZcore.Services
{
    public abstract class Service<TEntity, TModel> : ServiceBase<TEntity, TModel> where TEntity : Record, new() where TModel : Model<TEntity>, new()
    {
        private readonly PropertyInfo _guidProperty = typeof(TEntity).GetProperty(nameof(Record.Guid));
        private readonly PropertyInfo _isDeletedProperty = typeof(TEntity).GetProperty(nameof(ISoftDelete.IsDeleted));
        private readonly PropertyInfo _createDateProperty = typeof(TEntity).GetProperty(nameof(IModifiedBy.CreateDate));
        private readonly PropertyInfo _createdByProperty = typeof(TEntity).GetProperty(nameof(IModifiedBy.CreatedBy));
        private readonly PropertyInfo _updateDateProperty = typeof(TEntity).GetProperty(nameof(IModifiedBy.UpdateDate));
        private readonly PropertyInfo _updatedByProperty = typeof(TEntity).GetProperty(nameof(IModifiedBy.UpdatedBy));

        private bool _hasGuidProperty => _guidProperty is not null;
        private bool _hasIsDeletedProperty => _isDeletedProperty is not null;
        private bool _hasModifiedByProperty => _createDateProperty is not null && _createdByProperty is not null &&
            _updateDateProperty is not null && _updatedByProperty is not null;

        protected override IQueryable<TEntity> Records() => _hasIsDeletedProperty ?
            base.Records().Where(entity => (EF.Property<bool?>(entity, _isDeletedProperty.Name) ?? false) == false) : base.Records();

        protected readonly HttpServiceBase _httpService;

        protected Service(IDb db, HttpServiceBase httpService) : base(db)
        {
            _httpService = httpService;
        }

        protected override int Save()
        {
            foreach (var entityEntry in _db.ChangeTracker.Entries<TEntity>())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        if (_hasGuidProperty)
                        {
                            entityEntry.CurrentValues[_guidProperty.Name] = Guid.NewGuid().ToString();
                        }
                        if (_hasModifiedByProperty)
                        {
                            entityEntry.CurrentValues[_createDateProperty.Name] = DateTime.Now;
                            entityEntry.CurrentValues[_createdByProperty.Name] = _httpService.UserIdentityName;
                        }
                        break;
                    case EntityState.Modified:
                        if (_hasGuidProperty)
                        {
                            entityEntry.Property(_guidProperty.Name).IsModified = false;
                        }
                        if (_hasModifiedByProperty)
                        {
                            entityEntry.CurrentValues[_updateDateProperty.Name] = DateTime.Now;
                            entityEntry.CurrentValues[_updatedByProperty.Name] = _httpService.UserIdentityName;
                        }
                        break;
                    case EntityState.Deleted:
                        if (_hasIsDeletedProperty)
                        {
                            if (_hasGuidProperty)
                            {
                                entityEntry.Property(_guidProperty.Name).IsModified = false;
                            }
                            if (_hasModifiedByProperty)
                            {
                                entityEntry.CurrentValues[_updateDateProperty.Name] = DateTime.Now;
                                entityEntry.CurrentValues[_updatedByProperty.Name] = _httpService.UserIdentityName;
                            }
                            entityEntry.CurrentValues[_isDeletedProperty.Name] = true;
                            entityEntry.State = EntityState.Modified;
                        }
                        break;
                }
            }
            return base.Save();
        }
    }
}
