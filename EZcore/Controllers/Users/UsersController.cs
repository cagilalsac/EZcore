#nullable disable

using EZcore.DAL.Users;
using EZcore.Models;
using EZcore.Models.Users;
using EZcore.Services;
using EZcore.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers.Users
{
    public class UsersController : MvcController
    {
        private readonly Service<User, UserModel> _userService;

        public UsersController(Service<User, UserModel> userService)
        {
            _userService = userService;
            _userService.ViewModelName = _userService.Lang == Lang.EN ? "User" : "Kullanıcı";
        }

        public IActionResult Login(string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                Message = (_userService as UserService).NoAccess;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZLogin");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel user)
        {
            var userService = _userService as UserService;
            if (ModelState.IsValid)
            {
                await userService.SignInAsync(user);
                if (userService.IsSuccessful)
                    return RedirectToAction("Index", "Home");
            }
            Message = _userService.Message;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZLogin");
        }

        public async Task<IActionResult> Logout()
        {
            var userService = _userService as UserService;
            await userService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZRegister");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Register(UserModel user)
        {
            var userService = _userService as UserService;
            if (ModelState.IsValid && userService.Validate(user).IsSuccessful)
            {
                userService.Register(user);
                if (userService.IsSuccessful)
                    return RedirectToAction(nameof(Login));
            }
            Message = _userService.Message;
            SetViewData(_userService.Lang, _userService.ViewModelName);
            return View("_EZRegister", user);
        }
    }
}
