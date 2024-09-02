using Backend.Dtos.Countries;
using Backend.Enums;
using Backend.Models;
using Backend.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Repositories
{
    public interface ICountriesRepository
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<Country> GetCountryAsync(ObjectId id);
        Task<ReturnCreateCountryDto> CreateCountryAsync(Country country);
        Task<ErrorCountryCodes> UpdateCountryAsync(Country country);
        Task<bool> DeleteCountryAsync(ObjectId id);
    }

    public class CountriesRepository(IBackendContext context) : ICountriesRepository
    {
        private readonly IBackendContext _context = context;

        public async Task<ReturnCreateCountryDto> CreateCountryAsync(Country country)
        {
            ReturnCreateCountryDto returnCreateCountryDto = new()
            {
                Error = ErrorCountryCodes.Success
            };
            try
            {
                var add = country.ToBsonDocument();
                await _context.Countries.InsertOneAsync(country);
                returnCreateCountryDto.Id = country.Id;
                return returnCreateCountryDto;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                returnCreateCountryDto.Error = ErrorCountryCodes.DuplicateCountry;
                return returnCreateCountryDto;
            }
            catch (Exception)
            {
                returnCreateCountryDto.Error = ErrorCountryCodes.CountryNotCreated;
                return returnCreateCountryDto;
            }
        }

        public async Task<bool> DeleteCountryAsync(ObjectId id)
        {
            List<ObjectId> towns = _context
                .Towns
                .AsQueryable()
                .Where(t => t.Id_tara == id)
                .Select(t => t.Id)
                .ToList();

            FilterDefinition<Temperature> filterTemperature = Builders<Temperature>.Filter.In(m => m.Id_oras, towns);
            DeleteResult deleteResultTemperature = await _context
                .Temperatures
                .DeleteManyAsync(filterTemperature);

            FilterDefinition<Town> filterTown = Builders<Town>.Filter.Eq(m => m.Id_tara, id);
            DeleteResult deleteResultTown = await _context
                .Towns
                .DeleteManyAsync(filterTown);

            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context
                .Countries
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged 
                && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            return await _context
                .Countries
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<Country> GetCountryAsync(ObjectId id)
        {
            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(m => m.Id, id);
            return await _context
                    .Countries
                    .Find(filter)
                    .FirstOrDefaultAsync();
        }

        public async Task<ErrorCountryCodes> UpdateCountryAsync(Country country)
        {
            try
            {
                ReplaceOneResult updateResult =
                    await _context
                    .Countries
                    .ReplaceOneAsync(
                    filter: g => g.Id == country.Id,
                    replacement: country);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    return ErrorCountryCodes.Success;
                }
                return ErrorCountryCodes.CountryNotUpdated;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ErrorCountryCodes.DuplicateCountry;
            }
            catch (Exception)
            {
                return ErrorCountryCodes.CountryNotUpdated;
            }
        }
    }
}
