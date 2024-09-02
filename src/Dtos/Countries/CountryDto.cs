using MongoDB.Bson;

namespace Backend.Dtos.Countries
{
    public class CountryDto
    {
        public required string Id { get; set; }
        public required string Nume { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
