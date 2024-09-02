using Backend.Dtos.Countries;
using Backend.Models.Entities;
using MongoDB.Bson;

namespace Backend.Mappers
{
    public static class CountriesMapper
    {
        public static CountryDto ToDto(Country country, out CountryDto countryDto)
        {
            countryDto = new CountryDto
            {
                Id = country.Id.ToString(),
                Nume = country.Nume_tara,
                Lat = country.Latitudine,
                Lon = country.Longitudine
            };

            return countryDto;
        }

        public static Country ToEntity(CreateCountryDto createCountryDto, out Country country)
        {
            country = new Country
            {
                Nume_tara = createCountryDto.Nume,
                Latitudine = createCountryDto.Lat,
                Longitudine = createCountryDto.Lon
            };

            return country;
        }

        public static Country ToEntity(CountryDto countryDto, out Country country)
        {
            country = new Country
            {
                Id = ObjectId.Parse(countryDto.Id),
                Nume_tara = countryDto.Nume,
                Latitudine = countryDto.Lat,
                Longitudine = countryDto.Lon
            };

            return country;
        }
    }
}
