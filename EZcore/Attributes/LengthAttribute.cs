#nullable disable

namespace EZcore.Attributes
{
    public class LengthAttribute : System.ComponentModel.DataAnnotations.LengthAttribute
    {
        public LengthAttribute(int maximumLength, int minimumLength = 0) : base(minimumLength, maximumLength)
        {
        }

        public override bool IsValid(object value)
        {
            var valid = base.IsValid(value);
            if (!valid && string.IsNullOrWhiteSpace(ErrorMessage))
            {
                string valueAsString = Convert.ToString(value) ?? "0";
                if (Thread.CurrentThread.CurrentCulture.Name == "tr-TR")
                {
                    if (MinimumLength > 0)
                    {
                        ErrorMessage = $"Bu alan en az {MinimumLength} en çok {MaximumLength} karakter olmalıdır,";
                    }
                    else
                    {
                        ErrorMessage = $"Bu alan en çok {MaximumLength} karakter olmalıdır,";
                    }
                    ErrorMessage += $" {valueAsString.Length} karakter girilmiştir!";
                }
                else
                {
                    if (MinimumLength > 0)
                    {
                        ErrorMessage = $"This field must have minimum {MinimumLength} maximum {MaximumLength} characters,";
                    }
                    else
                    {
                        ErrorMessage = $"This field must have maximum {MaximumLength} characters,";
                    }
                    ErrorMessage += $" {valueAsString.Length} characters entered!";
                }
            }
            return valid;
        }
    }
}
