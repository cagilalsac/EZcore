#nullable disable

using EZcore.Models;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
    public abstract class MvcController : Controller
    {
        /// <summary>
        /// End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the view.
        /// </summary>
        protected virtual string Message
        {
            get => TempData["Message"]?.ToString();
            set => TempData["Message"] = (value + "<br>" + TempData["Message"]).Trim("<br>".ToCharArray());
        }

        protected virtual void SetViewData(Lang lang, string viewModelName = "Kayıt", PageOrder pageOrder = null)
        {
            ViewData[nameof(Lang)] = lang;
            ViewData["ViewModelName"] = viewModelName;
            ViewData[nameof(PageOrder)] = pageOrder is not null && pageOrder.PageNumber != 0 ? pageOrder : null;
        }
    }
}
