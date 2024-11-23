#nullable disable

using EZcore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EZcore.Extensions.TagHelpers
{
    [HtmlTargetElement("validation", Attributes = "asp-for,asp-lang")]
    public class ValidationTagHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        [HtmlAttributeName("asp-lang")]
        public Lang AspLang { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var modelName = AspFor.Name;
            var modelState = ViewContext.ViewData.ModelState;
            if (modelState.TryGetValue(modelName, out var entry) && entry.Errors.Count > 0)
            {
                var errorMessage = entry.Errors[0].ErrorMessage;
                errorMessage = GetErrorMessage(errorMessage, AspLang);
                output.TagName = "span";
                output.Content.SetHtmlContent(errorMessage);
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
