#nullable disable

using EZcore.DAL;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
    [Route("[controller]")]
    public class UsersApiController : ApiController
    {
        private readonly Service<User, UserModel> _userService;

        public UsersApiController(Service<User, UserModel> userService)
        {
            _userService = userService;
            _userService.Lang = Lang;
        }

        [HttpPost("[action]")]
        public IActionResult Token(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var userService = _userService as UserService;
                var token = userService.ReadJwt(user);
                if (userService.IsSuccessful)
                    return Ok(token);
                ModelState.AddModelError("Error", userService.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
