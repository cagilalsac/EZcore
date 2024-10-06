#nullable disable

using BLL.DAL;
using BLL.Models;
using EZcore.DAL;
using EZcore.Services;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CategoryService : ServiceBase<Category, CategoryModel>
    {
        public CategoryService(IDb db) : base(db)
        {
        }

        protected override IQueryable<Category> Records() => base.Records().Include(c => c.Products).OrderBy(c => c.Name);

        public override ResultServiceBase Create(Category record, bool save = true)
        {
            if (Records().Any(c => c.Name.ToUpper() == record.Name.ToUpper().Trim()))
                return Error(RecordWithSameNameExists);
            return base.Create(record, save);
        }

        public override ResultServiceBase Update(Category record, bool save = true)
        {
            if (Records().Any(c => c.Id != record.Id && c.Name.ToUpper() == record.Name.ToUpper().Trim()))
                return Error(RecordWithSameNameExists);
            return base.Update(record, save);
        }

        public override ResultServiceBase Delete(int id, bool save = true)
        {
            if (Records(id).Products.Any())
                return Error(RelationalRecordsFound);
            return base.Delete(id, save);
        }
    }
}
