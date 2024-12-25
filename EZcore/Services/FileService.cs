#nullable disable

using EZcore.Extensions;
using EZcore.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace EZcore.Services
{
    public class FileService : ServiceBase
    {
        public string Folder { get; set; } = "files";
        public double MaximumSizeInMb { get; set; } = 5;
        public int MaximumOtherFilesCount { get; set; } = 25;
        public List<string> Extensions { get; set; } = [".jpg", ".jpeg", ".png"];
        public Dictionary<string, string> MimeTypes { get; set; } = new Dictionary<string, string>
        {
            { ".txt", "text/plain" },
            { ".pdf", "application/pdf" },
            { ".doc", "application/vnd.ms-word" },
            { ".docx", "application/vnd.ms-word" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".csv", "text/csv" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" }
        };

        public string SizeNotValid { get; set; }
        public string ExtensionNotValid { get; set; }
        public string FilesCreated { get; set; }
        public string FilesUpdated { get; set; }
        public string FilesDeleted { get; set; }
        public string FileNotFound { get; set; }

        public bool IsExcelLicenseCommercial { get; set; }

        public FileService(HttpServiceBase httpService) : base(httpService)
        {
            SizeNotValid = Lang == Lang.EN ? $"Invalid file size, valid maxiumum file size: {MaximumSizeInMb} MB!" :
                $"Geçersiz dosya boyutu, geçerli maksimum dosya boyutu: {MaximumSizeInMb} MB!";
            ExtensionNotValid = Lang == Lang.EN ? $"Invalid file extension, valid file extensions: {string.Join(", ", Extensions)}!" :
               $"Geçersiz dosya uzantısı, geçerli dosya uzantıları: {string.Join(", ", Extensions)}!";
            FilesCreated = Lang == Lang.EN ? "Files created successfully." : "Dosyalar başarıyla oluşturuldu.";
            FilesUpdated = Lang == Lang.EN ? "Files updated successfully." : "Dosyalar başarıyla güncellendi.";
            FilesDeleted = Lang == Lang.EN ? "Files deleted successfully." : "Dosyalar başarıyla silindi.";
            FileNotFound = Lang == Lang.EN ? "File not found!" : "Dosya bulunamadı!";
        }

        protected string GetFileExtension(IFormFile formFile)
        {
            return Path.GetExtension(formFile.FileName).ToLower();
        }

        protected string GetFileExtension(string filePath)
        {
            return $".{filePath.Split('.')[1]}";
        }

        protected string GetFilePath(IFormFile formFile)
        {
            return Path.Combine("wwwroot", GetFileFolder(), $"{Guid.NewGuid().ToString()}{GetFileExtension(formFile)}");
        }

        protected string GetFilePath(string filePath, bool wwwroot = false)
        {
            return string.IsNullOrWhiteSpace(filePath) ? null : wwwroot ? $"wwwroot{filePath}" : filePath.Substring(7).Replace(@"\", "/");
        }

        protected string GetContentType(string filePath, bool data = false, bool base64 = false)
        {
            var fileExtension = GetFileExtension(filePath);
            var contentType = MimeTypes[fileExtension];
            if (data)
                contentType = "data:" + contentType;
            if (base64)
                contentType = contentType + ";base64,";
            return contentType;
        }

        public string GetFileFolder(string filePath = null)
        {
            return string.IsNullOrWhiteSpace(filePath) ? Folder : filePath.Split('/')[1];
        }

        public string GetFileName(string filePath, bool extension = true)
        {
            var fileName = filePath.Split('/')[filePath.Split('/').Length - 1].Split('.')[0];
            if (extension)
                fileName += GetFileExtension(filePath);
            return fileName;
        }

        public int GetFileOrder(string filePath)
        {
            return Convert.ToInt32(filePath.Split('/')[2]);
        }

        public void UpdateOtherFilePaths(List<string> filePaths, int orderInitialValue, int orderPaddingTotalWidth = 3)
        {
            if (filePaths is not null && filePaths.Any())
            {
                string orderValue;
                for (int i = 0; i < filePaths.Count; i++)
                {
                    orderValue = orderInitialValue++.ToString().PadLeft(orderPaddingTotalWidth, '0');
                    filePaths[i] = $"/{GetFileFolder(filePaths[i])}/{orderValue}/{GetFileName(filePaths[i])}";
                }
            }
        }

        public void UpdateOtherFilePaths(List<string> filePaths)
        {
            if (filePaths is not null && filePaths.Any())
            {
                for (int i = 0; i < filePaths.Count; i++)
                {
                    filePaths[i] = $"/{GetFileFolder(filePaths[i])}/{GetFileName(filePaths[i])}";
                }
            }
        }

        public virtual bool ValidateOtherFiles(List<IFormFile> formFiles, List<string> currentFilePaths = null)
        {
            var otherFilesCount = 0;
            if (formFiles is not null)
                otherFilesCount += formFiles.Count;
            if (currentFilePaths is not null)
                otherFilesCount += currentFilePaths.Count;
            if (otherFilesCount > MaximumOtherFilesCount)
                Error(Lang == Lang.TR ? $"Diğer dosya sayısı maksimum {MaximumOtherFilesCount} olmalıdır!" :
                    $"Other files count must be maximum {MaximumOtherFilesCount}!");
            return IsSuccessful;
        }

        public virtual string Create(IFormFile formFile)
        {
            string filePath = null;
            if (formFile is not null && formFile.Length > 0)
            {
                Validate(formFile);
                if (IsSuccessful)
                {
                    filePath = GetFilePath(formFile);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FilesCreated);
                }
            }
            return GetFilePath(filePath);
        }

        public virtual string Update(IFormFile formFile, string currentFilePath)
        {
            string filePath = GetFilePath(currentFilePath, true);
            if (formFile is not null && formFile.Length > 0)
            {
                Validate(formFile);
                if (IsSuccessful)
                {
                    Delete(currentFilePath);
                    filePath = GetFilePath(formFile);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    Success(FilesUpdated);
                }
            }
            return GetFilePath(filePath);
        }

        public virtual void Delete(string currentFilePath)
        {
            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                currentFilePath = GetFilePath(currentFilePath, true);
                if (File.Exists(currentFilePath))
                {
                    File.Delete(currentFilePath);
                    Success(FilesDeleted);
                }
            }
        }

        public virtual void Validate(IFormFile formFile)
        {
            if (formFile.Length > MaximumSizeInMb * Math.Pow(1024, 2))
                Error(SizeNotValid);
            else if (!Extensions.Contains(GetFileExtension(formFile)))
                Error(ExtensionNotValid);
        }

        public virtual List<string> Create(List<IFormFile> formFiles)
        {
            List<string> filePathList = null;
            if (formFiles is not null && formFiles.Any())
            {
                filePathList = new List<string>();
                foreach (var formFile in formFiles)
                {
                    Validate(formFile);
                    if (!IsSuccessful)
                        break;
                }
                if (IsSuccessful)
                {
                    foreach (var formFile in formFiles)
                    {
                        filePathList.Add(Create(formFile));
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

        public virtual void GetExcel<T>(List<T> list) where T : class, new()
        {
            var fileName = Lang == Lang.EN ? "Report.xlsx" : "Rapor.xlsx";
            var worksheet = Lang == Lang.EN ? "Sheet1" : "Sayfa1";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            ExcelPackage.LicenseContext = IsExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage();
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(worksheet);
            excelWorksheet.Cells["A1"].LoadFromDataTable(list.ConvertToDataTable(Lang), true);
            excelWorksheet.Cells["A1:AZ1"].Style.Font.Bold = true;
            excelWorksheet.Cells["A1:AZ1"].AutoFilter = true;
            excelWorksheet.Cells["A:AZ"].AutoFitColumns();
            _httpService.GetResponse(excelPackage.GetAsByteArray(), fileName, contentType);
        }

        public virtual FileDownloadModel GetFile(string filePath, bool useOctetStreamContentType = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;
            return new FileDownloadModel()
            {
                Stream = new FileStream(GetFilePath(filePath, true), FileMode.Open),
                ContentType = useOctetStreamContentType ? "application/octet-stream" : GetContentType(filePath),
                Name = GetFileName(filePath)
            };
        }
    }
}
