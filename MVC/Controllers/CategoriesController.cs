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
    public class CategoriesController : MvcController
    {
        // Views may be configured here by overriding the base controller properties starting with "View", "Lang" base property can also be overriden if necessary:
        protected override string ViewModelName => Lang == Lang.TR ? "Kategori" : "Category";
        protected override bool ViewPageOrder => false;
        
        // Service injections:
        private readonly Service<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
        //private readonly Service<*, *Model> _*Service;

        public CategoriesController(HttpServiceBase httpService
			, Service<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //, Service<*, *Model> *Service
        ) : base(httpService)
        {
            _categoryService = categoryService;
            _categoryService.Lang = Lang;

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //_*Service = *Service;
        }

        protected override void SetViewData()
        {
            base.SetViewData();
            
            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            //ViewBag.*Ids = new MultiSelectList(_*Service.Read(), "Record.Id", "Name");
        }

        // GET: Categories
        public IActionResult Index(PageOrder pageOrder = null)
        {
            PageOrder = pageOrder;

            // Get collection service logic:
            var list = _categoryService.Read(PageOrder);
            
            Message = _categoryService.Message;
            SetViewData();
            return View(list);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _categoryService.Read(id);

            Message = _categoryService.Message;
            SetViewData();
            return View(item);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Insert item service logic:
                _categoryService.Create(category.Record);
                
                Message = _categoryService.Message;
                if (_categoryService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            SetViewData();
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _categoryService.Read(id);

            Message = _categoryService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Update item service logic:
                _categoryService.Update(category.Record);
                
                Message = _categoryService.Message;
                if (_categoryService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            SetViewData();
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _categoryService.Read(id);

            Message = _categoryService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Categories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            _categoryService.Delete(id);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }

        // GET: Categories/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _categoryService.Delete(id);

            Message = _categoryService.Message;
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }
	}
}
