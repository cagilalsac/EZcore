#nullable disable

using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EZcore.Services
{
    public abstract class ServiceBase<TEntity, TModel> : ResultServiceBase, IDisposable where TEntity : Record, new() where TModel : Model<TEntity>, new()
    {
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

        protected ServiceBase(IDb db)
        {
            _db = db;
        }

        protected virtual IQueryable<TEntity> Records() => _db.Set<TEntity>();

        protected TEntity Records(int id) => Records().SingleOrDefault(entity => entity.Id == id);

        public virtual IQueryable<TModel> Query() => Records().AsNoTracking().Select(entity => new TModel() { Record = entity }); 

        public List<TModel> Read(PageOrder pageOrder = null)
        {
            List<TModel> list;
            Error(RecordNotFound);
            if (pageOrder is null)
            {
                list = Query().ToList();
                if (list.Any())
                    Success($"{list.Count} {(list.Count == 1 ? RecordFound : RecordsFound)}");
            }
            else
            {
                pageOrder.Lang = Lang;
                list = Records().AsNoTracking().OrderBy(pageOrder).Paginate(pageOrder).Select(entity => new TModel() { Record = entity }).ToList();
                if (pageOrder.TotalRecordsCount > 0)
                    Success($"{pageOrder.TotalRecordsCount} {(pageOrder.TotalRecordsCount == 1 ? RecordFound : RecordsFound)}");
            }
            return list;
        }

        public TModel Read(int id)
        {
            var item = Query().SingleOrDefault(model => model.Record.Id == id);
            if (item is not null)
                Success();
            else
                Error(RecordNotFound);
            return item;
        }

        public virtual ResultServiceBase Create(TEntity record, bool save = true)
        {
            _db.Set<TEntity>().Add(record.Trim());
            if (save)
            {
                Save();
                return Success(RecordCreated);
            }
            return Success(RecordNotSaved);
        }

        public virtual ResultServiceBase Update(TEntity record, bool save = true)
        {
            _db.Set<TEntity>().Update(record.Trim());
            if (save)
            {
                try
                {
                    Save();
                    return Success(RecordUpdated);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Error(RecordNotFound);
                }
            }
            return Success(RecordNotSaved);
        }

        public virtual ResultServiceBase Delete(int id, bool save = true)
        {
            var record = _db.Set<TEntity>().Find(id);
            if (record is null)
                return Error(RecordNotFound);
            _db.Set<TEntity>().Remove(record);
            if (save)
            {
                Save();
                return Success(RecordDeleted);
            }
            return Success(RecordNotSaved);
        }

        protected void Delete<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)))
                _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
        }

        protected virtual int Save() => _db.SaveChanges();

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
