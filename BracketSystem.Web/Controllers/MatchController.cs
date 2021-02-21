using BracketSystem.Core.Data;
using BracketSystem.Core.Helpers;
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class MatchController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private User _user;

        public MatchController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost(Name = "CreateSchedule")]
        public async Task<ActionResult<List<TeamDto[]>>> CreateSchedule(CreateScheduleDto createScheduleDto)
        {
            if (User.Identity.Name != null)
            {
                var currentUserId =
                    User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _user = await _userManager.FindByIdAsync(currentUserId).ConfigureAwait(false);
            }
            else
            {
                _user = await _userManager.FindByNameAsync("DummyUser").ConfigureAwait(false);
            }

            var teamDtos = createScheduleDto.Teams.Select(TeamDto.FromEntity).ToList();

            var parsedDate = DateTime.Parse(createScheduleDto.Date, CultureInfo.InvariantCulture);
            var matches = await _unitOfWork.Matches.CreateSchedule(teamDtos, _user, parsedDate,
                    createScheduleDto.PaintballfieldId, createScheduleDto.Name,
                    createScheduleDto.AddClashToAnExistingOne)
                .ConfigureAwait(false);

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(matches);
        }

        [HttpDelete(Name = "DeleteAllMatch")]
        public async Task<IActionResult> DeleteAllMatch()
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
            var matchesToDelete = await _unitOfWork.Matches.DeleteMatches(_user).ConfigureAwait(false);
            await _unitOfWork.Matches.RemoveRange(matchesToDelete).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return Ok(200);
        }

        [HttpDelete("{time}/{name}", Name = "DeleteMatch")]
        public async Task<IActionResult> DeleteMatch(DateTime time, string name)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
            var matchestoDelete = await _unitOfWork.Matches.GetMatchesByDateAndUserToDelete(time, _user, name)
                .ConfigureAwait(false);

            await _unitOfWork.Matches.RemoveRange(matchestoDelete).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return Ok(200);
        }

        [AllowAnonymous]
        [HttpGet("GetByField", Name = "GetByField")]
        public async Task<ActionResult<List<BlockDto>>> GetAllMatchesByField(int paintballfield)
        {
            try
            {
                var matches = await _unitOfWork.Matches.GetMatchesByField(paintballfield).ConfigureAwait(true);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{guid}", Name = "GetMatchesByGuid")]
        public async Task<ActionResult<IEnumerable<BlockDto>>> GetMatchesByGuid(string guid)
        {
            IEnumerable<BlockDto> matches = await _unitOfWork.Matches.GetMatchesByGuid(guid).ConfigureAwait(true);
            return Ok(matches);
        }
    }
}