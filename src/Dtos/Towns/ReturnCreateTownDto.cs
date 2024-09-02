using Backend.Enums;
using MongoDB.Bson;

namespace Backend.Dtos.Towns
{
    public class ReturnCreateTownDto
    {
        public ObjectId Id { get; set; }
        public ErrorTownCodes Error { get; set; }
    }
}
