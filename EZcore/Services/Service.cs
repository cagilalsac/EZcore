﻿#nullable disable

using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace EZcore.Services
{
    public abstract class Service<TEntity, TModel> : ServiceBase, IDisposable where TEntity : Record, new() where TModel : Model<TEntity>, new()
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

        private Dictionary<string, string> _pageOrderExpressions = new Dictionary<string, string>();

        protected virtual string Collation => "Turkish_CI_AS";

        protected virtual string RecordNotFound { get; private set; }
        protected virtual string RecordWithSameNameExists { get; private set; }
        protected virtual string RelationalRecordsFound { get; private set; }
        protected virtual string RecordFound { get; private set; }
        protected virtual string RecordsFound { get; private set; }
        protected virtual string RecordCreated { get; private set; }
        protected virtual string RecordUpdated { get; private set; }
        protected virtual string RecordDeleted { get; private set; }
        protected virtual string RecordNotSaved { get; private set; }

        public override Lang Lang
        {
            get => base.Lang;
            set
            {
                base.Lang = value;
                RecordNotFound = base.Lang == Lang.TR ? "Kayıt bulunamadı!" : "Record not found!";
                RecordWithSameNameExists = $"{OperationFailed} {(base.Lang == Lang.TR ? "Aynı ada sahip kayıt bulunmaktadır!" : "Record with the same name exists!")}";
                RelationalRecordsFound = $"{OperationFailed} {(base.Lang == Lang.TR ? "İlişkili kayıtlar bulunmaktadır!" : "Related records found!")}";
                RecordFound = base.Lang == Lang.TR ? "kayıt bulundu." : "record found.";
                RecordsFound = base.Lang == Lang.TR ? "kayıt bulundu." : "records found.";
                RecordCreated = base.Lang == Lang.TR ? "Kayıt başarıyla oluşturuldu." : "Record created successfully.";
                RecordUpdated = base.Lang == Lang.TR ? "Kayıt başarıyla güncellendi." : "Record updated successfully.";
                RecordDeleted = base.Lang == Lang.TR ? "Kayıt başarıyla silindi." : "Record deleted successfully.";
                RecordNotSaved = base.Lang == Lang.TR ? "Kaydedilmedi." : "Not saved.";
                Thread.CurrentThread.CurrentCulture = Lang == Lang.TR ? new CultureInfo("tr-TR") : new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = Lang == Lang.TR ? new CultureInfo("tr-TR") : new CultureInfo("en-US");
            }
        }

        protected readonly IDb _db;
        protected readonly HttpServiceBase _httpService;
        protected Service(IDb db, HttpServiceBase httpService)
        {
            _db = db;
            _httpService = httpService;
        }

        protected virtual IQueryable<TEntity> Records()
        {
            if (_hasIsDeletedProperty)
                return _db.Set<TEntity>().Where(entity => (EF.Property<bool?>(entity, _isDeletedProperty.Name) ?? false) == false);
            return _db.Set<TEntity>();
        }

        protected TEntity Records(int id) => Records().SingleOrDefault(entity => entity.Id == id);

        /// <summary>
        /// Must be added by related entity property names, seperated by a space character for multiple words if any, and the relevant Turkish expressions. 
        /// Turkish characters will be replaced with corresponding English characters. 
        /// </summary>
        /// <param name="entityPropertyName"></param>
        /// <param name="expressionTR"></param>
        protected void AddPageOrderExpression(string entityPropertyName, string expressionTR = "")
        {
            var descKey = "DESC";
            var descValue = Lang == Lang.TR ? "Azalan" : "Descending";
            var key = entityPropertyName.ChangeTurkishCharactersToEnglish().Replace(" ", "");
            var value = Lang == Lang.TR && !string.IsNullOrWhiteSpace(expressionTR) ? expressionTR : entityPropertyName;
            if (!_pageOrderExpressions.ContainsKey(key))
            {
                _pageOrderExpressions.Add(key, value);
                _pageOrderExpressions.Add($"{key}{descKey}", $"{value} {descValue}");
            }
        }

        public virtual List<TModel> Read(PageOrder pageOrder = null)
        {
            List<TModel> list;
            Error(RecordNotFound);
            if (pageOrder is null)
            {
                list = Records().AsNoTracking().Select(entity => new TModel() { Record = entity }).ToList();
                if (list.Any())
                    Success($"{list.Count} {(list.Count == 1 ? RecordFound : RecordsFound)}");
            }
            else
            {
                pageOrder.Lang = Lang;
                pageOrder.OrderExpressions = _pageOrderExpressions;
                list = Records().AsNoTracking().OrderBy(pageOrder).Paginate(pageOrder).Select(entity => new TModel() { Record = entity }).ToList();
                if (pageOrder.TotalRecordsCount > 0)
                    Success($"{pageOrder.TotalRecordsCount} {(pageOrder.TotalRecordsCount == 1 ? RecordFound : RecordsFound)}");
            }
            return list;
        }

        public virtual TModel Read(int id)
        {
            var item = Records().AsNoTracking().Select(entity => new TModel() { Record = entity }).SingleOrDefault(model => model.Record.Id == id);
            if (item is null)
                Error(RecordNotFound);
            return item;
        }

        protected virtual ServiceBase Validate(TEntity record)
        {
            if (Records().Any(entity => entity.Id != record.Id &&
                EF.Functions.Collate(entity.Name, Collation) == EF.Functions.Collate(record.Name ?? "", Collation).Trim()))
            {
                return Error(RecordWithSameNameExists);
            }
            return Success();
        }

        public virtual void Create(TEntity record, bool save = true)
        {
            if (!IsSuccessful)
                return;
            if (_hasGuidProperty)
            {
                record.Guid = Guid.NewGuid().ToString();
            }
            if (_hasModifiedByProperty)
            {
                (record as IModifiedBy).CreateDate = DateTime.Now;
                (record as IModifiedBy).CreatedBy = _httpService.UserIdentityName;
            }
            _db.Set<TEntity>().Add(record.Trim());
            if (save)
            {
                Save();
                Success(RecordCreated);
                return;
            }
            Success(RecordNotSaved);
        }

        public virtual void Update(TEntity record, bool save = true)
        {
            if (!IsSuccessful)
                return;
            if (_hasModifiedByProperty)
            {
                (record as IModifiedBy).UpdateDate = DateTime.Now;
                (record as IModifiedBy).UpdatedBy = _httpService.UserIdentityName;
            }
            _db.Set<TEntity>().Update(record.Trim());
            if (save)
            {
                try
                {
                    Save();
                    Success(RecordUpdated);
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {
                    Error(RecordNotFound);
                    return;
                }
            }
            Success(RecordNotSaved);
        }

        protected void Update<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)))
                _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
        }

        public virtual void Delete(int id, bool save = true)
        {
            var record = _db.Set<TEntity>().Find(id);
            if (record is null)
                Error(RecordNotFound);
            if (!IsSuccessful)
                return;
            if (_hasIsDeletedProperty)
            {
                (record as ISoftDelete).IsDeleted = true;
                if (_hasModifiedByProperty)
                {
                    (record as IModifiedBy).UpdateDate = DateTime.Now;
                    (record as IModifiedBy).UpdatedBy = _httpService.UserIdentityName;
                }
                _db.Set<TEntity>().Update(record);
            }
            else
            {
                _db.Set<TEntity>().Remove(record);
            }
            if (save)
            {
                Save();
                Success(RecordDeleted);
                return;
            }
            Success(RecordNotSaved);
        }

        protected void Delete<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)))
            {
                if (!_hasIsDeletedProperty)
                    _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
            }
        }

        protected ServiceBase Validate<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)))
            {
                if (relationalRecords.Any())
                    return Error(RelationalRecordsFound);
            }
            return Success();
        }

        protected virtual int Save()
        {
            foreach (var entityEntry in _db.ChangeTracker.Entries<TEntity>())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        if (_hasGuidProperty)
                        {
                            entityEntry.Property(_guidProperty.Name).IsModified = false;
                        }
                        break;
                    case EntityState.Deleted:
                        if (_hasGuidProperty && _hasIsDeletedProperty)
                        {
                            entityEntry.Property(_guidProperty.Name).IsModified = false;
                        }
                        break;
                }
            }
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}