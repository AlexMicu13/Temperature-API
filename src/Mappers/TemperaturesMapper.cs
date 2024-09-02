using Backend.Dtos.Temperatures;
using Backend.Models.Entities;
using MongoDB.Bson;

namespace Backend.Mappers
{
    public static class TemperaturesMapper
    {
        public static Temperature ToEntity(this CreateTemperatureDto temperatureDto, out Temperature temperature)
        {
            temperature = new Temperature
            {
                Id_oras = ObjectId.Parse(temperatureDto.IdOras),
                Valoare = temperatureDto.Valoare
            };
            return temperature;
        }

        public static Temperature ToEntity(this UpdateTemperatureDto temperatureDto, out Temperature temperature)
        {
            temperature = new Temperature
            {
                Id = ObjectId.Parse(temperatureDto.Id),
                Id_oras = ObjectId.Parse(temperatureDto.IdOras),
                Valoare = temperatureDto.Valoare,
            };
            return temperature;
        }

        public static ShowTemperatureDto ToDto(this Temperature temperature, out ShowTemperatureDto temperatureDto)
        {
            temperatureDto = new ShowTemperatureDto
            {
                Id = temperature.Id.ToString(),
                Timestamp = temperature.Timestamp,
                Valoare = temperature.Valoare
            };
            return temperatureDto;
        }
    }
}
