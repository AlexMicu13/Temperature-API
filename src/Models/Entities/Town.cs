using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.Entities
{
    public class Town
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId Id_tara { get; set; }
        public required string Nume_oras { get; set; }
        public double Latitudine { get; set; }
        public double Longitudine { get; set; }
    }
}
