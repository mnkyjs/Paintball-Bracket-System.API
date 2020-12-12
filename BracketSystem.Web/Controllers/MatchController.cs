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
        private string _randomUrl;
        private User _user;

        public MatchController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost(Name =  "createSchedule")]
        public async Task<ActionResult<List<TeamDto[]>>> CreateSchedule(CreateScheduleDto createScheduleDto)
        {
            var teamDtos = new List<TeamDto>();

            if (User.Identity.Name != null)
            {
                string currentUserId =
                    User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _user = await _userManager.FindByIdAsync(currentUserId).ConfigureAwait(false);
            }
            else
            {
                _user = await _userManager.FindByNameAsync("DummyUser").ConfigureAwait(false);
            }

            foreach (var team in createScheduleDto.Teams) teamDtos.Add(new TeamDto(team));
            if (createScheduleDto.AddClashToAnExistingOne) _randomUrl = RandomUrl.GetUrl();

            var parsedDate = DateTime.Parse(createScheduleDto.Date, CultureInfo.InvariantCulture);
            var matches = await _unitOfWork.Matches.CreateSchedule(teamDtos, _user, _randomUrl, parsedDate,
                createScheduleDto.PaintballfieldId, createScheduleDto.Name, createScheduleDto.AddClashToAnExistingOne).ConfigureAwait(false);

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(matches);
        }

        [HttpDelete(Name =  "deleteAllMatch")]
        public async Task<IActionResult> DeleteAllMatch()
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
            var matchestoDelete = await _unitOfWork.Matches.DeleteMatches(_user).ConfigureAwait(false);
            await _unitOfWork.Matches.RemoveRange(matchestoDelete).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return Ok(200);
        }

        [HttpDelete("{time}/{name}", Name = "deleteMatch")]
        public async Task<IActionResult> DeleteMatch(DateTime time, string name)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
            var matchestoDelete = await _unitOfWork.Matches.GetMatchesByDateAndUserToDelete(time, _user, name).ConfigureAwait(false);

            await _unitOfWork.Matches.RemoveRange(matchestoDelete).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return Ok(200);
        }

        [HttpGet(Name =  "getMatches")]
        public async Task<ActionResult<List<BlockDto>>> GetAllMatches()
        {
            try
            {
                var currentUserId =
                    Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
                _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);
                var matches = await _unitOfWork.Matches.GetMatches(_user).ConfigureAwait(true);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "getByField")]
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
        [HttpGet("{time}/{name}", Name = "GetMatchesByDate")]
        public async Task<ActionResult<List<BlockDto>>> GetMatchesByDate(DateTime time, string name)
        {
            var matches = await _unitOfWork.Matches.GetMatchesByDate(time, name).ConfigureAwait(true);
            return Ok(matches);
        }
    }
}