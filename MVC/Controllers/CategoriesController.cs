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
    public class CategoriesController : MvcController
    {
        // Service injections:
        private readonly Service<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
        //private readonly Service<{Entity}, {Entity}Model> _{Entity}Service;

        public CategoriesController(Service<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //, Service<{Entity}, {Entity}Model> {Entity}Service
        )
        {
            _categoryService = categoryService;

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //_{Entity}Service = {Entity}Service;
        }

        private void SetViewData(Lang lang = Lang.TR, PageOrder pageOrder = null)
        {
            base.SetViewData(lang, _categoryService.ViewModelName, pageOrder);
            
            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            //ViewBag.{Entity}s = new MultiSelectList(_{Entity}Service.Get(), "Record.Id", "Name");
        }

        // GET: Categories
        //[AllowAnonymous]
        public IActionResult Index(PageOrder pageOrder)
        {
            // Get collection service logic:
            var list = _categoryService.Get(pageOrder);
            
            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang, pageOrder);
            return View(list);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _categoryService.Get(id);

            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang);
            return View(item);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            SetViewData(_categoryService.Lang);
            return View();
        }

        // POST: Categories/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel category)
        {
            if (ModelState.IsValid && _categoryService.Validate(category))
            {
                // Insert item service logic:
                _categoryService.Create(category);
                
                Message = _categoryService.Message;
                if (_categoryService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang);
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _categoryService.Get(id);

            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang);
            return View(item);
        }

        // POST: Categories/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid && _categoryService.Validate(category))
            {
                // Update item service logic:
                _categoryService.Update(category);
                
                Message = _categoryService.Message;
                if (_categoryService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang);
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _categoryService.Get(id);

            Message = _categoryService.Message;
            SetViewData(_categoryService.Lang);
            return View(item);
        }

        // POST: Categories/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            _categoryService.Delete(id);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _categoryService.Delete(id);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteFile(int id, string path = null)
        {
            // Delete file logic:
            _categoryService.DeleteFiles(id, path);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        public void Export()
        {
            _categoryService.GetExcel();
        }

        public IActionResult Download(string path)
        {
            var file = _categoryService.GetFile(path);
            if (_categoryService.IsSuccessful)
                return File(file.Stream, file.ContentType, file.Name);
            return View("_EZMessage", _categoryService.Message);
        }
    }
}
