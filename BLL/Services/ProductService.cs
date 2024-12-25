#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Models;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ProductService : Service<Product, ProductModel>
    {
        public override string ViewModelName => Lang == Lang.TR ? "Ürün" : "Product";
        protected override bool ViewPageOrder => true;

        public ProductService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
            AddPageOrderExpression("Name", "Adı");
            AddPageOrderExpression("Unit Price", "Birim Fiyatı");
            AddPageOrderExpression("Stock Amount", "Stok Miktarı");
            AddPageOrderExpression("Expiration Date", "Son Kullanma Tarihi");
        }

        protected override IQueryable<Product> Records()
        {
            return base.Records().Include(p => p.Category).Include(p => p.ProductStores).ThenInclude(ps => ps.Store).OrderBy(p => p.Name);
        }

        public override bool Validate(ProductModel model)
        {
            if ((model.Record.StockAmount ?? 0) < 0)
                Error(Lang == Lang.TR ? "Stok miktarı 0 veya pozitif bir sayı olmalıdır!" : "Stock amount must be 0 or a positive number!");
            else if (model.Record.UnitPrice <= 0 || model.Record.UnitPrice > 100000)
                Error(Lang == Lang.TR ? "Birim fiyat 0'dan büyük 100000'den küçük olmalıdır!" : "Unit price must be greater than 0 and less than 100000!");
            return base.Validate(model);
        }

        public override void Update(ProductModel model, bool save = true)
        {
            Update(Records(model.Record.Id).ProductStores);
            base.Update(model, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }

        public override ProductModel Get(int id)
        {
            var product = base.Get(id);
            product.UnitPriceText = $"{(product.Record.UnitPrice ?? 0).ConvertMoneyToString()}";
            return product;
        }
    }
}
