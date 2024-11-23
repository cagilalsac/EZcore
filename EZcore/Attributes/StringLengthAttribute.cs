#nullable disable

namespace EZcore.Attributes
{
    public class StringLengthAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
    {
        private string _errorMessageTR, _errorMessageEN;

        public StringLengthAttribute(int maximumLength, string errorMessageTR = "", string errorMessageEN = "") : base(maximumLength)
        {
            _errorMessageTR = errorMessageTR;
            _errorMessageEN = errorMessageEN;
        }

        public override bool IsValid(object value)
        {
            var valid = base.IsValid(value);
            if (!valid)
            {
                string valueAsString = Convert.ToString(value) ?? "0";
                string emptyErrorMessageTR, emptyErrorMessageEN;
                if (MinimumLength > 0)
                {
                    emptyErrorMessageTR = $"Bu alan en az {MinimumLength} en çok {MaximumLength} karakter olmalıdır,";
                    emptyErrorMessageEN = $"This field must have minimum {MinimumLength} maximum {MaximumLength} characters,";
                }
                else
                {
                    emptyErrorMessageTR = $"Bu alan en çok {MaximumLength} karakter olmalıdır,";
                    emptyErrorMessageEN = $"This field must have maximum {MaximumLength} characters,";
                }
                emptyErrorMessageTR += $" {valueAsString.Length} karakter girilmiştir!";
                emptyErrorMessageEN += $" {valueAsString.Length} characters entered!";
                if (string.IsNullOrWhiteSpace(_errorMessageTR) && string.IsNullOrWhiteSpace(_errorMessageEN))
                    ErrorMessage = $"{emptyErrorMessageTR};{emptyErrorMessageEN}";
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
