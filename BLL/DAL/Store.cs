#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
	public class Store : Record, IName
    {
		[Required]
		[StringLength(200, MinimumLength = 5)]
		public string Name { get; set; }

        public bool IsVirtual { get; set; }

		[JsonIgnore]
		public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();
    }
}
