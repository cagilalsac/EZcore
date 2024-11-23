#nullable disable

namespace EZcore.Attributes
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        private string _errorMessageTR, _errorMessageEN;

        public RequiredAttribute(string errorMessageTR = "", string errorMessageEN = "")
        {
            _errorMessageTR = errorMessageTR;
            _errorMessageEN = errorMessageEN;
        }

        public override bool IsValid(object value)
        {
            var valid = base.IsValid(value);
            if (valid && value is not null && value is List<int> && !(value as List<int>).Any())
                valid = false;
            if (!valid)
            {
                if (string.IsNullOrWhiteSpace(_errorMessageTR) && string.IsNullOrWhiteSpace(_errorMessageEN))
                    ErrorMessage = "Bu alan zorunludur!;This field is required!";
                else if (!string.IsNullOrWhiteSpace(_errorMessageTR) && string.IsNullOrWhiteSpace(_errorMessageEN))
                    ErrorMessage = _errorMessageTR;
                else if (string.IsNullOrWhiteSpace(_errorMessageTR) && !string.IsNullOrWhiteSpace(_errorMessageEN))
                    ErrorMessage = _errorMessageEN;
                else
                    ErrorMessage = $"{_errorMessageTR};{_errorMessageEN}";
            }
            return valid;
        }
    }
}
