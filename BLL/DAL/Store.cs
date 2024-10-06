#nullable disable

using EZcore.Attributes;
using EZcore.DAL;

namespace BLL.DAL
{
    public class Store : Record
    {
        [EZRequired]
        [EZLength(200, 5)]
        public string Name { get; set; }

        public bool IsVirtual { get; set; }

        public List<ProductStore> ProductStores { get; private set; } = new List<ProductStore>();
    }
}
