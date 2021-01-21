using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(IConfiguration config, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _config = config;

            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<AuthModelDto> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username).ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, userForLoginDto.Password, lockoutOnFailure: false)
                .ConfigureAwait(false);

            return !result.Succeeded ? null : new AuthModelDto {Token = GenerateJwtToken(user).Result};
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthModelDto>> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = new User();
            userForRegisterDto.UpdateEntity(userToCreate);
            userToCreate.Created = DateTime.Now;

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password)
                .ConfigureAwait(false);
            await _userManager.AddToRoleAsync(userToCreate, "Member").ConfigureAwait(false);

            return !result.Succeeded ? (ActionResult) BadRequest() : Ok(new AuthModelDto {Token = GenerateJwtToken(userToCreate).Result});
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Team", user.TeamName)
            };

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}