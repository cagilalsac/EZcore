#nullable disable

namespace EZcore.Attributes
{
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public DisplayNameAttribute(string displayNameEN, string displayNameTR = "")
        {
            if (Thread.CurrentThread.CurrentCulture.Name == "tr-TR")
            {
                if (string.IsNullOrWhiteSpace(displayNameTR))
                    DisplayNameValue = displayNameEN;
                else
                    DisplayNameValue = displayNameTR;
            }
            else
            {
                DisplayNameValue = displayNameEN;
            }
        }
    }
}
