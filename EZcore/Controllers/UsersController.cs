#nullable disable

using EZcore.DAL;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
	public class UsersController : MvcController
	{
		protected override string ViewModelName => Lang == Lang.TR ? "Kullanıcı" : "User";

		private readonly Service<User, UserModel> _userService;

		public UsersController(HttpServiceBase httpService, Service<User, UserModel> userService) : base(httpService)
		{
			_userService = userService;
			_userService.Lang = Lang;
		}

		public IActionResult Login()
		{
			SetViewData();
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
			Message = userService.Message;
			SetViewData();
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
			SetViewData();
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
            Message = userService.Message;
            SetViewData();
            return View("_EZRegister", user);
        }
    }
}
