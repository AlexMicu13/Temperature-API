using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.Entities
{
    public class Temperature
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public double Valoare { get; set; }
        public DateTime Timestamp { get; set; }
        public ObjectId Id_oras { get; set; }
    }
}
