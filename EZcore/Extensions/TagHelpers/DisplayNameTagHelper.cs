#nullable disable

using EZcore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EZcore.Extensions.TagHelpers
{
    [HtmlTargetElement("displayname", Attributes = "asp-for,asp-lang")]
    public class DisplayNameTagHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        [HtmlAttributeName("asp-lang")]
        public Lang AspLang { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ModelMetadata aspForMetadata = AspFor.Metadata;
            string displayName;
            if (!string.IsNullOrWhiteSpace(aspForMetadata.DisplayName))
            {
                displayName = aspForMetadata.DisplayName;
            }
            else if (!string.IsNullOrWhiteSpace(aspForMetadata.PropertyName))
            {
                displayName = aspForMetadata.PropertyName;
            }
            else
            {
                displayName = aspForMetadata.Name;
            }
            displayName = GetDisplayName(displayName, AspLang);
            output.TagName = "label";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(displayName);
        }
    }
}
