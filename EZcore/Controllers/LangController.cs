using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
    public class LangController : Controller
    {
        private readonly HttpServiceBase _httpService;

        public LangController(HttpServiceBase httpService)
        {
            _httpService = httpService;
        }

        public IActionResult Set(int lang)
        {
            _httpService.SetCookie(nameof(Lang), lang.ToString());
            return RedirectToAction("Index", "Home");
        }
    }
}
