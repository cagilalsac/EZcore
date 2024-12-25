#nullable disable

using EZcore.DAL;
using EZcore.DAL.Users;
using EZcore.Models;
using EZcore.Models.JsonWebToken;
using EZcore.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;

namespace EZcore.Services.Users
{
    public class UserService : Service<User, UserModel>
    {
        protected override string RecordFound => Lang == Lang.EN ? "user found." : "kullanıcı bulundu.";
        protected override string RecordsFound => Lang == Lang.EN ? "users found." : "kullanıcı bulundu.";
        protected override string RecordWithSameNameExists => Lang == Lang.EN ? " User with the same user name exists!" : " Aynı kullanıcı adına sahip kullanıcı bulunmaktadır!";
        protected override string RecordCreated => Lang == Lang.EN ? "User created successfully." : "Kullanıcı başarıyla oluşturuldu.";
        protected override string RecordNotFound => Lang == Lang.EN ? "User not found!" : "Kullanıcı bulunamadı!";
        protected override string RecordUpdated => Lang == Lang.EN ? "User updated successfully." : "Kullanıcı başarıyla güncellendi.";
        protected override string RecordDeleted => Lang == Lang.EN ? "User deleted successfully." : "Kullanıcı başarıyla silindi.";
        public override string ViewModelName => Lang == Lang.TR ? "Kullanıcı" : "User";
        protected override bool ViewPageOrder => true;

        public string EMailNotValid { get; set; }
        public string UserNotValid { get; set; }
        public string PasswordNotValid { get; set; }
        public string EMailRegex { get; set; } = "^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";
        public string NoAccess { get; set; }

        public UserService(IDb db, HttpServiceBase httpService) : base(db, httpService)
        {
            EMailNotValid = Lang == Lang.EN ? "Invalid e-mail address!" : "Geçersiz e-posta adresi!";
            UserNotValid = Lang == Lang.EN ? "Invalid user name or password!" : "Geçersiz kullanıcı adı veya şifre!";
            PasswordNotValid = Lang == Lang.EN ? "Password and confirm password must be the same!" : "Şifre ve şifre onay aynı olmalıdır!";
            NoAccess = Lang == Lang.EN ? "You have no access to this operation!" : "Bu işleme yetkiniz bulunmamaktadır!";
            AddPageOrderExpression("User Name", "Kullanıcı Adı");
            AddPageOrderExpression("Is Active", "Aktif Mi");
        }

        protected override IQueryable<User> Records()
        {
            return base.Records().Include(u => u.UserRoles).ThenInclude(ur => ur.Role).OrderBy(u => u.UserName);
        }

        public override bool Validate(UserModel model)
        {
            if (Records().Any(u => u.Id != model.Record.Id && EF.Functions.Collate(u.UserName, Collation) == EF.Functions.Collate(model.Record.UserName, Collation).Trim()))
            {
                Error(RecordWithSameNameExists);
            }
            else
            {
                Regex regex = new Regex(EMailRegex);
                if (!string.IsNullOrWhiteSpace(model.Record.EMail) && !regex.IsMatch(model.Record.EMail.Trim()))
                    Error(EMailNotValid);
                else if (!string.IsNullOrWhiteSpace(model.ConfirmPassword) && model.ConfirmPassword != model.Record.Password)
                    Error(PasswordNotValid);
            }
            return base.Validate(model);
        }

        public override void Update(UserModel model, bool save = true)
        {
            Update(Records(model.Record.Id).UserRoles);
            base.Update(model, save);
        }

        public override void Delete(int id, bool save = true)
        {
            Delete(Records(id).UserRoles);
            base.Delete(id, save);
        }

        public virtual UserModel Get(string userName, string password)
        {
            return Records().Where(u => u.IsActive && u.UserName == userName && u.Password == password)
                .Select(u => new UserModel() { Record = u }).SingleOrDefault();
        }

        public virtual async Task SignInAsync(UserModel model)
        {
            var user = Get(model.Record.UserName, model.Record.Password);
            if (user is null)
            {
                Error(UserNotValid);
                return;
            }
            await _httpService.SignInAsync(user);
        }

        public virtual async Task SignOutAsync()
        {
            await _httpService.SignOutAsync();
        }

        public virtual void Register(UserModel model)
        {
            model.Record.Roles = [(int)Roles.User];
            model.Record.IsActive = true;
            base.Create(model);
        }

        public virtual Jwt GetJwt(UserModel model)
        {
            var user = Get(model.Record.UserName, model.Record.Password);
            if (user is null)
            {
                Error(UserNotValid);
                return null;
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, JwtSettings.SecurityAlgorithm);
            var expiration = DateTime.Now.AddMinutes(JwtSettings.ExpirationInMinutes);
            var jwtSecurityToken = new JwtSecurityToken(JwtSettings.Issuer, JwtSettings.Audience, user.Claims, DateTime.Now, expiration, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            return new Jwt()
            {
                Token = "Bearer " + token,
                Expiration = expiration
            };
        }
    }
}
