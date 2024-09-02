using Backend.Enums;
using MongoDB.Bson;

namespace Backend.Dtos.Countries
{
    public class ReturnCreateCountryDto
    {
        public ObjectId Id { get; set; }
        public ErrorCountryCodes Error { get; set; }
    }
}
