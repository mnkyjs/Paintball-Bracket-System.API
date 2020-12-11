using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BracketSystem.Web.Controllers
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