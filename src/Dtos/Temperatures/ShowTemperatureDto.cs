using MongoDB.Bson;

namespace Backend.Dtos.Temperatures
{
    public class ShowTemperatureDto
    {
        public required string Id { get; set; }
        public double Valoare { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
