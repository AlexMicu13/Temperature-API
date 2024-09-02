using Backend.Dtos.Countries;
using Backend.Dtos.Towns;
using Backend.Enums;
using Backend.Models;
using Backend.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Repositories
{
    public interface ITownsRepository
    {
        Task<IEnumerable<Town>> GetAllTownsAsync();
        Task<IEnumerable<Town>> GetAllTownsFromCountryAsync(ObjectId idTara);
        Task<Town> GetTownAsync(ObjectId id);
        Task<ReturnCreateTownDto> CreateTownAsync(Town town);
        Task<ErrorTownCodes> UpdateTownAsync(Town town);
        Task<bool> DeleteTownAsync(ObjectId id);
    }
    public class TownsRepository(IBackendContext context) : ITownsRepository
    {
        private readonly IBackendContext _context = context;

        public async Task<ReturnCreateTownDto> CreateTownAsync(Town town)
        {
            ReturnCreateTownDto returnCreateTownDto = new()
            {
                Error = ErrorTownCodes.Success
            };
            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(m => m.Id, town.Id_tara);
            var country = await _context
                    .Countries
                    .Find(filter)
                    .FirstOrDefaultAsync();
            
            if (country == null)
            {
                returnCreateTownDto.Error = ErrorTownCodes.CountryNotFound;
                return returnCreateTownDto;
            }

            try
            {
                await _context.Towns.InsertOneAsync(town);
                returnCreateTownDto.Id = town.Id;
                return returnCreateTownDto;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                returnCreateTownDto.Error = ErrorTownCodes.DuplicateTown;
                return returnCreateTownDto;
            } 
            catch (Exception)
            {
                returnCreateTownDto.Error = ErrorTownCodes.TownNotCreated;
                return returnCreateTownDto;
            }
        }

        public async Task<bool> DeleteTownAsync(ObjectId id)
        {
            FilterDefinition<Temperature> filterTemperature = Builders<Temperature>.Filter.Eq(m => m.Id_oras, id);
            DeleteResult deleteResultTemperature = await _context
                .Temperatures
                .DeleteManyAsync(filterTemperature);

            FilterDefinition<Town> filter = Builders<Town>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context
                .Towns
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Town>> GetAllTownsAsync()
        {
            return await _context.Towns.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Town>> GetAllTownsFromCountryAsync(ObjectId idTara)
        {
            return await _context.Towns.Find(t => t.Id_tara == idTara).ToListAsync();
        }

        public async Task<Town> GetTownAsync(ObjectId id)
        {
            return await _context.Towns.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ErrorTownCodes> UpdateTownAsync(Town town)
        {
            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(m => m.Id, town.Id_tara);
            var country = await _context
                    .Countries
                    .Find(filter)
                    .FirstOrDefaultAsync();

            if (country == null)
            {
                return ErrorTownCodes.CountryNotFound;
            }

            try
            {
                ReplaceOneResult updateResult =
                    await _context
                    .Towns
                    .ReplaceOneAsync(
                filter: g => g.Id == town.Id,
                    replacement: town);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    return ErrorTownCodes.Success;
                }
                return ErrorTownCodes.TownNotUpdated;
            }
            catch (MongoWriteException ex)
            when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ErrorTownCodes.DuplicateTown;
            }
            catch (Exception)
            {
                return ErrorTownCodes.TownNotUpdated;
            }
        }
    }
}
