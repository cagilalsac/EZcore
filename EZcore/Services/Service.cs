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
        private readonly PropertyInfo _guidProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(Record.Guid));
        private readonly PropertyInfo _nameProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IName.Name));
        private readonly PropertyInfo _isDeletedProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(ISoftDelete.IsDeleted));
        private readonly PropertyInfo _createDateProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IModifiedBy.CreateDate));
        private readonly PropertyInfo _createdByProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IModifiedBy.CreatedBy));
        private readonly PropertyInfo _updateDateProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IModifiedBy.UpdateDate));
        private readonly PropertyInfo _updatedByProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IModifiedBy.UpdatedBy));
        private readonly PropertyInfo _mainFilePathProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IFile.MainFilePath));
        private readonly PropertyInfo _otherFilePathsProperty = ObjectExtensions.GetPropertyInfo<TEntity>(nameof(IFile.OtherFilePaths));

        private bool _hasGuid => _guidProperty is not null;
        private bool _hasName => _nameProperty is not null;
        private bool _hasFile => _mainFilePathProperty is not null && _otherFilePathsProperty is not null;
        private bool _hasIsDeleted => _isDeletedProperty is not null;
        private bool _hasModifiedBy => _createDateProperty is not null && _createdByProperty is not null &&
            _updateDateProperty is not null && _updatedByProperty is not null;

        private Dictionary<string, string> _pageOrderExpressions = new Dictionary<string, string>();
        private string _pageOrderExpression;

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
                RelationalRecordsFound = Lang == Lang.TR ? "İlişkili kayıtlar bulunmaktadır!" : "Relational records found!";
                RecordFound = Lang == Lang.TR ? "kayıt bulundu." : "record found.";
                RecordsFound = Lang == Lang.TR ? "kayıt bulundu." : "records found.";
                RecordCreated = Lang == Lang.TR ? "Kayıt başarıyla oluşturuldu." : "Record created successfully.";
                RecordUpdated = Lang == Lang.TR ? "Kayıt başarıyla güncellendi." : "Record updated successfully.";
                RecordDeleted = Lang == Lang.TR ? "Kayıt başarıyla silindi." : "Record deleted successfully.";
                RecordNotSaved = Lang == Lang.TR ? "Kaydedilmedi." : "Not saved.";
            }
        }

        protected virtual bool ViewPageOrder { get; }

        protected FileService _fileService;

        private readonly IDb _db;

        protected Service(IDb db, HttpServiceBase httpService) : base(httpService)
        {
            _db = db;
        }

        /// <summary>
        /// Must be added by related entity property names, seperated by a space character for multiple words if any, 
        /// and the relevant Turkish expressions. Turkish characters will be replaced with corresponding English characters. 
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

        protected virtual IQueryable<TEntity> Records()
        {
            var query = _db.Set<TEntity>().AsNoTracking();
            if (_hasIsDeleted)
                query = query.Where(entity => (EF.Property<bool?>(entity, _isDeletedProperty.Name) ?? false) == false);
            return query;
        }

        protected TEntity Records(int id) => Records().SingleOrDefault(entity => entity.Id == id);

        public virtual List<TModel> Get(PageOrder pageOrder = null)
        {
            List<TModel> list;
            var totalRecordsCount = 0;
            if (ViewPageOrder && pageOrder is not null)
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
                list = Records().OrderBy(pageOrder).Paginate(pageOrder).Select(entity => new TModel() { Record = entity }).ToList();
                totalRecordsCount = pageOrder.TotalRecordsCount;
                if (!Api)
                    _httpService.SetSession(nameof(PageOrder), pageOrder);
            }
            else
            {
                if (pageOrder is not null)
                    pageOrder.PageNumber = 0;
                list = Records().Select(entity => new TModel() { Record = entity }).ToList();
                totalRecordsCount = list.Count;
            }
            if (totalRecordsCount > 0)
            {
                Success($"{totalRecordsCount} {(totalRecordsCount == 1 ? RecordFound : RecordsFound)}");
                if (_hasFile)
                {
                    _fileService = new FileService(_httpService);
                    foreach (var item in list)
                    {
                        _fileService.UpdateOtherFilePaths((item.Record as IFile).OtherFilePaths);
                    }
                }
            }
            else
            {
                Error(RecordNotFound, false);
            }
            return list;
        }

        public virtual TModel Get(int id)
        {
            var item = Records().Select(entity => new TModel() { Record = entity }).SingleOrDefault(model => model.Record.Id == id);
            if (item is null)
            {
                Error(RecordNotFound, false);
            }
            else if (_hasFile)
            {
                _fileService = new FileService(_httpService);
                _fileService.UpdateOtherFilePaths((item.Record as IFile).OtherFilePaths);
            }
            return item;
        }

        public virtual bool Validate(TModel model)
        {
            if (_hasName)
            {
                var record = model.Record as IName;
                if (Records().Any(entity => entity.Id != model.Record.Id &&
                    EF.Functions.Collate((entity as IName).Name, Collation) == EF.Functions.Collate(record.Name ?? "", Collation).Trim()))
                {
                    Error(RecordWithSameNameExists);
                }
            }
            return IsSuccessful;
        }

        public virtual void Create(TModel model, bool save = true)
        {
            if (!IsSuccessful)
                return;
            CreateFiles(model);
            if (!IsSuccessful)
                return;
            if (_hasGuid)
            {
                model.Record.Guid = Guid.NewGuid().ToString();
            }
            if (_hasModifiedBy)
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
            var record = _db.Set<TEntity>().Find(model.Record.Id);
            if (record is null)
            {
                Error(RecordNotFound);
                return;
            }
            UpdateFiles(model, record);
            if (!IsSuccessful)
                return;
            if (_hasModifiedBy)
            {
                (model.Record as IModifiedBy).UpdateDate = DateTime.Now;
                (model.Record as IModifiedBy).UpdatedBy = _httpService.UserName;
            }
            _db.Entry(record).State = EntityState.Detached;
            record = model.Record.Map<TEntity>();
            _db.Set<TEntity>().Update(record.Trim());
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
            if (ObjectExtensions.GetPropertyInfo<TRelationalEntity>().Any(property => property.PropertyType == typeof(TEntity)))
                _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
        }

        public virtual void Delete(int id, bool save = true)
        {
            if (!IsSuccessful)
                return;
            var record = _db.Set<TEntity>().Find(id);
            if (record is null)
            {
                Error(RecordNotFound);
                return;
            }
            if (_hasIsDeleted)
            {
                (record as ISoftDelete).IsDeleted = true;
                if (_hasModifiedBy)
                {
                    (record as IModifiedBy).UpdateDate = DateTime.Now;
                    (record as IModifiedBy).UpdatedBy = _httpService.UserName;
                }
                _db.Set<TEntity>().Update(record);
            }
            else
            {
                DeleteFiles(record);
                if (!IsSuccessful)
                    return;
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
            if (ObjectExtensions.GetPropertyInfo<TRelationalEntity>().Any(property => property.PropertyType == typeof(TEntity)))
            {
                if (!_hasIsDeleted)
                    _db.Set<TRelationalEntity>().RemoveRange(relationalRecords);
            }
        }

        protected void Validate<TRelationalEntity>(List<TRelationalEntity> relationalRecords) where TRelationalEntity : Record, new()
        {
            if (ObjectExtensions.GetPropertyInfo<TRelationalEntity>().Any(property => property.PropertyType == typeof(TEntity)) && relationalRecords.Any())
                Error(RelationalRecordsFound);
        }

        protected virtual void Save()
        {
            foreach (var entityEntry in _db.ChangeTracker.Entries<TEntity>())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        if (_hasGuid)
                        {
                            entityEntry.Property(_guidProperty.Name).IsModified = false;
                        }
                        if (_hasModifiedBy)
                        {
                            entityEntry.Property(_createDateProperty.Name).IsModified = false;
                            entityEntry.Property(_createdByProperty.Name).IsModified = false;
                        }
                        break;
                    case EntityState.Deleted:
                        if (_hasGuid && _hasIsDeleted)
                        {
                            entityEntry.Property(_guidProperty.Name).IsModified = false;
                        }
                        break;
                }
            }
            _db.SaveChanges();
        }

        public virtual void CreateFiles(TModel model)
        {
            if (_hasFile)
            {
                _fileService = new FileService(_httpService);
                var fileModel = model as IFileModel;
                if (_fileService.ValidateOtherFiles(fileModel?.OtherFormFiles))
                {
                    var filePath = _fileService.Create(fileModel.MainFormFile);
                    if (_fileService.IsSuccessful)
                    {
                        var fileEntity = model.Record as IFile;
                        fileEntity.MainFilePath = filePath;
                        var filePaths = _fileService.Create(fileModel.OtherFormFiles);
                        if (_fileService.IsSuccessful)
                        {
                            _fileService.UpdateOtherFilePaths(filePaths, 1);
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
            }
        }

        public virtual void UpdateFiles(TModel model, TEntity record = null)
        {
            if (_hasFile)
            {
                _fileService = new FileService(_httpService);
                if (record is null)
                {
                    record = _db.Set<TEntity>().Find(model.Record.Id);
                }
                if (record is null)
                {
                    Error(RecordNotFound);
                }
                else
                {
                    var fileModel = model as IFileModel;
                    var fileEntity = model.Record as IFile;
                    if (_fileService.ValidateOtherFiles(fileModel.OtherFormFiles, fileEntity.OtherFilePaths))
                    {
                        var filePath = _fileService.Update(fileModel.MainFormFile, fileEntity.MainFilePath);
                        if (_fileService.IsSuccessful)
                        {
                            fileEntity.MainFilePath = filePath;
                            var fileRecord = record as IFile;
                            var orderInitialValue = 1;
                            if (fileRecord.OtherFilePaths is not null && fileRecord.OtherFilePaths.Any())
                            {
                                fileEntity.OtherFilePaths = fileRecord.OtherFilePaths;
                                var lastOtherFilePath = fileRecord.OtherFilePaths.Order().Last();
                                orderInitialValue = _fileService.GetFileOrder(lastOtherFilePath) + 1;
                            }
                            var filePaths = _fileService.Create(fileModel.OtherFormFiles);
                            if (_fileService.IsSuccessful)
                            {
                                if (filePaths is not null && filePaths.Any())
                                {
                                    _fileService.UpdateOtherFilePaths(filePaths, orderInitialValue);
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
            }
        }

        protected void DeleteFiles(TEntity record, string filePath = null)
        {
            if (_hasFile)
            {
                _fileService = new FileService(_httpService);
                var fileEntity = record as IFile;
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    _fileService.Delete(fileEntity.MainFilePath);
                    fileEntity.MainFilePath = null;
                    _fileService.Delete(fileEntity.OtherFilePaths);
                    fileEntity.OtherFilePaths = null;
                }
                else if (filePath == fileEntity.MainFilePath)
                {
                    _fileService.Delete(fileEntity.MainFilePath);
                    fileEntity.MainFilePath = null;
                }
                else
                {
                    _fileService.Delete(filePath);
                    filePath = fileEntity.OtherFilePaths.SingleOrDefault(otherFilePath => 
                        $"/{_fileService.GetFileFolder(otherFilePath)}/{_fileService.GetFileName(otherFilePath)}" == filePath);
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        fileEntity.OtherFilePaths.Remove(filePath);
                        if (!fileEntity.OtherFilePaths.Any())
                            fileEntity.OtherFilePaths = null;
                    }
                }
                _db.Set<TEntity>().Update(record);
                Success(_fileService.FilesDeleted);
            }
        }

        public virtual void DeleteFiles(int id, string filePath = null)
        {
            var record = _db.Set<TEntity>().Find(id);
            if (record is null)
            {
                Error(RecordNotFound);
            }
            else
            {
                DeleteFiles(record, filePath);
                _db.SaveChanges();
            }
        }

        public virtual void GetExcel()
        {
            _fileService = new FileService(_httpService);
            _fileService.GetExcel(Get());
        }

        public virtual FileDownloadModel GetFile(string filePath)
        {
            _fileService = new FileService(_httpService);
            if (_hasFile)
            {
                var file = _fileService.GetFile(filePath);
                if (file is null)
                    Error(_fileService.FileNotFound, false);
                return file;
            }
            return null;
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
