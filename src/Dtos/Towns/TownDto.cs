using MongoDB.Bson;

namespace Backend.Dtos.Towns
{
    public class TownDto
    {
        public required string Id { get; set; }
        public required string IdTara { get; set; }
        public required string Nume { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
