using EZcore.Controllers;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class HomeController : MvcController
    {
        private readonly ServiceBase _service;

        public HomeController(ServiceBase service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            SetViewData(_service.Lang);
            return View();
        }

        public IActionResult About()
        {
            SetViewData(_service.Lang);
            return View();
        }

        public IActionResult Error()
        {
            SetViewData(_service.Lang);
            return View("_EZMessage", _service.Lang == Lang.EN ? 
                "An error occurred while processing your request!" :
                "Ýþlem sýrasýnda hata meydana geldi!");
        }
    }
}
