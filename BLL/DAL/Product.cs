#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.DAL
{
    public class Product : Record, ISoftDelete, IModifiedBy
    {
        [Required]
        [StringLength(150)]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required]
        public decimal? UnitPrice { get; set; }

        public int? StockAmount { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public Category Category { get; set; }

        public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();

        [NotMapped]
        public List<int> StoreIds 
        { 
            get => ProductStores?.Select(ps => ps.StoreId).ToList(); 
            set => ProductStores = value?.Select(v => new ProductStore() { StoreId = v }).ToList(); 
        }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
