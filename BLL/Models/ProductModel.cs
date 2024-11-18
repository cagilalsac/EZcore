#nullable disable

using BLL.DAL;
using EZcore.Models;
using Microsoft.AspNetCore.Http;
using EZcore.Attributes;

namespace BLL.Models
{
    public class ProductModel : Model<Product>, IFileModel
    {
        [DisplayName("Name", "Adı")]
        public string Name => Record.Name;

        [DisplayName("Unit Price", "Birim Fiyatı")]
        public string UnitPrice => (Record.UnitPrice ?? 0).ToString("C2");

        [DisplayName("Stock Amount", "Stok Miktarı")]
        public string StockAmount => Record.StockAmount.HasValue ?
            ("<span class=" +
                (Record.StockAmount.Value < 10 ? "\"badge bg-danger\">"
                : Record.StockAmount.Value < 100 ? "\"badge bg-warning\">"
                : "\"badge bg-success\">") + Record.StockAmount.Value + "</span>")
            : string.Empty;

        [DisplayName("Expiration Date", "Son Kullanma Tarihi")]
        public string ExpirationDate => Record.ExpirationDate.HasValue ? Record.ExpirationDate.Value.ToShortDateString() : "";

        [DisplayName("Category", "Kategori")]
        public string Category => Record.Category?.Name;

        [DisplayName("Stores", "Mağazalar")]
        public string Stores => string.Join("<br>", Record.ProductStores?.OrderBy(ps => ps.Store?.Name).Select(ps => ps.Store?.Name));

        [DisplayName("File", "Dosya")]
        public IFormFile MainFormFilePath { get; set; }

        [DisplayName("Stock Amount", "Stok Miktarı")]
        public string StockAmountExcel => Record.StockAmount.HasValue ? Record.StockAmount.Value.ToString() : "";

        [DisplayName("Stores", "Mağazalar")]
        public string StoresExcel => string.Join(", ", Record.ProductStores?.OrderBy(ps => ps.Store?.Name).Select(ps => ps.Store?.Name));
    }
}
