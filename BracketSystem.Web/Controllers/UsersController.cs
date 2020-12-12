using BracketSystem.Core.Data;
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private User _user;

        public UsersController(IUnitOfWork unitOfWork,
            ILogger<UsersController> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            //InitUser();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Root")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetById(id).ConfigureAwait(false);

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

            await _unitOfWork.Users.DeleteObjectById(user.Id).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(200);
        }

        [Authorize(Policy = "Root")]
        [HttpPost("{userName}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

            var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles ??= Array.Empty<string>();

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles, StringComparer.Ordinal)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles, StringComparer.Ordinal)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove to roles");
            }

            return Ok(await _userManager.GetRolesAsync(user).ConfigureAwait(false));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<KeyPairValueDto>>> GetSharedMatches()
        {
            if (User.Identity.Name == null)
            {
                return NotFound("Benutzer konnte nicht gefunden werden.");
            }

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
            var matches = await _unitOfWork.Matches.GetMatchesByUser(_user.Id).ConfigureAwait(false);
            var listOfMatches = matches.ConvertAll(match =>
            {
                if (match.Date != null)
                {
                    return new KeyPairValueDto
                    { Name = match.MatchName, Date = (DateTime)match.Date };
                }

                return null;
            });

            // TODO refactor this section (service, dto etc.)
            var listToView = new List<KeyPairValueDto>();

            foreach (var item in listOfMatches)
            {
                var keyPair = new KeyPairValueDto
                {
                    Name = item.Name,
                    Date = item.Date,
                };

                var containsItem = listToView.Any(n => string.Equals(n.Name, keyPair.Name, StringComparison.Ordinal));

                if (!containsItem) listToView.Add(keyPair);
            }

            return Ok(listToView);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetById(id).ConfigureAwait(false);
                var userForListDto = UserForListDto.FromEntity(user);

                var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
                _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

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

        [HttpGet]
        [Authorize(Policy = "Root")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            // only allow admins to access other user records

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

            IEnumerable<object> users = await _unitOfWork.Users.UserListWithRoles().ConfigureAwait(false);

            return Ok(users);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, User userDto)
        {
            userDto.UserName = userDto.UserName.ToLower(CultureInfo.InvariantCulture);
            var user = await _unitOfWork.Users.GetById(id).ConfigureAwait(false);

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

            var userToCheck = await _unitOfWork.Users.FindByConditionSingle(x => x.UserName == userDto.UserName).ConfigureAwait(false);
            if (userToCheck != null && userToCheck.Id != user.Id)
                return BadRequest($"{userDto.UserName} existiert bereits!");

            user.UserName = userDto.UserName;
            user.TeamName = userDto.TeamName;

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(200);
        }
    }
}