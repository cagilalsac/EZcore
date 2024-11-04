#nullable disable

using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
    public abstract class MvcController : Controller
    {
        protected virtual string ViewModelName { get; }
        protected virtual bool ViewPageOrder { get; }

        protected virtual Lang Lang { get; }
        
        protected virtual Dictionary<string, string> Languages => new Dictionary<string, string>()
        {
            { Lang.EN.ToString(), "English" },
            { Lang.TR.ToString(), "Türkçe" }
        };

        private readonly HttpServiceBase _httpService;

        protected MvcController(HttpServiceBase httpService)
        {
            Lang = Lang.EN;
            _httpService = httpService;
            ViewModelName = Lang == Lang.EN ? "Record" : "Kayıt";
        }

        private PageOrder _pageOrder;
        protected PageOrder PageOrder 
        { 
            get
            {
                if (ViewPageOrder)
                    _httpService.SetSession(this.ToString().LastOrDefault() + "PageOrderSessionKey", _pageOrder);
                ViewData[nameof(PageOrder)] = _pageOrder;
                return _pageOrder;
            }
            set
            {
                _pageOrder = null;
                if (ViewPageOrder)
                {
                    _pageOrder = value;
                    if (_pageOrder.PageOrderSession)
                    {
                        var pageOrderFromSession = _httpService.GetSession<PageOrder>(this.ToString().LastOrDefault() + "PageOrderSessionKey");
                        if (pageOrderFromSession is not null)
                        {
                            _pageOrder.PageNumber = pageOrderFromSession.PageNumber;
                            _pageOrder.RecordsPerPageCount = pageOrderFromSession.RecordsPerPageCount;
                            _pageOrder.OrderExpression = pageOrderFromSession.OrderExpression;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the view.
        /// </summary>
        protected string Message 
        { 
            get => TempData["Message"]?.ToString(); 
            set => TempData["Message"] = (value + "<br>" + TempData["Message"]).Trim("<br>".ToCharArray());
        }

        protected virtual void SetViewData()
        {
            ViewData[nameof(Lang)] = Lang;
            ViewData[nameof(Languages)] = Languages;
            ViewData[nameof(ViewModelName)] = ViewModelName;
        }
    }
}
