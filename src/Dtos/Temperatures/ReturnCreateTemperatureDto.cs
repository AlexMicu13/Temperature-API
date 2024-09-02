using Backend.Enums;
using MongoDB.Bson;

namespace Backend.Dtos.Temperatures
{
    public class ReturnCreateTemperatureDto
    {
        public ObjectId Id { get; set; }
        public ErrorTemperatureCodes Error { get; set; }
    }
}
