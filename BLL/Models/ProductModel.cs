#nullable disable

using BLL.DAL;
using EZcore.Models;
using System.ComponentModel;

namespace BLL.Models
{
    public class ProductModel : Model<Product>
    {
        public string Name => Record.Name;

        [DisplayName("Unit Price")]
        public string UnitPrice => (Record.UnitPrice ?? 0).ToString("C2");

        [DisplayName("Stock Amount")]
        public string StockAmount => Record.StockAmount.HasValue ?
            ("<span class=" +
                (Record.StockAmount.Value < 10 ? "\"badge bg-danger\">"
                : Record.StockAmount.Value < 100 ? "\"badge bg-warning\">"
                : "\"badge bg-success\">") + Record.StockAmount.Value + "</span>")
            : string.Empty;

        [DisplayName("Expiration Date")]
        public string ExpirationDate => Record.ExpirationDate.HasValue ? Record.ExpirationDate.Value.ToShortDateString() : "";

        public string Category => Record.Category?.Name;

        public string Stores => string.Join("<br>", Record.ProductStores?.OrderBy(ps => ps.Store?.StoreName).Select(ps => ps.Store?.StoreName));
    }
}
