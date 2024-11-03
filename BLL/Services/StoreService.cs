#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class StoreService : Service<Store, StoreModel>
    {
        public StoreService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Store> Records() => base.Records().Include(s => s.ProductStores).ThenInclude(ps => ps.Product)
            .OrderByDescending(s => s.IsVirtual).ThenBy(s => s.StoreName);

        protected override ServiceBase Validate(Store record)
        {
            if (Records().Any(s => s.Id != record.Id &&
                EF.Functions.Collate(s.StoreName, Collation) == EF.Functions.Collate(record.StoreName, Collation)))
            {
                return Error(RecordWithSameNameExists);
            }
            return Success();
        }

        public override void Create(Store record, bool save = true)
        {
            if (Validate(record).IsSuccessful)
                base.Create(record, save);
        }

        public override void Update(Store record, bool save = true)
        {
            if (!Validate(record).IsSuccessful)
                return;
            var store = Records(record.Id);
            store.StoreName = record.StoreName;
            store.IsVirtual = record.IsVirtual;
            base.Update(store, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }
    }
}
