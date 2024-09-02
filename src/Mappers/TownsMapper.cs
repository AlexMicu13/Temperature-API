using Backend.Dtos.Towns;
using Backend.Models.Entities;
using MongoDB.Bson;

namespace Backend.Mappers
{
    public static class TownsMapper
    {
        public static TownDto ToDto(Town town, out TownDto townDto)
        {
            townDto = new TownDto
            {
                Id = town.Id.ToString(),
                IdTara = town.Id_tara.ToString(),
                Nume = town.Nume_oras,
                Lat = town.Latitudine,
                Lon = town.Longitudine
            };

            return townDto;
        }

        public static Town ToEntity(CreateTownDto createTownDto, out Town town)
        {
            town = new Town
            {
                Id_tara = ObjectId.Parse(createTownDto.IdTara),
                Nume_oras = createTownDto.Nume,
                Latitudine = createTownDto.Lat,
                Longitudine = createTownDto.Lon
            };

            return town;
        }

        public static Town ToEntity(TownDto townDto, out Town town)
        {
            town = new Town
            {
                Id = ObjectId.Parse(townDto.Id),
                Id_tara = ObjectId.Parse(townDto.IdTara),
                Nume_oras = townDto.Nume,
                Latitudine = townDto.Lat,
                Longitudine = townDto.Lon
            };

            return town;
        }
    }
}
