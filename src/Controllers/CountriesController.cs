using Backend.Dtos;
using Backend.Dtos.Countries;
using Backend.Enums;
using Backend.Mappers;
using Backend.Models.Entities;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController(ICountriesRepository countriesRepository) : ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository = countriesRepository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
        {
            List<Country> list = (await _countriesRepository.GetAllCountriesAsync()).ToList();
            List<CountryDto> listDto = [];
            
            foreach (Country country in list)
            {
                CountryDto countryDto = CountriesMapper.ToDto(country, out _);
                listDto.Add(countryDto);
            }

            return Ok(listDto);
        }

        [HttpPost]
        public async Task<ActionResult<IdDto>> CreateCountry([FromBody] CreateCountryDto countryDto)
        {
            if (countryDto == null)
            {
                return BadRequest("No data introduced!");
            }

            Country country = CountriesMapper.ToEntity(countryDto, out _);
            ReturnCreateCountryDto status = await _countriesRepository.CreateCountryAsync(country);

            if (status.Error < 0)
            {
                ErrorCountryCodes error = status.Error;
                if (error == ErrorCountryCodes.CountryNotCreated)
                {
                    return BadRequest("Could not create the entry in the database!");
                }

                if (error == ErrorCountryCodes.DuplicateCountry)
                {
                    return Conflict("There is already a country with that name!");
                }
            }

            return new ObjectResult(new IdDto { Id = status.Id.ToString()} ) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry([FromRoute] string id, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null)
            {
                return BadRequest("No data introduced!");
            }

            if (id != countryDto.Id)
            {
                return BadRequest("Country ID mismatch");
            }

            ObjectId objectId;

            try
            {
                objectId = ObjectId.Parse(id);
            }
            catch (Exception)
            {
                return BadRequest("Invalid country ID format!");
            }

            Country countryToUpdate;

            try
            {
                countryToUpdate = await _countriesRepository.GetCountryAsync(objectId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound($"Country with Id = {id} not found");
            }

            if (countryToUpdate == null)
            {
                return NotFound($"Country with Id = {id} not found");
            }

            Country country = CountriesMapper.ToEntity(countryDto, out _);

            ErrorCountryCodes updated = await _countriesRepository.UpdateCountryAsync(country);

            if (updated == ErrorCountryCodes.CountryNotUpdated)
            {
                return BadRequest("Could not update the entry in the database!");
            }

            if (updated == ErrorCountryCodes.DuplicateCountry)
            {
                return Conflict("There is already a country with that name!");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry([FromRoute] string id)
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

            var countryToDelete = await _countriesRepository.GetCountryAsync(objectId);

            if (countryToDelete == null)
            {
                return NotFound($"Country with Id = {id} not found");
            }

            bool deleted = await _countriesRepository.DeleteCountryAsync(objectId);

            if (deleted == false)
            {
                return BadRequest("Could not delete the entry from the database!");
            }

            return Ok();
        }
    }
}
