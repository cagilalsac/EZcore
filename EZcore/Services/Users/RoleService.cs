#nullable disable

using EZcore.DAL;
using EZcore.DAL.Users;
using EZcore.Models.Users;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;

namespace EZcore.Services.Users
{
    public class RoleService : Service<Role, RoleModel>
    {
        protected override string RecordFound => Lang == Lang.EN ? "role found." : "rol bulundu.";
        protected override string RecordsFound => Lang == Lang.EN ? "roles found." : "rol bulundu.";
        protected override string RecordWithSameNameExists => Lang == Lang.EN ? " Role with the same name exists!" : " Aynı ada sahip rol bulunmaktadır!";
        protected override string RecordCreated => Lang == Lang.EN ? "Role created successfully." : "Rol başarıyla oluşturuldu.";
        protected override string RecordNotFound => Lang == Lang.EN ? "Role not found!" : "Rol bulunamadı!";
        protected override string RecordUpdated => Lang == Lang.EN ? "Role updated successfully." : "Rol başarıyla güncellendi.";
        protected override string RecordDeleted => Lang == Lang.EN ? "Role deleted successfully." : "Rol başarıyla silindi.";
        protected override string RelationalRecordsFound => Lang == Lang.EN ? "Relational users found!" : "İlişkili kullanıcılar bulunmaktadır!";
        public override string ViewModelName => Lang == Lang.TR ? "Rol" : "Role";
        protected override bool ViewPageOrder => true;

        public RoleService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
        }

        protected override IQueryable<Role> Records()
        {
            return base.Records().Include(r => r.UserRoles).ThenInclude(ur => ur.User);
        }

        public override void Delete(int id, bool save = true)
        {
            Validate(Records(id).UserRoles);
            base.Delete(id, save);
        }
    }
}
