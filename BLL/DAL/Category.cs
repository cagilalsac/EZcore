#nullable disable

using EZcore.Attributes;
using EZcore.DAL;

namespace BLL.DAL
{
    public class Category : Record
    {
        [Required]
        [StringLength(100)]
        public override string Name { get => base.Name; set => base.Name = value; }

        public string Description { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
