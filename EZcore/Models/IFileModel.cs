using Microsoft.AspNetCore.Http;

namespace EZcore.Models
{
    public interface IFileModel
    {
        public IFormFile MainFormFile { get; set; }

        public List<IFormFile> OtherFormFiles { get; set; }
    }
}
