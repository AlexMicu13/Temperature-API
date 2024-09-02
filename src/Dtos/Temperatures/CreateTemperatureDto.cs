using MongoDB.Bson;

namespace Backend.Dtos.Temperatures
{
    public class CreateTemperatureDto
    {
        public required string IdOras { get; set; }
        public required double Valoare { get; set; }
    }
}
