#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
	public class Store : Record
    {
		[Required]
		[StringLength(200, MinimumLength = 5)]
		public override string Name { get => base.Name; set => base.Name = value; }

        public bool IsVirtual { get; set; }

		[JsonIgnore]
		public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();
    }
}
