#nullable disable

using EZcore.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace EZcore.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent DisplayNameFor<TModel, TProperty>(this IHtmlHelper<TModel> helper,
                Expression<Func<TModel, TProperty>> expression, Lang lang = Lang.TR)
        {
            ModelExpressionProvider modelExpressionProvider = (ModelExpressionProvider)helper.ViewContext.HttpContext.RequestServices
                .GetService(typeof(ModelExpressionProvider));
            ModelExpression modelExpression = modelExpressionProvider.CreateModelExpression(helper.ViewData, expression);
            string displayName;
            if (!string.IsNullOrWhiteSpace(modelExpression.Metadata.DisplayName))
            {
                displayName = modelExpression.Metadata.DisplayName;
            }
            else if (!string.IsNullOrWhiteSpace(modelExpression.Metadata.PropertyName))
            {
                displayName = modelExpression.Metadata.PropertyName;
            }
            else
            {
                displayName = modelExpression.Metadata.Name;
            }
            displayName = displayName.GetDisplayName(lang);
            TagBuilder labelTag = new TagBuilder("label");
            labelTag.Attributes.Add("for", helper.IdFor(expression).ToString());
            labelTag.Attributes.Add("style", "cursor:pointer");
            labelTag.InnerHtml.AppendHtml(displayName);
            return labelTag;
        }

        public static IHtmlContent ValidationMessageFor<TModel, TProperty>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, Lang lang = Lang.TR)
        {
            string modelName = expression.Body.ToString();
            if (modelName.Contains("."))
                modelName = modelName.Remove(0, modelName.IndexOf(".") + 1);
            var modelState = helper.ViewContext.ModelState;
            TagBuilder spanTag = new TagBuilder("span");
            if (modelState.TryGetValue(modelName, out var entry) && entry.Errors.Count > 0)
            {
                var errorMessage = entry.Errors[0].ErrorMessage;
                errorMessage = errorMessage.GetErrorMessage(lang);
                spanTag.Attributes.Add("class", "text-danger");
                spanTag.InnerHtml.AppendHtml(errorMessage);
            }
            return spanTag;
        }
    }
}
