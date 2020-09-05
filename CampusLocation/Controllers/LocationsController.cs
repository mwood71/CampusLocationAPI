using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CampusLocation.Contracts;
using CampusLocation.Data;
using CampusLocation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusLocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepo;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public LocationsController(ILocationRepository locationRepo,
            ILoggerService logger,
            IMapper mapper)
        {
            _locationRepo = locationRepo;
            _logger = logger;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all locations
        /// </summary>
        /// <returns>List of locations</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Request GetAll sent");

                var locations = await _locationRepo.GetAll();
                var response = _mapper.Map<IList<LocationDTO>>(locations);

                _logger.LogInfo($"{location}: call was successful");

                return Ok(response);


            }
            catch (Exception e)
            {

                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Get location by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Location</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: location with Id:{id} requested");

                var locationObj = await _locationRepo.FindById(id);

                if (locationObj == null)
                {
                    _logger.LogWarn($"{location}: location with Id:{id} is null");
                    _logger.LogInfo($"{location}: Successfully got location with Id:{id}");
                    return NotFound();
                }

                var response = _mapper.Map<LocationDTO>(locationObj);
                return Ok(response);

            }
            catch (Exception e)
            {

                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Post a Location
        /// </summary>
        /// <returns>Adding a location</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody]LocationDTO locationDTO)
        {
            var location = GetControllerActionNames();

            try
            {

                _logger.LogInfo($"{location}: Create location requested");

                if (locationDTO == null)
                {
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var locationObj = _mapper.Map<Location>(locationDTO);
                var isSuccess = await _locationRepo.Create(locationObj);

                if (!isSuccess)
                {
                    return InternalError($"{location}: Creation failed");
                }

                return Created("Create", new { locationObj });

            }
            catch (Exception e)
            {

                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Update location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody]LocationUpdateDTO locationDTO)
        {
            var location = GetControllerActionNames();

            try
            {

                if (locationDTO == null || id < 1 || id != locationDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update request sent for location with Id:{id}");
                    return BadRequest(ModelState);
                }

                var doesExist = await _locationRepo.doesExist(id);

                if (!doesExist)
                {
                    _logger.LogWarn($"{location}: location with Id:{id} not found");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: update for Id:{id} is invalid");
                    return BadRequest(ModelState);
                }

                var locationObj = _mapper.Map<Location>(locationDTO);
                var success = await _locationRepo.Update(locationObj);

                if (!success)
                {
                    return InternalError($"{location}: Update failed for location with id: {id}");
                }

                _logger.LogInfo($"{location}: Successful update request for Id:{id}");
                return NoContent();


            }
            catch (Exception e)
            {

                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }
        /// <summary>
        /// Delete location
        /// </summary>
        /// <param name="id"></param>
        /// <returns>location deleted</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Delete request sent with Id:{id}");

                if (id < 1)
                {
                    _logger.LogWarn($"{location}: Invalid Id:{id} passed");
                    return BadRequest();

                }

                var doesExist = await _locationRepo.doesExist(id);

                if (!doesExist)
                {
                    _logger.LogWarn($"{location}: location with Id:{id} doesn't exist");
                    return NotFound();
                }

                var locationObj = await _locationRepo.FindById(id);
                var success = await _locationRepo.Delete(locationObj);

                if (!success)
                {
                    return InternalError($"{location}: Delete failed for location with id: {id}");
                }

                _logger.LogInfo($"{location}: location with Id:{id} successfully deleted");
                return NoContent();

            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");


            }
        }
       

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }
    }
}
