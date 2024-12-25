#nullable disable
using EZcore.DAL.Users;
using EZcore.Models;
using EZcore.Models.Users;
using EZcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers.Users
{
    [Authorize(Roles = "SystemAdmin")]
    public class RolesController : MvcController
    {
        // Service injections:
        private readonly Service<Role, RoleModel> _roleService;

        public RolesController(Service<Role, RoleModel> roleService)
        {
            _roleService = roleService;
        }

        private void SetViewData(Lang lang = Lang.TR, PageOrder pageOrder = null)
        {
            base.SetViewData(lang, _roleService.ViewModelName, pageOrder);
        }

        // GET: Roles
        public IActionResult Index(PageOrder pageOrder)
        {
            // Get collection service logic:
            var list = _roleService.Get(pageOrder);
            
            Message = _roleService.Message;
            SetViewData(_roleService.Lang, pageOrder);
            return View("_EZRolesIndex", list);
        }

        // GET: Roles/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _roleService.Get(id);

            Message = _roleService.Message;
            SetViewData(_roleService.Lang);
            return View("_EZRolesDetails", item);
        }

        // GET: Roles/CreateEdit
        public IActionResult CreateEdit(int id = 0)
        {
            RoleModel item;
            if (id == 0)
            {
                item = new RoleModel();
            }
            else
            {
                // Get item to edit service logic:
                item = _roleService.Get(id);
            }
            SetViewData(_roleService.Lang);
            return View("_EZRolesCreateEdit", item);
        }

        // POST: Roles/CreateEdit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateEdit(RoleModel role)
        {
            if (ModelState.IsValid && _roleService.Validate(role))
            {
                if (role.Record.Id == 0)
                {
                    // Insert item service logic:
                    _roleService.Create(role);
                }
                else
                {
                    // Update item service logic:
                    _roleService.Update(role);
                }

                if (_roleService.IsSuccessful)
                {
                    Message = _roleService.Message;
                    return RedirectToAction(nameof(Details), new { id = role.Record.Id });
                }
            }
            Message = _roleService.Message;
            SetViewData(_roleService.Lang);
            return View("_EZRolesCreateEdit", role);
        }

        // GET: Roles/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _roleService.Delete(id);

            Message = _roleService.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
