using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Data;
using il_y.BracketSystem.Core.Models;
using il_y.BracketSystem.Core.Models.Dtos;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace il_y.BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private User _user;

        public UsersController(IUnitOfWork unitOfWork,
            ILogger<UsersController> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            //InitUser();
        }

        [HttpGet]
        [Authorize(Policy = "Root")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            // only allow admins to access other user records

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            IEnumerable<object> users = await _unitOfWork.Users.UserListWithRoles();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetById(id);
                var userForListDto = UserForListDto.FromEntity(user);

                var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _user = await _unitOfWork.Users.GetById(currentUserId);


                // only owner has access own records
                if (id != _user.Id) return Unauthorized();

                return Ok(userForListDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Es ist ein Fehler aufgetreten");
            }
        }

        [Authorize(Policy = "Root")]
        [HttpPost("{userName}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles ??= new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove to roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, User userDto)
        {
            userDto.UserName = userDto.UserName.ToLower();
            var user = await _unitOfWork.Users.GetById(id);

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            var userToCheck = await _unitOfWork.Users.FindByConditionSingle(x => x.UserName == userDto.UserName);
            if (userToCheck != null && userToCheck.Id != user.Id)
                return BadRequest($"{userDto.UserName} existiert bereits!");

            user.UserName = userDto.UserName;
            user.TeamName = userDto.TeamName;

            await _unitOfWork.CompleteAsync();

            return Ok(200);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Root")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetById(id);

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            await _unitOfWork.Users.DeleteObjectById(user.Id);
            await _unitOfWork.CompleteAsync();

            return Ok(200);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<KeyPairValueDto>>> GetSharedMatches()
        {
            if (User == null)
            {
                return NotFound("Benutzer konnte nicht gefunden werden.");
            }

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);
            var matches = await _unitOfWork.Matches.GetMatchesByUser(_user.Id);
            var listOfMatches = matches.Select(match =>
            {
                if (match.Date != null)
                    return new KeyPairValueDto
                        {Name = match.MatchName, Date = (DateTime) match.Date};
                return null;
            }).ToList();

            // TODO refactor this section (service, dto etc.)
            var listToView = new List<KeyPairValueDto>();

            foreach (var item in listOfMatches)
            {
                var keyPair = new KeyPairValueDto
                {
                    Name = item.Name,
                    Date = item.Date
                };

                var containsItem = listToView.Any(n => n.Name == keyPair.Name);

                if (!containsItem) listToView.Add(keyPair);
            }

            return Ok(listToView);
        }
    }
}