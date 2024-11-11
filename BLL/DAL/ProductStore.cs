#nullable disable

using EZcore.DAL;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
    public class ProductStore : Record
    {
        public int ProductId { get; set; }

        public int StoreId { get; set; }

		[JsonIgnore]
		public Product Product { get; set; }

		[JsonIgnore]
		public Store Store { get; set; }
    }
}
