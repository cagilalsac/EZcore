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
            _categoryService.ViewModelName = _categoryService.Lang == Lang.EN ? "Category" : "Kategori";
            _categoryService.UsePageOrder = false;
            _categoryService.ExcelFileNameWithoutExtension = _categoryService.Lang == Lang.EN ? "Report" : "Rapor";

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
            if (ModelState.IsValid && _categoryService.Validate(category).IsSuccessful)
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
            if (ModelState.IsValid && _categoryService.Validate(category).IsSuccessful)
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

        // GET: Categories/DeleteFile/5
        public IActionResult DeleteFile(int id)
        {
            // Delete file logic:
            _categoryService.DeleteFile(id);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Categories/Export
        public void Export()
        {
            _categoryService.GetExcel();
        }
	}
}
