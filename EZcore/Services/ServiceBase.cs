#nullable disable

using EZcore.Models;
using System.Globalization;

namespace EZcore.Services
{
    public abstract class ServiceBase
    {
        public bool IsSuccessful { get; private set; } = true;
        public string Message { get; private set; } = "";

        protected virtual string OperationFailed { get; private set; }
        protected virtual string OperationSuccessful { get; private set; }

        private Lang _lang;
        public virtual Lang Lang
        {
            get => _lang;
            set
            {
                _lang = value;
                Thread.CurrentThread.CurrentCulture = Lang == Lang.TR ? new CultureInfo("tr-TR") : new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = Lang == Lang.TR ? new CultureInfo("tr-TR") : new CultureInfo("en-US");
                OperationFailed = Lang == Lang.TR ? "İşlem gerçekleştirilemedi!" : "Operation failed!";
                OperationSuccessful = Lang == Lang.TR ? "İşlem başarıyla gerçekleştirildi." : "Operation successful.";
            }
        }

        public bool Api { get; set; }

        public virtual string ViewModelName => Lang == Lang.EN ? "Record" : "Kayıt";


        protected readonly HttpServiceBase _httpService;

        protected ServiceBase(HttpServiceBase httpService)
        {
            _httpService = httpService;
            if (Api)
                Lang = Lang.TR;
            else
                Lang = (Lang)int.Parse(_httpService.GetCookie(nameof(Lang)) ?? "0");
        }

        public void Success(string message = "")
        {
            IsSuccessful = true;
            Message = message;
        }

        public void Error(string message = "", bool operationFailed = true)
        {
            IsSuccessful = false;
            Message = operationFailed ? $"{OperationFailed} {message}" : message;
        }
    }
}
