using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleController(RoleManager<Core.Models.Entities.Role> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET
        [HttpGet]
        // [Authorize(Policy = "Root")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleNames = roles.ConvertAll(role => role.Name);

            return Ok(roleNames);
        }
    }
}