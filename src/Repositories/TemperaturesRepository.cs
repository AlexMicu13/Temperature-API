using Backend.Dtos.Temperatures;
using Backend.Enums;
using Backend.Models;
using Backend.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Repositories
{
    public interface ITemperaturesRepository
    {
        Task<Temperature> GetTemperatureByIdAsync(ObjectId id);
        Task<List<Temperature>> GetTemperaturesBetweenDatesAsync(DateTime? startDate, DateTime? endDate);
        Task<List<Temperature>> GetTemperaturesByTownIdAsync(ObjectId id, DateTime? startDate, DateTime? endDate);
        Task<List<Temperature>> GetTemperaturesByCountryIdAsync(ObjectId id, DateTime? startDate, DateTime? endDate);
        Task<List<Temperature>> GetTemperaturesByLocationAsync(double? lat, double? lon, DateTime? startDate, DateTime? endDate);
        Task<ReturnCreateTemperatureDto> CreateTemperatureAsync(Temperature temperature);
        Task<ErrorTemperatureCodes> UpdateTemperatureAsync(Temperature temperature);
        Task<bool> DeleteTemperatureAsync(ObjectId id);
    }
    public class TemperaturesRepository(IBackendContext context) : ITemperaturesRepository
    {
        private readonly IBackendContext _context = context;

        public async Task<ReturnCreateTemperatureDto> CreateTemperatureAsync(Temperature temperature)
        {
            ReturnCreateTemperatureDto returnCreateTownDto = new()
            {
                Error = ErrorTemperatureCodes.Success
            };
            FilterDefinition<Town> filter = Builders<Town>.Filter.Eq(m => m.Id, temperature.Id_oras);
            var town = await _context
                    .Towns
                    .Find(filter)
                    .FirstOrDefaultAsync();

            if (town == null)
            {
                returnCreateTownDto.Error = ErrorTemperatureCodes.TownNotFound;
                return returnCreateTownDto;
            }

            try
            {
                await _context.Temperatures.InsertOneAsync(temperature);
                returnCreateTownDto.Id = temperature.Id;
                return returnCreateTownDto;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                returnCreateTownDto.Error = ErrorTemperatureCodes.DuplicateTemperature;
                return returnCreateTownDto;
            }
            catch (Exception)
            {
                returnCreateTownDto.Error = ErrorTemperatureCodes.TemperatureNotCreated;
                return returnCreateTownDto;
            }
            
        }

        public async Task<bool> DeleteTemperatureAsync(ObjectId id)
        {
            FilterDefinition<Temperature> filter = Builders<Temperature>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context
                .Temperatures
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }

        public async Task<Temperature> GetTemperatureByIdAsync(ObjectId id)
        {
            return await _context.Temperatures.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Temperature>> GetTemperaturesBetweenDatesAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null && endDate == null)
            {
                return await _context.Temperatures.Find(_ => true).ToListAsync();
            }
            if (startDate == null)
            {
                return await _context.Temperatures.Find(t => t.Timestamp <= endDate).ToListAsync();
            }
            if (endDate == null)
            {
                return await _context.Temperatures.Find(t => t.Timestamp >= startDate).ToListAsync();
            }

            return await _context.Temperatures.Find(t => t.Timestamp >= startDate && t.Timestamp <= endDate).ToListAsync();
            
        }

        public async Task<List<Temperature>> GetTemperaturesByCountryIdAsync(ObjectId id, DateTime? startDate, DateTime? endDate)
        {
            List<Temperature> allTemps = await GetTemperaturesBetweenDatesAsync(startDate, endDate);
            List<ObjectId> towns = _context.Towns.AsQueryable().Where(t => t.Id_tara == id).Select(t => t.Id).ToList();
            return allTemps.FindAll(t => towns.Contains(t.Id_oras));
        }

        public async Task<List<Temperature>> GetTemperaturesByLocationAsync(double? lat, double? lon, DateTime? startDate, DateTime? endDate)
        {
            List<Temperature> allTemps = await GetTemperaturesBetweenDatesAsync(startDate, endDate);
            
            if (lat == null && lon == null)
            {
                return allTemps;
            }

            List<ObjectId> towns = [];
            List<ObjectId> countries = [];
            
            if (lat == null && lon != null)
            {
                towns = _context.Towns.AsQueryable().Where(t => t.Longitudine == lon).Select(t => t.Id).ToList();
                countries = _context.Countries.AsQueryable().Where(c => c.Longitudine == lon).Select(c => c.Id).ToList();
            }
            else if (lat != null && lon == null)
            {
                towns = _context.Towns.AsQueryable().Where(t => t.Latitudine == lat).Select(t => t.Id).ToList();
                countries = _context.Countries.AsQueryable().Where(c => c.Latitudine == lat).Select(c => c.Id).ToList();
            }
            else if (lat != null && lon != null)
            {
                towns = _context.Towns.AsQueryable().Where(t => t.Latitudine == lat && t.Longitudine == lon).Select(t => t.Id).ToList();
                countries = _context.Countries.AsQueryable().Where(c => c.Latitudine == lat && c.Longitudine == lon).Select(c => c.Id).ToList();
           
            }

            IEnumerable<ObjectId> extraTowns = countries.SelectMany(c => _context.Towns.AsQueryable().Where(t => t.Id_tara == c).ToList().Select(t => t.Id));
            towns.AddRange(extraTowns);

            return allTemps.FindAll(t => towns.Contains(t.Id_oras));
        }

        public async Task<List<Temperature>> GetTemperaturesByTownIdAsync(ObjectId id, DateTime? startDate, DateTime? endDate)
        {
            List<Temperature> allTemps = await GetTemperaturesBetweenDatesAsync(startDate, endDate);
            return allTemps.FindAll(t => t.Id_oras == id);
        }

        public async Task<ErrorTemperatureCodes> UpdateTemperatureAsync(Temperature temperature)
        {
            FilterDefinition<Town> filter = Builders<Town>.Filter.Eq(m => m.Id, temperature.Id_oras);
            var town = await _context
                    .Towns
                    .Find(filter)
                    .FirstOrDefaultAsync();

            if (town == null)
            {
                return ErrorTemperatureCodes.TownNotFound;
            }

            try
            {
                ReplaceOneResult updateResult =
                    await _context
                    .Temperatures
                    .ReplaceOneAsync(
                filter: g => g.Id == temperature.Id,
                    replacement: temperature);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    return ErrorTemperatureCodes.Success;
                }
                return ErrorTemperatureCodes.TemperatureNotUpdated;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ErrorTemperatureCodes.DuplicateTemperature;
            }
            catch (Exception)
            {
                return ErrorTemperatureCodes.TemperatureNotUpdated;
            }
        }
    }
}
