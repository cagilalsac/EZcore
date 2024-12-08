#nullable disable

using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;
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
        private string _pageOrderExpression;

        protected FileServiceBase _fileService;

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
                RecordNotFound = Lang == Lang.TR ? "Kayıt bulunamadı!" : "Record not found!";
                RecordWithSameNameExists = Lang == Lang.TR ? "Aynı ada sahip kayıt bulunmaktadır!" : "Record with the same name exists!";
                RelationalRecordsFound = Lang == Lang.TR ? "İlişkili kayıtlar bulunmaktadır!" : "Related records found!";
                RecordFound = Lang == Lang.TR ? "kayıt bulundu." : "record found.";
                RecordsFound = Lang == Lang.TR ? "kayıt bulundu." : "records found.";
                RecordCreated = Lang == Lang.TR ? "Kayıt başarıyla oluşturuldu." : "Record created successfully.";
                RecordUpdated = Lang == Lang.TR ? "Kayıt başarıyla güncellendi." : "Record updated successfully.";
                RecordDeleted = Lang == Lang.TR ? "Kayıt başarıyla silindi." : "Record deleted successfully.";
                RecordNotSaved = Lang == Lang.TR ? "Kaydedilmedi." : "Not saved.";
            }
        }

        public bool UsePageOrder { get; set; }
        public string ExcelFileNameWithoutExtension { get; set; }

        protected readonly IDb _db;

        protected Service(IDb db, HttpServiceBase httpService) : base(httpService)
        {
            _db = db;
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

        public virtual List<TModel> Get(PageOrder pageOrder = null)
        {
            List<TModel> list;
            Error(RecordNotFound);
            if (UsePageOrder && pageOrder is not null)
            {
                if (!Api && pageOrder.Session)
                {
                    var pageOrderFromSession = _httpService.GetSession<PageOrder>(nameof(PageOrder));
                    if (pageOrderFromSession is not null)
                    {
                        pageOrder.PageNumber = pageOrderFromSession.PageNumber;
                        pageOrder.RecordsPerPageCount = pageOrderFromSession.RecordsPerPageCount;
                        pageOrder.OrderExpression = pageOrderFromSession.OrderExpression;
                    }
                }
                pageOrder.OrderExpressions = _pageOrderExpressions;
                if (pageOrder.OrderExpressions.Any() && string.IsNullOrWhiteSpace(pageOrder.OrderExpression))
                    pageOrder.OrderExpression = pageOrder.OrderExpressions.FirstOrDefault().Key;
                list = Records().AsNoTracking().OrderBy(pageOrder).Paginate(pageOrder).Select(entity => new TModel() { Record = entity }).ToList();
                if (pageOrder.TotalRecordsCount > 0)
                {
                    if (list.First() is IFileModel)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            UpdateOtherFilePaths((list[i].Record as IFile).OtherFilePaths);
                        }
                    }
                    Success($"{pageOrder.TotalRecordsCount} {(pageOrder.TotalRecordsCount == 1 ? RecordFound : RecordsFound)}");
                }
                if (!Api)
                    _httpService.SetSession(nameof(PageOrder), pageOrder);
            }
            else
            {
                if (pageOrder is not null)
                    pageOrder.PageNumber = 0;
                list = Records().AsNoTracking().Select(entity => new TModel() { Record = entity }).ToList();
                if (list.Any())
                {
                    if (list.First() is IFileModel)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            UpdateOtherFilePaths((list[i].Record as IFile).OtherFilePaths);
                        }
                    }
                    Success($"{list.Count} {(list.Count == 1 ? RecordFound : RecordsFound)}");
                }
            }
            return list;
        }

        public virtual TModel Get(int id)
        {
            var item = Records().Select(entity => new TModel() { Record = entity }).SingleOrDefault(model => model.Record.Id == id);
            if (item is null)
                Error(RecordNotFound);
            if (item is IFileModel)
                UpdateOtherFilePaths((item.Record as IFile).OtherFilePaths);
            return item;
        }

        public virtual ServiceBase Validate(TModel model)
        {
            if (model.Record is IName)
            {
                var record = model.Record as IName;
                if (Records().Any(entity => entity.Id != model.Record.Id &&
                    EF.Functions.Collate((entity as IName).Name, Collation) == EF.Functions.Collate(record.Name ?? "", Collation).Trim()))
                {
                    return Error(RecordWithSameNameExists);
                }
            }
            return Success();
        }

        public virtual void Create(TModel model, bool save = true)
        {
            if (!IsSuccessful)
                return;
            if (!CreateFiles(model))
                return;
            if (_hasGuidProperty)
            {
                model.Record.Guid = Guid.NewGuid().ToString();
            }
            if (_hasModifiedByProperty)
            {
                (model.Record as IModifiedBy).CreateDate = DateTime.Now;
                (model.Record as IModifiedBy).CreatedBy = _httpService.UserName;
            }
            _db.Set<TEntity>().Add(model.Record.Trim());
            Success(RecordNotSaved);
            if (save)
            {
                Save();
                Success(RecordCreated);
            }
        }

        public virtual void Update(TModel model, bool save = true)
        {
            if (!IsSuccessful)
                return;
            if (!UpdateFiles(model))
                return;
            if (_hasModifiedByProperty)
            {
                (model.Record as IModifiedBy).UpdateDate = DateTime.Now;
                (model.Record as IModifiedBy).UpdatedBy = _httpService.UserName;
            }
            _db.Set<TEntity>().Update(model.Record.Trim());
            Success(RecordNotSaved);
            if (save)
            {
                try
                {
                    Save();
                    Success(RecordUpdated);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Error(RecordNotFound);
                }
            }
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
            {
                Error(RecordNotFound);
                return;
            }
            if (_hasIsDeletedProperty)
            {
                (record as ISoftDelete).IsDeleted = true;
                if (_hasModifiedByProperty)
                {
                    (record as IModifiedBy).UpdateDate = DateTime.Now;
                    (record as IModifiedBy).UpdatedBy = _httpService.UserName;
                }
                _db.Set<TEntity>().Update(record);
            }
            else
            {
                DeleteFiles(record);
                _db.Set<TEntity>().Remove(record);
            }
            Success(RecordNotSaved);
            if (save)
            {
                Save();
                Success(RecordDeleted);
            }
        }

        protected void Delete<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)))
            {
                if (!_hasIsDeletedProperty)
                    _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
            }
        }

        protected bool Validate<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (typeof(TRelationalEntity).GetProperties().Any(property => property.PropertyType == typeof(TEntity)) && relationalRecords.Any())
                Error(RelationalRecordsFound);
            else
                Success();
            return IsSuccessful;
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

        public virtual bool CreateFiles(TModel model)
        {
            if (model is IFileModel)
            {
                var fileModel = model as IFileModel;
                _fileService = new FileService(_httpService);
                var filePath = _fileService.Create(fileModel.MainFormFilePath);
                if (_fileService.IsSuccessful)
                {
                    var fileEntity = model.Record as IFile;
                    fileEntity.MainFilePath = filePath;
                    var filePaths = _fileService.Create(fileModel.OtherFormFilePaths);
                    if (_fileService.IsSuccessful)
                    {
                        UpdateOtherFilePaths(filePaths, 1);
                        fileEntity.OtherFilePaths = filePaths;
                    }
                    else
                    {
                        Error(_fileService.Message);
                    }
                }
                else
                {
                    Error(_fileService.Message);
                }
            }
            return IsSuccessful;
        }

        private void UpdateOtherFilePaths(List<string> filePaths, int orderInitialValue, int paddingTotalWidth = 2)
        {
            if (filePaths is not null && filePaths.Any())
            {
                for (int i = 0; i < filePaths.Count; i++)
                {
                    filePaths[i] = "/" + filePaths[i].Split('/')[1] + "/" +
                        orderInitialValue++.ToString().PadLeft(paddingTotalWidth, '0') +
                        "/" + filePaths[i].Split('/')[2];
                }
            }
        }

        private void UpdateOtherFilePaths(List<string> filePaths)
        {
            if (filePaths is not null && filePaths.Any())
            {
                for (int i = 0; i < filePaths.Count; i++)
                {
                    filePaths[i] = "/" + filePaths[i].Split('/')[1] + "/" + filePaths[i].Split('/')[3];
                }
            }
        }

        public virtual bool UpdateFiles(TModel model)
        {
            if (model is IFileModel)
            {
                var record = _db.Set<TEntity>().Find(model.Record.Id);
                if (record is null)
                {
                    Error(RecordNotFound);
                }
                else
                {
                    var fileModel = model as IFileModel;
                    var fileEntity = model.Record as IFile;
                    _fileService = new FileService(_httpService);
                    var filePath = _fileService.Update(fileModel.MainFormFilePath, fileEntity.MainFilePath);
                    if (_fileService.IsSuccessful)
                    {
                        fileEntity.MainFilePath = filePath;
                        var fileRecord = record as IFile;
                        var orderInitialValue = 1;
                        if (fileRecord.OtherFilePaths is not null && fileRecord.OtherFilePaths.Any())
                        {
                            var lastOtherFilePath = fileRecord.OtherFilePaths.Order().Last();
                            orderInitialValue = Convert.ToInt32(lastOtherFilePath.Split('/')[2]) + 1;
                        }
                        var filePaths = _fileService.Create(fileModel.OtherFormFilePaths);
                        if (_fileService.IsSuccessful)
                        {
                            if (filePaths is not null && filePaths.Any())
                            {
                                UpdateOtherFilePaths(filePaths, orderInitialValue);
                                fileEntity.OtherFilePaths = fileEntity.OtherFilePaths ?? new List<string>();
                                fileEntity.OtherFilePaths.AddRange(filePaths);
                            }
                        }
                        else
                        {
                            Error(_fileService.Message);
                        }
                    }
                    else
                    {
                        Error(_fileService.Message);
                    }
                }
            }
            return IsSuccessful;
        }

        protected void DeleteFiles(TEntity record, string path = null)
        {
            if (record is IFile)
            {
                var fileEntity = record as IFile;
                _fileService = new FileService(_httpService);
                if (string.IsNullOrWhiteSpace(path))
                {
                    _fileService.Delete(fileEntity.MainFilePath);
                    fileEntity.MainFilePath = null;
                    _fileService.Delete(fileEntity.OtherFilePaths);
                    fileEntity.OtherFilePaths = null;
                }
                else if (path == fileEntity.MainFilePath)
                {
                    _fileService.Delete(fileEntity.MainFilePath);
                    fileEntity.MainFilePath = null;
                }
                else
                {
                    _fileService.Delete(path);
                    path = fileEntity.OtherFilePaths.SingleOrDefault(otherFilePath => 
                        "/" + otherFilePath.Split('/')[1] + "/" + otherFilePath.Split('/')[3] == path);
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        fileEntity.OtherFilePaths.Remove(path);
                        if (!fileEntity.OtherFilePaths.Any())
                            fileEntity.OtherFilePaths = null;
                    }
                }
                _db.Set<TEntity>().Update(record);
                Success(_fileService.FilesDeleted);
            }
        }

        public virtual void DeleteFiles(int id, string path = null)
        {
            var record = _db.Set<TEntity>().Find(id);
            if (record is null)
            {
                Error(RecordNotFound);
            }
            else
            {
                DeleteFiles(record, path);
                _db.SaveChanges();
            }
        }

        public virtual void GetExcel()
        {
            _fileService = new FileService(_httpService);
            _fileService.GetExcel(Get(), ExcelFileNameWithoutExtension);
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class Service : ServiceBase
    {
        public Service(HttpServiceBase httpService) : base(httpService)
        {
        }
    }
}
