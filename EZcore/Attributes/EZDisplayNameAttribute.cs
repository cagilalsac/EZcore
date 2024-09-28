#nullable disable

using System.ComponentModel;

namespace EZcore.Attributes
{
    public class EZDisplayNameAttribute : DisplayNameAttribute
    {
        public EZDisplayNameAttribute(string displayNameEN, string displayNameTR = "")
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
