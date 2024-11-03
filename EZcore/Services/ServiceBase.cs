#nullable disable

using EZcore.Models;

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
                OperationFailed = _lang == Lang.TR ? "İşlem gerçekleştirilemedi!" : "Operation failed!";
                OperationSuccessful = _lang == Lang.TR ? "İşlem başarıyla gerçekleştirildi." : "Operation successful.";
            }
        }

        public ServiceBase Success(string message = "")
        {
            IsSuccessful = true;
            Message = message;
            return this;
        }

        public ServiceBase Error(string message = "")
        {
            IsSuccessful = false;
            Message = message;
            return this;
        }
    }
}
