using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.Entities
{
    public class Country
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public required string Nume_tara { get; set; }
        public double Latitudine { get; set; }
        public double Longitudine { get; set; }
    }
}
