#nullable disable

using BLL.DAL;
using EZcore.Attributes;
using EZcore.Models;

namespace BLL.Models
{
    public class StoreModel : Model<Store>
    {
        [DisplayName("Name", "Adı")]
        public string Name => Record.Name;

        [DisplayName("Virtual", "Sanal")]
        public string IsVirtual => Record.IsVirtual ? "Yes" : "No";

        [DisplayName("Product Count", "Ürün Sayısı")]
        public string ProductCount => Record.ProductStores?.Count.ToString();

        [DisplayName("Products", "Ürünler")]
        public string Products => string.Join("<br>", Record.ProductStores?.Select(ps => ps.Product?.Name));
    }
}
