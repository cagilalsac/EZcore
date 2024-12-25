#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;
using EZcore.Models;

namespace BLL.Services
{
    public class StoreService : Service<Store, StoreModel>
    {
        public override string ViewModelName => Lang == Lang.TR ? "Mağaza" : "Store";
        public StoreService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Store> Records() => base.Records().Include(s => s.ProductStores).ThenInclude(ps => ps.Product)
            .OrderByDescending(s => s.IsVirtual).ThenBy(s => s.Name);

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).ProductStores);
            base.Delete(id, save);
        }
    }
}
