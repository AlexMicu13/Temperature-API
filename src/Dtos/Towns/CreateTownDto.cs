using MongoDB.Bson;

namespace Backend.Dtos.Towns
{
    public class CreateTownDto
    {
        public required string IdTara { get; set; }
        public required string Nume { get; set; }
        public required double Lat { get; set; }
        public required double Lon { get; set; }
    }
}
