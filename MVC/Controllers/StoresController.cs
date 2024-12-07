#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using EZcore.Services;
using EZcore.Models;
using EZcore.Controllers;
using BLL.Models;
using BLL.DAL;

// Generated from EZcore Template.

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin,EZcodeAdmin")]
    public class StoresController : MvcController
    {
        // Service injections:
        private readonly Service<Store, StoreModel> _storeService;

        /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
        //private readonly Service<{Entity}, {Entity}Model> _{Entity}Service;

        public StoresController(Service<Store, StoreModel> storeService

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //, Service<{Entity}, {Entity}Model> {Entity}Service
        )
        {
            _storeService = storeService;
            _storeService.ViewModelName = _storeService.Lang == Lang.EN ? "Store" : "Mağaza";
            _storeService.UsePageOrder = false;
            _storeService.ExcelFileNameWithoutExtension = _storeService.Lang == Lang.EN ? "Report" : "Rapor";

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //_{Entity}Service = {Entity}Service;
        }

        private void SetViewData(Lang lang = Lang.TR, PageOrder pageOrder = null)
        {
            base.SetViewData(lang, _storeService.ViewModelName, pageOrder);
            
            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //ViewBag.{Entity}s = new MultiSelectList(_{Entity}Service.Get(), "Record.Id", "Name");
        }

        // GET: Stores
        //[AllowAnonymous]
        public IActionResult Index(PageOrder pageOrder)
        {
            // Get collection service logic:
            var list = _storeService.Get(pageOrder);
            
            Message = _storeService.Message;
            SetViewData(_storeService.Lang, pageOrder);
            return View(list);
        }

        // GET: Stores/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _storeService.Get(id);

            Message = _storeService.Message;
            SetViewData(_storeService.Lang);
            return View(item);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            SetViewData(_storeService.Lang);
            return View();
        }

        // POST: Stores/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(StoreModel store)
        {
            if (ModelState.IsValid && _storeService.Validate(store).IsSuccessful)
            {
                // Insert item service logic:
                _storeService.Create(store);
                
                Message = _storeService.Message;
                if (_storeService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = store.Record.Id });
            }
            Message = _storeService.Message;
            SetViewData(_storeService.Lang);
            return View(store);
        }

        // GET: Stores/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _storeService.Get(id);

            Message = _storeService.Message;
            SetViewData(_storeService.Lang);
            return View(item);
        }

        // POST: Stores/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(StoreModel store)
        {
            if (ModelState.IsValid && _storeService.Validate(store).IsSuccessful)
            {
                // Update item service logic:
                _storeService.Update(store);
                
                Message = _storeService.Message;
                if (_storeService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = store.Record.Id });
            }
            Message = _storeService.Message;
            SetViewData(_storeService.Lang);
            return View(store);
        }

        // GET: Stores/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _storeService.Get(id);

            Message = _storeService.Message;
            SetViewData(_storeService.Lang);
            return View(item);
        }

        // POST: Stores/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            _storeService.Delete(id);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _storeService.Delete(id);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/DeleteFile/5
        public IActionResult DeleteFile(int id)
        {
            // Delete file logic:
            _storeService.DeleteFiles(id);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Stores/Export
        public void Export()
        {
            _storeService.GetExcel();
        }
	}
}
