using Backend.Dtos;
using Backend.Dtos.Temperatures;
using Backend.Enums;
using Backend.Mappers;
using Backend.Models.Entities;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/temperatures")]
    public class TemperaturesController(ITemperaturesRepository temperaturesRepository) : ControllerBase
    {
        private readonly ITemperaturesRepository _temperaturesRepository = temperaturesRepository;

        [HttpPost]
        public async Task<ActionResult<IdDto>> CreateTemperature([FromBody] CreateTemperatureDto temperatureDto)
        {
            if (temperatureDto == null)
            {
                return BadRequest("No data introduced!");
            }
                
            Temperature temperature = TemperaturesMapper.ToEntity(temperatureDto, out _);
            temperature.Timestamp = DateTime.UtcNow;
            ReturnCreateTemperatureDto status = await _temperaturesRepository.CreateTemperatureAsync(temperature);

            if (status.Error < 0)
            {
                ErrorTemperatureCodes error = status.Error;
                if (error == ErrorTemperatureCodes.TemperatureNotCreated)
                {
                    return BadRequest("Could not create the entry in the database!");
                }

                if (error == ErrorTemperatureCodes.TownNotFound)
                {
                    return NotFound("The town with the specified id was not found!");
                }

                if (error == ErrorTemperatureCodes.DuplicateTemperature)
                {
                    return Conflict("There is already a temperature with that unique key!");
                }
            }

            return new ObjectResult(new IdDto { Id = status.Id.ToString() }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemperature([FromRoute] string id, [FromBody] UpdateTemperatureDto temperatureDto)
        {
            if (temperatureDto == null)
            {
                return BadRequest("No data introduced!");
            }

            if (id != temperatureDto.Id)
            {
                return BadRequest("Temperature ID mismatch");
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

            Temperature temperature = await _temperaturesRepository.GetTemperatureByIdAsync(objectId);

            if (temperature == null)
            {
                return NotFound($"Temperature with Id = {id} not found");
            }

            Temperature newTemperature = TemperaturesMapper.ToEntity(temperatureDto, out _);
            newTemperature.Timestamp = temperature.Timestamp;

            ErrorTemperatureCodes status = await _temperaturesRepository.UpdateTemperatureAsync(newTemperature);

            if (status == ErrorTemperatureCodes.TemperatureNotUpdated)
            {
                return BadRequest("The temperature was not updated found!");
            }

            if (status == ErrorTemperatureCodes.TownNotFound)
            {
                return BadRequest("The town with the specified id was not found!");
            }

            if (status == ErrorTemperatureCodes.DuplicateTemperature)
            {
                return Conflict("There is already a temperature with that unique key!");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemperature([FromRoute] string id)
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

            var temperature = await _temperaturesRepository.GetTemperatureByIdAsync(objectId);
            
            if (temperature == null)
            {
                return NotFound($"Temperature with Id = {id} not found");
            }

            bool deleted = await _temperaturesRepository.DeleteTemperatureAsync(objectId);

            if (!deleted)
            {
                return NotFound($"Temperature with Id = {id} not found");
            }

            return Ok();
        }

        [HttpGet("cities/{idOras}")]
        public async Task<ActionResult<List<ShowTemperatureDto>>> GetTemperaturesFromCity([FromRoute] string idOras, [FromQuery] DateTime? from, [FromQuery] DateTime? until)
        {
            ObjectId objectIdOras;

            try
            {
                objectIdOras = ObjectId.Parse(idOras);
            }
            catch (Exception)
            {
                return NotFound("Invalid country ID format!");
            }

            if (until != null)
            {
                until = until.Value.AddDays(1);
                until = until.Value.AddMicroseconds(-1);
            }

            if (from != null && until != null && from > until)
            {
                return BadRequest("The start date cannot be after the end date!");
            }

            List<Temperature> temperatures = await _temperaturesRepository.GetTemperaturesByTownIdAsync(objectIdOras, from, until);

            List<ShowTemperatureDto> temperaturesDto = [];
            foreach (var temperature in temperatures)
            {
                temperaturesDto.Add(TemperaturesMapper.ToDto(temperature, out _));
            }

            return Ok(temperaturesDto);
        }

        [HttpGet("countries/{idTara}")]
        public async Task<ActionResult<List<ShowTemperatureDto>>> GetTemperaturesFromCountry([FromRoute] string idTara, [FromQuery] DateTime? from, [FromQuery] DateTime? until)
        {
            ObjectId objectIdTara;

            try
            {
                objectIdTara = ObjectId.Parse(idTara);
            }
            catch (Exception)
            {
                return NotFound("Invalid country ID format!");
            }
            if (until != null)
            {
                until = until.Value.AddDays(1);
                until = until.Value.AddMicroseconds(-1);
            }

            if (from != null && until != null && from > until)
            {
                return BadRequest("The start date cannot be after the end date!");
            }
            List<Temperature> temperatures = await _temperaturesRepository.GetTemperaturesByCountryIdAsync(objectIdTara, from, until);

            List<ShowTemperatureDto> temperaturesDto = [];
            foreach (var temperature in temperatures)
            {
                temperaturesDto.Add(TemperaturesMapper.ToDto(temperature, out _));
            }

            return Ok(temperaturesDto);
        }

        [HttpGet()]
        public async Task<ActionResult<List<ShowTemperatureDto>>> GetTemperaturesFromLocation([FromQuery] double? lat, [FromQuery] double? lon, [FromQuery] DateTime? from, [FromQuery] DateTime? until)
        {
            if (until != null)
            {
                until = until.Value.AddDays(1);
                until = until.Value.AddMicroseconds(-1);
            }

            if (from != null && until != null && from > until)
            {
                return BadRequest("The start date cannot be after the end date!");
            }

            List<Temperature> temperatures = await _temperaturesRepository.GetTemperaturesByLocationAsync(lat, lon, from, until);

            List<ShowTemperatureDto> temperaturesDto = [];
            foreach (var temperature in temperatures)
            {
                temperaturesDto.Add(TemperaturesMapper.ToDto(temperature, out _));
            }

            return Ok(temperaturesDto);
        }
    }
}
