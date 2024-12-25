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
    [Authorize]
    public class ProductsController : MvcController
    {
        // Service injections:
        private readonly Service<Product, ProductModel> _productService;
        private readonly Service<Category, CategoryModel> _categoryService;

        /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
        private readonly Service<Store, StoreModel> _StoreService;

        public ProductsController(Service<Product, ProductModel> productService
            , Service<Category, CategoryModel> categoryService

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            , Service<Store, StoreModel> StoreService
        )
        {
            _productService = productService;
            _categoryService = categoryService;

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            _StoreService = StoreService;
        }

        private void SetViewData(Lang lang = Lang.TR, PageOrder pageOrder = null)
        {
            base.SetViewData(lang, _productService.ViewModelName, pageOrder);
            
            // Related items service logic to set ViewData (Record.Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            ViewData["CategoryId"] = new SelectList(_categoryService.Get(), "Record.Id", "Name");

            /* Can be uncommented and used for many to many relationships. Entity must be replaced with the related name in the controller and views. */
            ViewBag.Stores = new MultiSelectList(_StoreService.Get(), "Record.Id", "Name");
        }

        // GET: Products
        [AllowAnonymous]
        public IActionResult Index(PageOrder pageOrder)
        {
            // Get collection service logic:
            var list = _productService.Get(pageOrder);
            
            Message = _productService.Message;
            SetViewData(_productService.Lang, pageOrder);
            return View(list);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _productService.Get(id);

            Message = _productService.Message;
            SetViewData(_productService.Lang);
            return View(item);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            SetViewData(_productService.Lang);
            return View();
        }

        // POST: Products/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid && _productService.Validate(product))
            {
                // Insert item service logic:
                _productService.Create(product);
                
                if (_productService.IsSuccessful)
                {
                    Message = _productService.Message;
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
                }
            }
            Message = _productService.Message;
            SetViewData(_productService.Lang);
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _productService.Get(id);

            Message = _productService.Message;
            SetViewData(_productService.Lang);
            return View(item);
        }

        // POST: Products/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(ProductModel product)
        {
            if (ModelState.IsValid && _productService.Validate(product))
            {
                // Update item service logic:
                _productService.Update(product);
                
                if (_productService.IsSuccessful)
                {
                    Message = _productService.Message;
                    return RedirectToAction(nameof(Details), new { id = product.Record.Id });
                }
            }
            Message = _productService.Message;
            SetViewData(_productService.Lang);
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _productService.Get(id);

            Message = _productService.Message;
            SetViewData(_productService.Lang);
            return View(item);
        }

        // POST: Products/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
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

        public IActionResult DeleteFile(int id, string path = null)
        {
            // Delete file logic:
            _productService.DeleteFiles(id, path);

            Message = _productService.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        public void Export()
        {
            _productService.GetExcel();
        }

        public IActionResult Download(string path)
        {
            var file = _productService.GetFile(path);
            if (_productService.IsSuccessful)
                return File(file.Stream, file.ContentType, file.Name);
            return View("_EZMessage", _productService.Message); 
        }
	}
}
