using BracketSystem.Core.Data;
using BracketSystem.Core.Models;
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        #region Constructors

        public TeamsController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #endregion Constructors

        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private User _user;

        #endregion Fields

        #region Methods

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<ActionResult<TeamDto>> Create(TeamDto teamDto)
        {
            // validate request

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

            teamDto.Name = teamDto.Name.ToLower(CultureInfo.InvariantCulture);
            var team = await _unitOfWork.Teams.FindByConditionSingle(x => x.Name == teamDto.Name).ConfigureAwait(false);
            if (team != null) return BadRequest($"{teamDto.Name} existiert bereits!");

            var teamToCreate = new Team();
            teamDto.UpdateEntity(teamToCreate);
            teamToCreate.CreatorId = _user.Id;

            _unitOfWork.Teams.Add(teamToCreate);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return CreatedAtAction(nameof(GetSingleRecord), new {teamToCreate.Id}, teamDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var team = await _unitOfWork.Teams.FindByConditionSingle(x => x.Id == id).ConfigureAwait(false);

            if (team == null)
                return BadRequest("Kein Team gefunden");

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

            // if (team.CreatorId != _user.Id && !_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");
            await _unitOfWork.Teams.DeleteObjectById(id).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(200);
        }

        [AllowAnonymous]
        [HttpGet("GetTeams")]
        public async Task<ActionResult<PagedResult<Team>>> FindTeams(int page = 1, int pageSize = 10,
            string filter = null, string sortColumn = "Name", string sortOrder = "asc")
        {
            var teams = await _unitOfWork.Teams.FindTeams(page, pageSize, filter, sortColumn, sortOrder).ConfigureAwait(false);

            return teams;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IOrderedEnumerable<TeamDto>>> GetAllRecords()
        {
            var teams = await _unitOfWork.Teams.GetAllRecordsFromDatabase().ConfigureAwait(false);
            var tempTeams = await _unitOfWork.Teams.FindByConditionList(filter: x => x.Name != "pause").ConfigureAwait(false);
            var teamDtoList = teams.ConvertAll(TeamDto.FromEntity).OrderBy(tn => tn.Name);
            return Ok(teamDtoList);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetSingleRecord(int id)
        {
            var teamDto = new TeamDto(await _unitOfWork.Teams.FindByConditionSingle(x => x.Id == id).ConfigureAwait(false));

            return Ok(teamDto);
        }

        [Authorize(Policy = "Root")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDto>> PutAsync(int id, TeamDto teamDto)
        {
            teamDto.Name = teamDto.Name.ToLower(CultureInfo.InvariantCulture);
            var team = await _unitOfWork.Teams.GetById(id).ConfigureAwait(false);
            var teamNameCheck = await _unitOfWork.Teams.FindByConditionSingle(x => x.Name == teamDto.Name).ConfigureAwait(false);
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
            _user = await _unitOfWork.Users.GetById(currentUserId).ConfigureAwait(false);

            // if (team.CreatorId != _user.Id && !_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");

            if (teamNameCheck != null) return BadRequest($"{teamDto.Name} existiert bereits!");

            teamDto.UpdateEntity(team);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return CreatedAtAction(nameof(GetSingleRecord), new {team.Id}, teamDto);
        }
        #endregion Methods
    }
}