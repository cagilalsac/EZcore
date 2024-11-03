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
    public class ProductsController : MvcController
    {
        // Views may be configured here by overriding the base controller properties starting with "View", "Lang" base property can also be overriden if necessary:
        protected override string ViewModelName => Lang == Lang.TR ? "Ürün" : "Product";
        protected override bool ViewPageOrder => true;
        
        // Service injections:
        private readonly Service<Product, ProductModel> _productService;
        private readonly Service<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
        private readonly Service<Store, StoreModel> _storeService;

        public ProductsController(HttpServiceBase httpService
			, Service<Product, ProductModel> productService
            , Service<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            , Service<Store, StoreModel> storeService
        ) : base(httpService)
        {
            _productService = productService;
            _productService.Lang = Lang;
            _categoryService = categoryService;

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            _storeService = storeService;
        }

        protected override void SetViewData()
        {
            base.SetViewData();
            
            // Related items service logic to set ViewData (Record.Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            ViewData["CategoryId"] = new SelectList(_categoryService.Read(), "Record.Id", "Name");

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            ViewBag.StoreIds = new MultiSelectList(_storeService.Read(), "Record.Id", "StoreName");
        }

        // GET: Products
        public IActionResult Index(PageOrder pageOrder = null)
        {
            PageOrder = pageOrder;

            // Get collection service logic:
            var list = _productService.Read(PageOrder);
            
            Message = _productService.Message;
            SetViewData();
            return View(list);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _productService.Read(id);

            Message = _productService.Message;
            SetViewData();
            return View(item);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                // Insert item service logic:
                _productService.Create(product.Record);
                
                Message = _productService.Message;
                if (_productService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
            }
            SetViewData();
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _productService.Read(id);

            Message = _productService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                // Update item service logic:
                _productService.Update(product.Record);
                
                Message = _productService.Message;
                if (_productService.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
            }
            SetViewData();
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _productService.Read(id);

            Message = _productService.Message;
            SetViewData();
            return View(item);
        }

        // POST: Products/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            _productService.Delete(id);

            Message = _productService.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _productService.Delete(id);

            Message = _productService.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
