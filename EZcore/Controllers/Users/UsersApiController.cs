#nullable disable

using EZcore.DAL.Users;
using EZcore.Models.Users;
using EZcore.Services;
using EZcore.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers.Users
{
    [ApiController, Route("[controller]")]
    public class UsersApiController : ControllerBase
    {
        private readonly Service<User, UserModel> _userService;

        public UsersApiController(Service<User, UserModel> userService)
        {
            _userService = userService;
            _userService.Api = true;
        }

        [HttpPost("[action]")]
        public IActionResult Token(UserModel user)
        {
            ModelState.Remove("Record.Roles");
            if (ModelState.IsValid)
            {
                var userService = _userService as UserService;
                var token = userService.GetJwt(user);
                if (userService.IsSuccessful)
                    return Ok(token);
                ModelState.AddModelError("Error", userService.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
