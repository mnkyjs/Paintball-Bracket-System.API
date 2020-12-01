using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Data;
using il_y.BracketSystem.Core.Helpers;
using il_y.BracketSystem.Core.Models.Dtos;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace il_y.BracketSystem.Web.Controllers
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
        [HttpPost("createSchedule")]
        public async Task<List<TeamDto[]>> CreateSchedule(CreateScheduleDto createScheduleDto)
        {
            var teamDtos = new List<TeamDto>();

            if (User.Identity.Name != null)
            {
                string currentUserId =
                    User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _user = await _userManager.FindByIdAsync(currentUserId);
            }
            else
            {
                _user = await _userManager.FindByNameAsync("DummyUser");
            }

            foreach (var team in createScheduleDto.Teams) teamDtos.Add(new TeamDto(team));
            if (createScheduleDto.AddClashToAnExistingOne) _randomUrl = RandomUrl.GetUrl();

            var parsedDate = DateTime.Parse(createScheduleDto.Date);
            var matches = await _unitOfWork.Matches.CreateSchedule(teamDtos, _user, _randomUrl, parsedDate,
                createScheduleDto.PaintballfieldId, createScheduleDto.Name, createScheduleDto.AddClashToAnExistingOne);

            await _unitOfWork.CompleteAsync();

            return matches;
        }

        [HttpDelete("deleteAllMatch")]
        public async Task<IActionResult> DeleteAllMatch()
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);
            var matchestoDelete = await _unitOfWork.Matches.DeleteMatches(_user);
            await _unitOfWork.Matches.RemoveRange(matchestoDelete);
            await _unitOfWork.CompleteAsync();
            return Ok(200);
        }

        [HttpDelete("{time}/{name}")]
        public async Task<IActionResult> DeleteMatch(DateTime time, string name)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);
            var matchestoDelete = await _unitOfWork.Matches.GetMatchesByDateAndUserToDelete(time, _user, name);

            await _unitOfWork.Matches.RemoveRange(matchestoDelete);
            await _unitOfWork.CompleteAsync();
            return Ok(200);
        }

        [HttpGet("getMatches")]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                var currentUserId =
                    Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _user = await _unitOfWork.Users.GetById(currentUserId);
                var matches = await _unitOfWork.Matches.GetMatches(_user);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("getByField")]
        public async Task<IActionResult> GetAllMatchesByField(int paintballfield)
        {
            try
            {
                var matches = await _unitOfWork.Matches.GetMatchesByField(paintballfield);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{time}/{name}")]
        public async Task<IActionResult> GetMatchesByDate(DateTime time, string name)
        {
            var matches = await _unitOfWork.Matches.GetMatchesByDate(time, name);
            return Ok(matches);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMatchesByDateAndUser(DateTime time, string name)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);
            var matches = await _unitOfWork.Matches.GetMatchesByDateAndUser(time, _user, name);
            return Ok(matches);
        }
    }
}