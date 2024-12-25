#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Extensions;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;
using EZcore.Models;

namespace BLL.Services
{
    public class CategoryService : Service<Category, CategoryModel>
    {
        public override string ViewModelName => Lang == Lang.TR ? "Kategori" : "Category";

        public CategoryService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Category> Records() => base.Records().Include(c => c.Products).OrderBy(c => c.Name);

        public override void Delete(int id, bool save = true)
        {
            Validate(Records(id).Products);
            base.Delete(id, save);
        }
    }
}
