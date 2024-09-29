#nullable disable
using BLL.DAL;
using BLL.Models;
using EZcore.Controllers;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// Generated from EZcore Template.

namespace MVC.Controllers
{
    public class ProductsController : MvcController
    {
        // Views may be configured here by overriding the base controller properties starting with "View", "Lang" base property can also be overriden if necessary:
        protected override string ViewModelName => Lang == Lang.TR ? "Ürün" : "Product";
        protected override bool ViewPageOrder => true;
        
        // Service injections:
        private readonly ServiceBase<Product, ProductModel> _productService;
        private readonly ServiceBase<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
        private readonly ServiceBase<Store, StoreModel> _storeService;

        public ProductsController(HttpServiceBase httpService
			, ServiceBase<Product, ProductModel> productService
            , ServiceBase<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            , ServiceBase<Store, StoreModel> storeService
        ) : base(httpService)
        {
            _productService = productService;
            _productService.Lang = Lang;
            _categoryService = categoryService;

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            _storeService = storeService;
        }

        protected override void SetViewData(string message = null)
        {
            base.SetViewData(message);
            
            // Related items service logic to set ViewData (Record.Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            ViewData["CategoryId"] = new SelectList(_categoryService.Read(), "Record.Id", "Name");

            /* Can be uncommented and used for many to many relationships. * must be replaced with the related entiy name in the controller and views. */
            ViewBag.StoreIds = new MultiSelectList(_storeService.Read(), "Record.Id", "Name");
        }

        // GET: Products
        public IActionResult Index(PageOrder pageOrder = null)
        {
            PageOrder = pageOrder;

            // Adding order expressions if needed:
            PageOrder?.AddOrderExpression("Stock Amount", "Stok Miktarı");
            PageOrder?.AddOrderExpression("Unit Price", "Birim Fiyatı");
            PageOrder?.AddOrderExpression("Expiration Date", "Son Kullanma Tarihi");
            PageOrder?.AddOrderExpression("Name", "Adı");

            // Get collection service logic:
            var list = _productService.Read(PageOrder);

            SetViewData(_productService.Message);
            return View(list);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _productService.Read(id);

            SetViewData(_productService.Message);
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
                var result = _productService.Create(product.Record);
                
                SetViewData(result.Message);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
            }
            else
            {
                SetViewData();
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _productService.Read(id);

            SetViewData(_productService.Message);
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
                var result = _productService.Update(product.Record);
                
                SetViewData(result.Message);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
            }
            else
            {
                SetViewData();
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _productService.Read(id);

            SetViewData(_productService.Message);
            return View(item);
        }

        // POST: Products/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var result = _productService.Delete(id);

            SetViewData(result.Message);
            return RedirectToAction(nameof(Index), new { pageordersession = true });
        }
	}
}
