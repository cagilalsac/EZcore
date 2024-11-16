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
        public ProductService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Product> Records()
        {
            AddPageOrderExpression("Name", "Adı");
            AddPageOrderExpression("Unit Price", "Birim Fiyatı");
            AddPageOrderExpression("Stock Amount", "Stok Miktarı");
            AddPageOrderExpression("Expiration Date", "Son Kullanma Tarihi");
            return base.Records().Include(p => p.Category).Include(p => p.ProductStores).ThenInclude(ps => ps.Store).OrderBy(p => p.Name);
        }

        public override ServiceBase Validate(ProductModel model)
        {
            if ((model.Record.StockAmount ?? 0) < 0)
                return Error(Lang == Lang.TR ? "Stok miktarı 0 veya pozitif bir sayı olmalıdır!" : "Stock amount must be 0 or a positive number!");
            if (model.Record.UnitPrice <= 0 || model.Record.UnitPrice > 100000)
                return Error(Lang == Lang.TR ? "Birim fiyat 0'dan büyük 100000'den küçük olmalıdır!" : "Unit price must be greater than 0 and less than 100000!");
            return base.Validate(model);
        }

        public override void Update(ProductModel model, bool save = true)
        {
            var product = Records(model.Record.Id);
            Update(product.ProductStores);
            product.Name = model.Record.Name;
            product.UnitPrice = model.Record.UnitPrice;
            product.StockAmount = model.Record.StockAmount;
            product.ExpirationDate = model.Record.ExpirationDate;
            product.CategoryId = model.Record.CategoryId;
            product.Stores = model.Record.Stores;
            model.Record = product;
            base.Update(model, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }
    }
}
