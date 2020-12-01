using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace il_y.BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RoleController : Controller
    {
        private readonly RoleManager<Core.Models.Entities.Role> _roleManager;

        public RoleController(RoleManager<Core.Models.Entities.Role> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET
        [HttpGet]
        // [Authorize(Policy = "Root")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleNames = roles.Select(role => role.Name).ToList();

            return Ok(roleNames);
        }
    }
}