#nullable disable

using BLL.DAL;
using EZcore.Attributes;
using EZcore.Extensions;
using EZcore.Models;

namespace BLL.Models
{
    public class StoreModel : Model<Store>
    {
        [DisplayName("Adı", "Name")]
        public string Name => Record.Name;

        [DisplayName("Sanal", "Virtual")]
        public string IsVirtual => Record.IsVirtual.ToHtml();

        [DisplayName("Ürün Sayısı", "Product Count")]
        public string ProductCount => Record.ProductStores?.Count(ps => (ps.Product?.IsDeleted ?? false) == false).ToString();

        [DisplayName("Ürünler", "Products")]
        public string Products => string.Join("<br>", Record.ProductStores?.Where(ps => (ps.Product?.IsDeleted ?? false) == false)
            .Select(ps => ps.Product?.Name));
    }
}
