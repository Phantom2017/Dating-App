using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class BaseApiController:ControllerBase
    {
        
    }
}