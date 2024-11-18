#nullable disable

using EZcore.DAL;
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
        public string FileCreated { get; set; }
        public string FileUpdated { get; set; }
        public string FileDeleted { get; set; }

        public bool IsExcelLicenseCommercial { get; set; }

        protected readonly HttpServiceBase _httpService;

        protected FileServiceBase(Lang lang, HttpServiceBase httpService)
        {
            Lang = lang;
            SizeNotValid = Lang == Lang.EN ? $"Invalid file size, valid maxiumum file size: {MaximumSizeInMb} MB!" :
                $"Geçersiz dosya boyutu, geçerli maksimum dosya boyutu: {MaximumSizeInMb} MB!";
            ExtensionNotValid = Lang == Lang.EN ? $"Invalid file extension, valid file extensions: {string.Join(", ", Extensions)}!" :
               $"Geçersiz dosya uzantısı, geçerli dosya uzantıları: {string.Join(", ", Extensions)}!";
            FileCreated = Lang == Lang.EN ? "File created successfully." : "Dosya başarıyla oluşturuldu.";
            FileUpdated = Lang == Lang.EN ? "File updated successfully." : "Dosya başarıyla güncellendi.";
            FileDeleted = Lang == Lang.EN ? "File deleted successfully." : "Dosya başarıyla silindi.";
            _httpService = httpService;
        }

        public virtual string Create(IFormFile formFile)
        {
            string filePath = null;
            string fileName;
            if (formFile is not null && formFile.Length > 0)
            {
                if (Validate(formFile).IsSuccessful)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName).ToLower();
                    filePath = Path.Combine("wwwroot", Folder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FileCreated);
                }
            }
            return filePath?.Substring(7).Replace(@"\", "/");
        }

        public virtual string Update(IFormFile formFile, string currentFilePath)
        {
            string filePath = string.IsNullOrWhiteSpace(currentFilePath) ? null : "wwwroot" + currentFilePath;
            string fileName;
            if (formFile is not null && formFile.Length > 0)
            {
                if (Validate(formFile).IsSuccessful)
                {
                    Delete(currentFilePath);
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName).ToLower();
                    filePath = Path.Combine("wwwroot", Folder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FileUpdated);
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
                    Success(FileDeleted);
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

        public virtual void GetExcel<T>(List<T> list, string fileNameWithoutExtension, params int[] columnNumbersToDelete) 
        {
            if (list is not null && list.Any())
            {
                ExcelPackage.LicenseContext = IsExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
                var excelPackage = new ExcelPackage();
                var excelWorksheet = excelPackage.Workbook.Worksheets.Add(Lang == Lang.EN ? "Sheet1" : "Sayfa1");
                excelWorksheet.Cells["A1"].LoadFromCollection(list, true);
                excelWorksheet.Cells["A1:AZ1"].Style.Font.Bold = true;
                excelWorksheet.Cells["A1:AZ1"].AutoFilter = true;
                excelWorksheet.Cells["A:AZ"].AutoFitColumns();
                columnNumbersToDelete = columnNumbersToDelete.OrderByDescending(columnNumber => columnNumber).ToArray();
                foreach (var columnNumberToDelete in columnNumbersToDelete)
                {
                    excelWorksheet.DeleteColumn(columnNumberToDelete);
                }
                _httpService.GetResponse(excelPackage.GetAsByteArray(), fileNameWithoutExtension + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }

    public class FileService : FileServiceBase
    {
        public FileService(Lang lang, HttpServiceBase httpService) : base(lang, httpService)
        {
        }
    }
}
