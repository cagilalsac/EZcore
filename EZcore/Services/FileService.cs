#nullable disable

using EZcore.Extensions;
using EZcore.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace EZcore.Services
{
    public abstract class FileServiceBase : ServiceBase
    {
        public string Folder { get; set; } = "files";
        public double MaximumSizeInMb { get; set; } = 5;
        public List<string> Extensions { get; set; } = [".jpg", ".jpeg", ".png" ];

        public string SizeNotValid { get; set; }
        public string ExtensionNotValid { get; set; }
        public string FilesCreated { get; set; }
        public string FilesUpdated { get; set; }
        public string FilesDeleted { get; set; }

        public bool IsExcelLicenseCommercial { get; set; }

        protected FileServiceBase(HttpServiceBase httpService) : base(httpService)
        {
            SizeNotValid = Lang == Lang.EN ? $"Invalid file size, valid maxiumum file size: {MaximumSizeInMb} MB!" :
                $"Geçersiz dosya boyutu, geçerli maksimum dosya boyutu: {MaximumSizeInMb} MB!";
            ExtensionNotValid = Lang == Lang.EN ? $"Invalid file extension, valid file extensions: {string.Join(", ", Extensions)}!" :
               $"Geçersiz dosya uzantısı, geçerli dosya uzantıları: {string.Join(", ", Extensions)}!";
            FilesCreated = Lang == Lang.EN ? "Files created successfully." : "Dosyalar başarıyla oluşturuldu.";
            FilesUpdated = Lang == Lang.EN ? "Files updated successfully." : "Dosyalar başarıyla güncellendi.";
            FilesDeleted = Lang == Lang.EN ? "Files deleted successfully." : "Dosyalar başarıyla silindi.";
        }

        public virtual string Create(IFormFile formFile, int? order = null)
        {
            string filePath = null;
            string fileName;
            if (formFile is not null && formFile.Length > 0)
            {
                if (Validate(formFile).IsSuccessful)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName).ToLower();
                    if (order.HasValue)
                        fileName = order.Value.ToString().PadLeft(2, '0') + "-" + fileName;
                    filePath = Path.Combine("wwwroot", Folder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FilesCreated);
                }
            }
            return filePath?.Substring(7).Replace(@"\", "/");
        }

        public virtual string Update(IFormFile formFile, string currentFilePath, int? order = null)
        {
            string filePath = string.IsNullOrWhiteSpace(currentFilePath) ? null : "wwwroot" + currentFilePath;
            string fileName;
            if (formFile is not null && formFile.Length > 0)
            {
                if (Validate(formFile).IsSuccessful)
                {
                    if (!string.IsNullOrWhiteSpace(currentFilePath))
                        Delete(currentFilePath);
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName).ToLower();
                    if (order.HasValue)
                        fileName = order.Value.ToString().PadLeft(2, '0') + "-" + fileName;
                    filePath = Path.Combine("wwwroot", Folder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FilesUpdated);
                }
            }
            return filePath?.Substring(7).Replace(@"\", "/");
        }

        public virtual void Delete(string currentFilePath)
        {
            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                currentFilePath = "wwwroot" + currentFilePath;
                if (File.Exists(currentFilePath))
                {
                    File.Delete(currentFilePath);
                    Success(FilesDeleted);
                }
            }
        }

        public virtual ServiceBase Validate(IFormFile formFile)
        {
            if (formFile.Length > MaximumSizeInMb * Math.Pow(1024, 2))
                return Error(SizeNotValid);
            if (!Extensions.Contains(Path.GetExtension(formFile.FileName).ToLower()))
                return Error(ExtensionNotValid);
            return Success();
        }

        public virtual List<string> Create(List<IFormFile> formFiles, int order)
        {
            List<string> filePathList = null;
            bool valid = false;
            if (formFiles is not null && formFiles.Any())
            {
                filePathList = new List<string>();
                foreach (var formFile in formFiles)
                {
                    valid = Validate(formFile).IsSuccessful;
                    if (!valid)
                        break;
                }
                if (valid)
                {
                    foreach (var formFile in formFiles)
                    {
                        filePathList.Add(Create(formFile, ++order));
                    }
                }
            }
            return filePathList;
        }

        public virtual void Delete(List<string> currentFilePaths)
        {
            if (currentFilePaths is not null && currentFilePaths.Any())
            {
                foreach (var currentFilePath in currentFilePaths)
                {
                    Delete(currentFilePath);
                }
            }
        }

        public virtual void GetExcel<T>(List<T> list, string fileNameWithoutExtension = null)
            where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
                fileNameWithoutExtension = Lang == Lang.EN ? "Report" : "Rapor";
            ExcelPackage.LicenseContext = IsExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage();
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(Lang == Lang.EN ? "Sheet1" : "Sayfa1");
            excelWorksheet.Cells["A1"].LoadFromDataTable(list.ConvertToDataTable(Lang), true);
            excelWorksheet.Cells["A1:AZ1"].Style.Font.Bold = true;
            excelWorksheet.Cells["A1:AZ1"].AutoFilter = true;
            excelWorksheet.Cells["A:AZ"].AutoFitColumns();
            _httpService.GetResponse(excelPackage.GetAsByteArray(), fileNameWithoutExtension + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }

    public class FileService : FileServiceBase
    {
        public FileService(HttpServiceBase httpService) : base(httpService)
        {
        }
    }
}
