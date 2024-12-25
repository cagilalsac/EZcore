#nullable disable

namespace EZcore.Models
{
    public class FileDownloadModel
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
    }
}
