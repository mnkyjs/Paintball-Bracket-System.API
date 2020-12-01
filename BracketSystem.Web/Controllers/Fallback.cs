using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace il_y.BracketSystem.Web.Controllers
{
    [AllowAnonymous]
    public class Fallback : Controller
    {
        // GET
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Path.Combine(Directory.GetCurrentDirectory()), "wwwroot", "index.html"),
                "text/HTML");
        }
    }
}