#nullable disable

namespace EZcore.Attributes
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var valid = base.IsValid(value);
            if (valid && value is not null && value is List<int> && !(value as List<int>).Any())
                valid = false;
            if (!valid && string.IsNullOrWhiteSpace(ErrorMessage))
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "tr-TR")
                    ErrorMessage = "Bu alan zorunludur!";
                else
                    ErrorMessage = "This field is required!";
            }
            return valid;
        }
    }
}
