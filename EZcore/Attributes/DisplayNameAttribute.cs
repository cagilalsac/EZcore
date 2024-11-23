#nullable disable

namespace EZcore.Attributes
{
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public DisplayNameAttribute(string displayNameTR, string displayNameEN = "")
        {
            if (string.IsNullOrWhiteSpace(displayNameEN))
                DisplayNameValue = displayNameTR;
            else
                DisplayNameValue = "{" + displayNameTR + ";" + displayNameEN + "}";
        }
    }
}
