#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Services;
using EZcore.Extensions;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ProductService : ServiceBase<Product, ProductModel>
    {
        protected override string RecordFound => Lang == Lang.EN ? "product found." : "ürün bulundu.";
        protected override string RecordsFound => Lang == Lang.EN ? "products found." : "ürün bulundu.";
        protected override string RecordWithSameNameExists => Lang == Lang.EN ? "Product with the same name exists!" : "Aynı ada sahip ürün bulunmaktadır!";
        protected override string RecordCreated => Lang == Lang.EN ? "Product created successfully." : "Ürün başarıyla oluşturuldu.";
        protected override string RecordNotFound => Lang == Lang.EN ? "Product not found!" : "Ürün bulunamadı!";
        protected override string RecordUpdated => Lang == Lang.EN ? "Product updated successfully." : "Ürün başarıyla güncellendi.";
        protected override string RecordDeleted => Lang == Lang.EN ? "Product deleted successfully." : "Ürün başarıyla silindi.";

        public ProductService(IDb db) : base(db)
        {
        }

        protected override IQueryable<Product> Records => base.Records.Include(p => p.Category).Include(p => p.ProductStores).ThenInclude(ps => ps.Store)
            .OrderBy(p => p.StockAmount).ThenByDescending(p => p.UnitPrice).ThenBy(p => p.Name);

        public override ResultServiceBase Create(Product record, bool save = true)
        {
            if (Records.Any(p => EF.Functions.Collate(p.Name, Collation) == EF.Functions.Collate(record.Trim().Name, Collation)))
                return Error(RecordWithSameNameExists);
            return base.Create(record, save);
        }

        public override ResultServiceBase Update(Product record, bool save = true)
        {
            if (Records.Any(p => p.Id != record.Id && EF.Functions.Collate(p.Name, Collation) == EF.Functions.Collate(record.Trim().Name, Collation)))
                return Error(RecordWithSameNameExists);
            var product = Find(record.Id);
            if (product is null)
                return Error(RecordNotFound);
            product.Name = record.Name;
            product.UnitPrice = record.UnitPrice;
            product.StockAmount = record.StockAmount;
            product.ExpirationDate = record.ExpirationDate;
            product.CategoryId = record.CategoryId;
            product.ProductStores = record.ProductStores;
            return base.Update(product, save);
        }

        public override ResultServiceBase Delete(int id, bool save = true)
        {
            var product = Find(id);
            if (product is null)
                return Error(RecordNotFound);
            Delete(product.ProductStores);
            return base.Delete(id, save);
        }
    }
}
