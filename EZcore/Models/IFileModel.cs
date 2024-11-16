using Microsoft.AspNetCore.Http;

namespace EZcore.Models
{
    public interface IFileModel
    {
        public IFormFile MainFormFilePath { get; set; }
    }
}
