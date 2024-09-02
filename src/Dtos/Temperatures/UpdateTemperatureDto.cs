using MongoDB.Bson;

namespace Backend.Dtos.Temperatures
{
    public class UpdateTemperatureDto
    {
        public required string Id { get; set; }
        public required string IdOras { get; set; }
        public double Valoare { get; set; }
    }
}
