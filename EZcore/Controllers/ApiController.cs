using EZcore.Models;
using Microsoft.AspNetCore.Mvc;

namespace EZcore.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected virtual Lang Lang { get; }

        protected ApiController()
        {
            Lang = Lang.EN;
        }
    }
}
