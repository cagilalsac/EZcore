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
        private readonly ServiceBase<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
        //private readonly Service<ManyToManyRecord, ManyToManyRecordModel> _ManyToManyRecordService;

        public CategoriesController(HttpServiceBase httpService
			, ServiceBase<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //, Service<ManyToManyRecord, ManyToManyRecordModel> ManyToManyRecordService
        ) : base(httpService)
        {
            _categoryService = categoryService;
            _categoryService.Lang = Lang;

            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //_ManyToManyRecordService = ManyToManyRecordService;
        }

        protected override void SetViewData(string message = null)
        {
            base.SetViewData(message);
            
            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //ViewBag.ManyToManyRecordIds = new MultiSelectList(_ManyToManyRecordService.Read(), "Record.Id", "Name");
        }

        // GET: Categories
        public IActionResult Index(PageOrder pageOrder = null)
        {
            PageOrder = pageOrder;

            // Adding order expressions if needed:
            PageOrder?.AddOrderExpression("Name", "Adı");

            // Get collection service logic:
            var list = _categoryService.Read(PageOrder);

            SetViewData(_categoryService.Message);
            return View(list);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _categoryService.Read(id);

            SetViewData(_categoryService.Message);
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
                var result = _categoryService.Create(category.Record);
                
                SetViewData(result.Message);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            else
            {
                SetViewData();
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _categoryService.Read(id);

            SetViewData(_categoryService.Message);
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
                var result = _categoryService.Update(category.Record);
                
                SetViewData(result.Message);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Record.Id });
            }
            else
            {
                SetViewData();
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _categoryService.Read(id);

            SetViewData(_categoryService.Message);
            return View(item);
        }

        // POST: Categories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var result = _categoryService.Delete(id);

            SetViewData(result.Message);
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }
	}
}
