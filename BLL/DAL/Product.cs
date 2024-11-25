#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BLL.DAL
{
    public class Product : Record, ISoftDelete, IModifiedBy, IFile
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

        [JsonIgnore]
        public Category Category { get; set; }

		[JsonIgnore]
		public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();

        [NotMapped]
        public List<int> Stores
        {
            get => ProductStores?.Select(ps => ps.StoreId).ToList();
            set => ProductStores = value?.Select(v => new ProductStore() { StoreId = v }).ToList();
        }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }

        public string MainFilePath { get; set; }
        public List<string> OtherFilePaths { get; set; }
    }
}
