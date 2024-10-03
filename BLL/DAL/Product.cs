#nullable disable

using EZcore.Attributes;
using EZcore.DAL;

namespace BLL.DAL
{
    public class Product : Record, ISoftDelete, IModifiedBy
    {
        [EZRequired]
        [EZLength(150)]
        public string Name { get; set; }

        [EZRequired]
        public decimal? UnitPrice { get; set; }

        public int? StockAmount { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [EZRequired]
        public int? CategoryId { get; set; }

        public Category Category { get; set; }

        public List<ProductStore> ProductStores { get; set; } = new List<ProductStore>();

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
