#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
    public class Category : Record, IName
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
