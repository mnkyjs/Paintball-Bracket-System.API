using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using il_y.BracketSystem.Core.Data;
using il_y.BracketSystem.Core.Models.Dtos;
using il_y.BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace il_y.BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FieldController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private User _user;

        public FieldController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Methods

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create(FieldDto fieldDto)
        {
            // validate request
            fieldDto.Name = fieldDto.Name.ToLower();
            var field = await _unitOfWork.Fields.FindByConditionSingle(x => x.Name == fieldDto.Name);
            if (field != null) return BadRequest($"{fieldDto.Name} existiert bereits!");

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

            var fieldToCreate = new Paintballfield
            {
                CreatorId = _user.Id
            };
            fieldDto.UpdateEntity(fieldToCreate);

            _unitOfWork.Fields.Add(fieldToCreate);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetSingleRecord), new {fieldToCreate.Id}, fieldDto);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            var dbModel =
                await _unitOfWork.Fields.FindByConditionList(include: source => source.Include(x => x.Matches));
            var fields = new List<Paintballfield>();

            var listFieldDto = dbModel.Select(field => new FieldDto(field)).ToList();
            return Ok(listFieldDto);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleRecord(int id)
        {
            var fieldDto =
                new FieldDto(await _unitOfWork.Fields.FindByConditionSingle(
                    include: source => source.Include(x => x.Matches),
                    filter: x => x.Id == id));
            if (fieldDto != null)
                return Ok(fieldDto);

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("getFieldWithMatches")]
        public async Task<IActionResult> GetAllMatchesByField()
        {
            try
            {
                var fields = await _unitOfWork.Fields.GetFieldsWithMatches();
                var listFieldDto = fields.Select(field => new FieldDto(field)).ToList();
                return Ok(listFieldDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [Authorize(Policy = "Root")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, FieldDto fieldDto)
        {
            fieldDto.Name = fieldDto.Name.ToLower();
            var field = await _unitOfWork.Fields.GetById(id);

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            // if (field.CreatorId != _user.Id && !_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");

            //_unitOfWork.Fields.LoadRelatedEntities(field);
            var fieldToCheck = await _unitOfWork.Fields.FindByConditionSingle(x => x.Name == fieldDto.Name);
            if (fieldToCheck != null && fieldToCheck.Id != field.Id)
                return BadRequest($"{fieldDto.Name} existiert bereits!");

            fieldDto.UpdateEntity(field);

            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetSingleRecord), new {field.Id}, fieldDto);
        }

        [Authorize(Policy = "Root")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var field = await _unitOfWork.Fields.FindByConditionSingle(x => x.Id == id);
            if (field == null) return BadRequest();

            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            // if (field.CreatorId != _user.Id && !_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");

            await _unitOfWork.Fields.DeleteObjectById(id);
            await _unitOfWork.CompleteAsync();

            return Ok(200);
        }

        #endregion Methods
    }
}