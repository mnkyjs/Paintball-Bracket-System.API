using BracketSystem.Core.Data;
using BracketSystem.Core.Models.Dtos;
using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BracketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private User _user;

        public LocationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Methods

        [HttpPost("create")]
        public async Task<ActionResult<Location>> Create(Location location)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            // validate request
            location.Name = location.Name.ToLower();
            var checkLocation = _unitOfWork.Locations.FindByConditionSingle(x => x.Name == location.Name);
            if (checkLocation != null) return BadRequest($"{location.Name} already exists!");

            var locationToCreate = new Location();

            _unitOfWork.Locations.Add(locationToCreate);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetSingleRecord), new {locationToCreate.Id}, location);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Location location)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            // if (_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");
            var tempLocation = await _unitOfWork.Locations.FindByConditionSingle(x => x.Id == location.Id);
            if (tempLocation == null) return BadRequest();

            await _unitOfWork.Locations.DeleteObjectById(tempLocation.Id);
            await _unitOfWork.CompleteAsync();

            return Ok(200);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllRecords()
        {
            var dbModel = await _unitOfWork.Locations.FindByConditionList(include: source => source.Include(x => x.Paintballfields));
            var listLocationDto = dbModel.Select(LocationDto.FromEntity);
            return Ok(listLocationDto);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetSingleRecord(int id)
        {
            var location = await _unitOfWork.Locations.GetById(id).ConfigureAwait(true);
            if (location == null)
                return BadRequest();

            return Ok(location);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Location>> PutAsync(int id, Location location)
        {
            var currentUserId =
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _user = await _unitOfWork.Users.GetById(currentUserId);

            // if (_user.IsAdmin()) return BadRequest("Du hast keine Berechtigung!");
            location.Name = location.Name.ToLower();
            var tempLocation = await _unitOfWork.Locations.GetById(id);
            var locationToCheck = await _unitOfWork.Locations.FindByConditionSingle(x => x.Name == location.Name);
            if (locationToCheck != null) return BadRequest($"{location.Name} already exists!");

            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetSingleRecord), new {location.Id}, location);
        }

        #endregion Methods
    }
}