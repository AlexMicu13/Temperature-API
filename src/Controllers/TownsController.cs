using Backend.Dtos;
using Backend.Dtos.Towns;
using Backend.Enums;
using Backend.Mappers;
using Backend.Models.Entities;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class TownsController(ITownsRepository townsRepository) : ControllerBase
    {
        private readonly ITownsRepository _townsRepository = townsRepository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TownDto>>> GetAllTowns()
        {
            List<Town> list = (await _townsRepository.GetAllTownsAsync()).ToList();
            List<TownDto> listDto = [];
            
            foreach (Town town in list)
            {
                TownDto townDto = TownsMapper.ToDto(town, out _);
                listDto.Add(townDto);
            }

            return Ok(listDto);
        }

        [HttpGet("country/{id_Tara}")]
        public async Task<ActionResult<IEnumerable<TownDto>>> GetAllTownsFromCountry([FromRoute] string id_Tara)
        {
            ObjectId objectIdTara;
            try
            {
                objectIdTara = ObjectId.Parse(id_Tara);
            } 
            catch (Exception)
            {
                return BadRequest("Invalid country ID!");
            }

            List<Town> list = (await _townsRepository.GetAllTownsFromCountryAsync(objectIdTara)).ToList();
            List<TownDto> listDto = [];
            
            foreach (Town town in list)
            {
                TownDto townDto = TownsMapper.ToDto(town, out _);
                listDto.Add(townDto);
            }

            return Ok(listDto);
        }

        [HttpPost]
        public async Task<ActionResult<IdDto>> CreateTown([FromBody] CreateTownDto townDto)
        {
            if (townDto == null)
            {
                return BadRequest("No data introduced!");
            }
   
            Town town = TownsMapper.ToEntity(townDto, out _);
            ReturnCreateTownDto status = await _townsRepository.CreateTownAsync(town);

            if (status.Error < 0)
            {
                ErrorTownCodes error = status.Error;
                if (error == ErrorTownCodes.TownNotCreated)
                {
                    return BadRequest("Could not create the entry in the database!");
                }

                if (error == ErrorTownCodes.CountryNotFound)
                {
                    return NotFound("Country not found!");
                }
                if (error == ErrorTownCodes.DuplicateTown)
                {
                    return Conflict("There is already a town with that name!");
                }
            }

            return new ObjectResult(new IdDto { Id = status.Id.ToString() }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTown([FromRoute] string id, [FromBody] TownDto townDto)
        {
            if (townDto == null)
            {
                return BadRequest("No data introduced!");
            }

            ObjectId objectId;

            try
            {
                objectId = ObjectId.Parse(id);
            }
            catch (Exception)
            {
                return NotFound("Invalid country ID format!");
            }

            Town town = TownsMapper.ToEntity(townDto, out _);

            if (objectId != town.Id)
            {
                return BadRequest("Town ID mismatch");
            }

            var townToUpdate = await _townsRepository.GetTownAsync(objectId);

            if (townToUpdate == null)
            {
                return NotFound($"Town with Id = {id} not found");
            }

            ErrorTownCodes updated = await _townsRepository.UpdateTownAsync(town);

            if (updated == ErrorTownCodes.CountryNotFound)
            {
                return NotFound("Country not found!");
            }

            if (updated == ErrorTownCodes.TownNotUpdated)
            {
                return BadRequest("Could not update the entry in the database!");
            }

            if (updated == ErrorTownCodes.DuplicateTown)
            {
                return Conflict("There is already a town with that name!");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTown([FromRoute] string id)
        {
            ObjectId objectId;

            try
            {
                objectId = ObjectId.Parse(id);
            }
            catch (Exception)
            {
                return NotFound("Invalid country ID format!");
            }

            var town = await _townsRepository.GetTownAsync(objectId);

            if (town == null)
            {
                return NotFound($"Town with Id = {id} not found");
            }

            bool status = await _townsRepository.DeleteTownAsync(objectId);

            if (status == false)
            {
                return BadRequest("Could not delete the entry from the database!");
            }

            return Ok();
        }
    }
}
