#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
    public class Store : Record
    {
        [NotMapped, Obsolete]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string StoreName { get; set; }

        public bool IsVirtual { get; set; }

		[JsonIgnore]
		public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();
    }
}
