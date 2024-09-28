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

        protected override IQueryable<Category> Records => base.Records.OrderBy(r => r.Name);

        public override ResultServiceBase Create(Category record, bool save = true)
        {
            if (Records.Any(r => r.Name.ToUpper() == record.Name.ToUpper().Trim()))
                return Error(RecordWithSameNameExists);
            return base.Create(record, save);
        }

        public override ResultServiceBase Update(Category record, bool save = true)
        {
            if (Records.Any(r => r.Id != record.Id && r.Name.ToUpper() == record.Name.ToUpper().Trim()))
                return Error(RecordWithSameNameExists);
            return base.Update(record, save);
        }

        public override ResultServiceBase Delete(int id, bool save = true)
        {
            var record = Records.Include(r => r.Products).SingleOrDefault(r => r.Id == id);
            if (record.Products.Any())
                return Error(RelationalRecordsFound);
            return base.Delete(id, save);
        }
    }
}
