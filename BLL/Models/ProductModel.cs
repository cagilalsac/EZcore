#nullable disable

using BLL.DAL;
using EZcore.Models;
using Microsoft.AspNetCore.Http;
using EZcore.Attributes;
using System.Globalization;

namespace BLL.Models
{
    public class ProductModel : Model<Product>, IFileModel
    {
        [DisplayName("Adı", "Name")]
        public string Name => Record.Name;

        [DisplayName("Birim Fiyatı", "Unit Price")]
        public string UnitPrice => (Record.UnitPrice ?? 0).ToString("C2", new CultureInfo("tr-TR"));

        [DisplayName("Stok Miktarı", "Stock Amount")]
        [Ignore]
        public string StockAmount => Record.StockAmount.HasValue ?
            ("<span class=" +
                (Record.StockAmount.Value < 10 ? "\"badge bg-danger\">"
                : Record.StockAmount.Value < 100 ? "\"badge bg-warning\">"
                : "\"badge bg-success\">") + Record.StockAmount.Value + "</span>")
            : string.Empty;

        [DisplayName("Son Kullanma Tarihi", "Expiration Date")]
        public string ExpirationDate => Record.ExpirationDate.HasValue ? Record.ExpirationDate.Value.ToShortDateString() : "";

        [DisplayName("Kategori", "Category")]
        public string Category => Record.Category?.Name;

        [DisplayName("Mağazalar", "Stores")]
        [Ignore]
        public string Stores => string.Join("<br>", Record.ProductStores?.OrderBy(ps => ps.Store?.Name).Select(ps => ps.Store?.Name));

        [DisplayName("Dosya", "File")]
        [Ignore]
        public IFormFile MainFormFilePath { get; set; }

        [DisplayName("Stok Miktarı", "Stock Amount")]
        public string StockAmountExcel => Record.StockAmount.HasValue ? Record.StockAmount.Value.ToString() : "";

        [DisplayName("Mağazalar", "Stores")]
        public string StoresExcel => string.Join(", ", Record.ProductStores?.OrderBy(ps => ps.Store?.Name).Select(ps => ps.Store?.Name));
    }
}
