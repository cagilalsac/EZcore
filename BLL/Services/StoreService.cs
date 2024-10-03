#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class StoreService : Service<Store, StoreModel>
    {
        public StoreService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Store> Records => base.Records.Include(s => s.ProductStores).ThenInclude(ps => ps.Product)
            .OrderByDescending(s => s.IsVirtual).ThenBy(s => s.Name);

        public override ResultServiceBase Create(Store record, bool save = true)
        {
            if (Records.Any(s => EF.Functions.Collate(s.Name, Collation) == EF.Functions.Collate(record.Trim().Name, Collation)))
                return Error(RecordWithSameNameExists);
            return base.Create(record, save);
        }

        public override ResultServiceBase Update(Store record, bool save = true)
        {
            if (Records.Any(s => s.Id != record.Id && EF.Functions.Collate(s.Name, Collation) == EF.Functions.Collate(record.Trim().Name, Collation)))
                return Error(RecordWithSameNameExists);
            var store = Find(record.Id);
            store.Name = record.Name;
            store.IsVirtual = record.IsVirtual;
            return base.Update(store, save);
        }

        public override ResultServiceBase Delete(int id, bool save = true)
        {
            var store = Find(id);
            Delete(store.ProductStores);
            return base.Delete(id, save);
        }
    }
}
