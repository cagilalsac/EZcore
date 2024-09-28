#nullable disable

using EZcore.Models;

namespace EZcore.Services
{
    public abstract class ResultServiceBase
    {
        public bool IsSuccessful { get; private set; }
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

        public ResultServiceBase Success(string message = "")
        {
            IsSuccessful = true;
            Message = message;
            return this;
        }

        public ResultServiceBase Error(string message = "")
        {
            IsSuccessful = false;
            Message = message;
            return this;
        }
    }
}
