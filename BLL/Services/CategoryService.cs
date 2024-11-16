#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;
using EZcore.Extensions;

namespace BLL.Services
{
    public class CategoryService : Service<Category, CategoryModel>
    {
        public CategoryService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Category> Records() => base.Records().Include(c => c.Products).OrderBy(c => c.Name);

        public override void Update(CategoryModel model, bool save = true)
        {
            var category = Records(model.Record.Id);
            category.Name = model.Record.Name;
            category.Description = model.Record.Description;
            model.Record = category;
            base.Update(model, save);
        }

        public override void Delete(int id, bool save = true)
        {
            if (Validate(Records(id).Products))
                base.Delete(id, save);
        }
    }
}
