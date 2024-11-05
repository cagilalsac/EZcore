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

        public override CategoryModel Update(Category record, bool save = true)
        {
            var category = Records(record.Id);
            category.Name = record.Name;
            category.Description = record.Description;
            return base.Update(category, save);
        }

        public override void Delete(int id, bool save = true)
        {
            if (Validate(Records(id).Products))
                base.Delete(id, save);
        }
    }
}
