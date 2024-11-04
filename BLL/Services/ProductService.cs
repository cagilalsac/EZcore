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
        protected override string RecordFound => Lang == Lang.EN ? "product found." : "ürün bulundu.";
        protected override string RecordsFound => Lang == Lang.EN ? "products found." : "ürün bulundu.";
        protected override string RecordWithSameNameExists => OperationFailed + (Lang == Lang.EN ? " Product with the same name exists!" : " Aynı ada sahip ürün bulunmaktadır!");
        protected override string RecordCreated => Lang == Lang.EN ? "Product created successfully." : "Ürün başarıyla oluşturuldu.";
        protected override string RecordNotFound => Lang == Lang.EN ? "Product not found!" : "Ürün bulunamadı!";
        protected override string RecordUpdated => Lang == Lang.EN ? "Product updated successfully." : "Ürün başarıyla güncellendi.";
        protected override string RecordDeleted => Lang == Lang.EN ? "Product deleted successfully." : "Ürün başarıyla silindi.";

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

        public override void Update(Product record, bool save = true)
        {
            var product = Records(record.Id);
            Update(product.ProductStores);
            product.Name = record.Name;
            product.UnitPrice = record.UnitPrice;
            product.StockAmount = record.StockAmount;
            product.ExpirationDate = record.ExpirationDate;
            product.CategoryId = record.CategoryId;
            product.StoreIds = record.StoreIds;
            base.Update(product, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }
    }
}
