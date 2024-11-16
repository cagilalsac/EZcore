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
            .OrderByDescending(s => s.IsVirtual).ThenBy(s => s.Name);

        public override void Update(StoreModel model, bool save = true)
        {
            var store = Records(model.Record.Id);
            store.Name = model.Record.Name;
            store.IsVirtual = model.Record.IsVirtual;
            model.Record = store;
            base.Update(model, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }
    }
}
