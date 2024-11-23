using EZcore.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EZcore.Extensions.TagHelpers
{
    public abstract class TagHelperBase : TagHelper
    {
        protected virtual string GetDisplayName(string value, Lang lang)
        {
            return value.GetDisplayName(lang);
        }

        protected virtual string GetErrorMessage(string value, Lang lang)
        {
            return value.GetErrorMessage(lang);
        }
    }
}
