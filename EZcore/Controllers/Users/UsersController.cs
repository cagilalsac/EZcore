#nullable disable

using EZcore.DAL.Users;
using EZcore.Models;
using EZcore.Models.Users;
using EZcore.Services;
using EZcore.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EZcore.Controllers.Users
{
    [Authorize(Roles = "SystemAdmin")]
    public class UsersController : MvcController
    {
        // Service injections:
        private readonly Service<User, UserModel> _userService;
        private readonly Service<Role, RoleModel> _roleService;

        public UsersController(Service<User, UserModel> userService, Service<Role, RoleModel> roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        private void SetViewData(Lang lang = Lang.TR, PageOrder pageOrder = null)
        {
            base.SetViewData(lang, _userService.ViewModelName, pageOrder);
            ViewBag.Roles = new MultiSelectList(_roleService.Get(), "Record.Id", "Name");
        }

        // GET: Users
        public IActionResult Index(PageOrder pageOrder)
        {
            // Get collection service logic:
            var list = _userService.Get(pageOrder);

            Message = _userService.Message;
            SetViewData(_userService.Lang, pageOrder);
            return View("_EZUsersIndex", list);
        }

        // GET: Users/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _userService.Get(id);

            Message = _userService.Message;
            SetViewData(_userService.Lang);
            return View("_EZUsersDetails", item);
        }

        // GET: Users/CreateEdit
        public IActionResult CreateEdit(int id = 0)
        {
            UserModel item;
            if (id == 0)
            {
                item = new UserModel() { Record = new User() { IsActive = true } };
            }
            else
            {
                // Get item to edit service logic:
                item = _userService.Get(id);
            }
            SetViewData(_userService.Lang);
            return View("_EZUsersCreateEdit", item);
        }

        // POST: Users/CreateEdit
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateEdit(UserModel user)
        {
            if (ModelState.IsValid && _userService.Validate(user))
            {
                if (user.Record.Id == 0)
                {
                    // Insert item service logic:
                    _userService.Create(user);
                }
                else
                {
                    // Update item service logic:
                    _userService.Update(user);
                }

                if (_userService.IsSuccessful)
                {
                    Message = _userService.Message;
                    return RedirectToAction(nameof(Details), new { id = user.Record.Id });
                }
            }
            Message = _userService.Message;
            SetViewData(_userService.Lang);
            return View("_EZUsersCreateEdit", user);
        }

        // GET: Users/DeleteByAlertify/5
        public IActionResult DeleteByAlertify(int id)
        {
            // Delete item service logic:
            _userService.Delete(id);

            Message = _userService.Message;
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                Message = (_userService as UserService).NoAccess;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZUsersLogin");
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<IActionResult> Login(UserModel user)
        {
            var userService = _userService as UserService;
            ModelState.Remove("Record.Roles");
            if (ModelState.IsValid)
            {
                await userService.SignInAsync(user);
                if (userService.IsSuccessful)
                    return RedirectToAction("Index", "Home");
            }
            Message = _userService.Message;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZUsersLogin");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var userService = _userService as UserService;
            await userService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZUsersRegister");
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public IActionResult Register(UserModel user)
        {
            var userService = _userService as UserService;
            ModelState.Remove("Record.Roles");
            if (ModelState.IsValid && userService.Validate(user))
            {
                userService.Register(user);
                if (userService.IsSuccessful)
                    return RedirectToAction(nameof(Login));
            }
            Message = _userService.Message;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZUsersRegister", user);
        }
    }
}
