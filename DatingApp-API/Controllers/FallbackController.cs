using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp_API.Controllers
{
    public class FallbackController:Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory()
            ,"wwwroot","index.html"),"text/html");
        }
    }
}