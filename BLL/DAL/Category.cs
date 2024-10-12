#nullable disable

using EZcore.Attributes;
using EZcore.DAL;

namespace BLL.DAL
{
    public class Category : Record
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
