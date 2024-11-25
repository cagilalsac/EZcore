using Microsoft.AspNetCore.Http;

namespace EZcore.Models
{
    public interface IFileModel
    {
        public IFormFile MainFormFilePath { get; set; }

        public List<IFormFile> OtherFormFilePaths { get; set; }
    }
}
