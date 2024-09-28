#nullable disable

using EZcore.DAL;

namespace BLL.DAL
{
    public class ProductStore : Record
    {
        public int ProductId { get; set; }

        public int StoreId { get; set; }

        public Product Product { get; set; }

        public Store Store { get; set; }
    }
}
