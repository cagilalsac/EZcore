#nullable disable

using BLL.DAL;
using EZcore.Models;
using System.ComponentModel;

namespace BLL.Models
{
    public class StoreModel : Model<Store>
    {
        public string Name => Record.Name;

        [DisplayName("Virtual")]
        public string IsVirtual => Record.IsVirtual ? "Yes" : "No";

        [DisplayName("Product Count")]
        public string ProductCount => Record.ProductStores?.Count.ToString();

        public string Products => string.Join("<br>", Record.ProductStores?.Select(ps => ps.Product?.Name));
    }
}
