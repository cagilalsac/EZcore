#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EZcore.Controllers;
using EZcore.Services;
using EZcore.Models;
using BLL.Models;
using BLL.DAL;

// Generated from EZcore Template.

namespace MVC.Controllers
{
    public class StoresController : MvcController
    {
        // Views may be configured here by overriding the base controller properties starting with "View", "Lang" base property can also be overriden if necessary:
        protected override string ViewModelName => Lang == Lang.TR ? "Mağaza" : "Store";
        protected override bool ViewPageOrder => false;
        
        // Service injections:
        private readonly Service<Store, StoreModel> _storeService;

        /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
        //private readonly Service<*, *Model> _*Service;

        public StoresController(HttpServiceBase httpService
			, Service<Store, StoreModel> storeService

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //, Service<*, *Model> *Service
        ) : base(httpService)
        {
            _storeService = storeService;
            _storeService.Lang = Lang;

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //_*Service = *Service;
        }

        protected override void SetViewData()
        {
            base.SetViewData();
            
            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //ViewBag.*Ids = new MultiSelectList(_*Service.Read(), "Record.Id", "Name");
        }

        // GET: Stores
        public IActionResult Index(PageOrder pageOrder = null)
        {
            PageOrder = pageOrder;

            // Get collection service logic:
            var list = _storeService.Read(PageOrder);
            
            Message = _storeService.Message;
            SetViewData();
            return View(list);
        }

        // GET: Stores/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _storeService.Read(id);

            Message = _storeService.Message;
            SetViewData();
            return View(item);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                // Insert item service logic:
                _storeService.Create(store.Record);
                
                Message = _storeService.Message;
                if (_storeService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = store.Record.Id });
            }
            SetViewData();
            return View(store);
        }

        // GET: Stores/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _storeService.Read(id);

            Message = _storeService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Stores/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                // Update item service logic:
                _storeService.Update(store.Record);
                
                Message = _storeService.Message;
                if (_storeService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = store.Record.Id });
            }
            SetViewData();
            return View(store);
        }

        // GET: Stores/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _storeService.Read(id);

            Message = _storeService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Stores/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            _storeService.Delete(id);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }

        // GET: Stores/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _storeService.Delete(id);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }
	}
}
