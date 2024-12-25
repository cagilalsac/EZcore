#nullable disable
using BLL.DAL;
using BLL.Models;
using EZcore.Controllers;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Generated from EZcore Template.

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
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
            if (ModelState.IsValid && _storeService.Validate(store))
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
            if (ModelState.IsValid && _storeService.Validate(store))
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

        public IActionResult DeleteFile(int id, string path = null)
        {
            // Delete file logic:
            _storeService.DeleteFiles(id, path);

            Message = _storeService.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        public void Export()
        {
            _storeService.GetExcel();
        }

        public IActionResult Download(string path)
        {
            var file = _storeService.GetFile(path);
            if (_storeService.IsSuccessful)
                return File(file.Stream, file.ContentType, file.Name);
            return View("_EZMessage", _storeService.Message);
        }
    }
}
