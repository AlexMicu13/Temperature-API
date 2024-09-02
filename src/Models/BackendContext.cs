namespace Backend.Models
{
    using Backend.Config;
    using Backend.Models.Entities;
    using MongoDB.Driver;

    public interface IBackendContext
    {
        IMongoCollection<Country> Countries { get; }
        IMongoCollection<Town> Towns { get; }
        IMongoCollection<Temperature> Temperatures { get; }
        static HashSet<string> GetExpectedKeys<T>()
        {
            var keys = new HashSet<string>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                keys.Add(property.Name);
            }

            return keys;
        }
    }

    public class BackendContext : IBackendContext
    {
        private readonly IMongoDatabase _db;
        public BackendContext(MongoDBConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            Console.WriteLine(config.ConnectionString);
            _db = client.GetDatabase(config.Database);

            var keysTara = Builders<Country>.IndexKeys.Ascending(f => f.Nume_tara);
            var indexModelTara = new CreateIndexModel<Country>(keysTara, new CreateIndexOptions { Unique = true });
            _db.GetCollection<Country>("tari").Indexes.CreateOne(indexModelTara);

            var keysOras = Builders<Town>.IndexKeys.Combine(
                Builders<Town>.IndexKeys.Ascending(f => f.Id_tara),
                Builders<Town>.IndexKeys.Ascending(f => f.Nume_oras));
            var indexModelOras = new CreateIndexModel<Town>(keysOras, new CreateIndexOptions { Unique = true });
            _db.GetCollection<Town>("orase").Indexes.CreateOne(indexModelOras);


            var keysTemp = Builders<Temperature>.IndexKeys.Combine(
                Builders<Temperature>.IndexKeys.Ascending(f => f.Timestamp),
                Builders<Temperature>.IndexKeys.Ascending(f => f.Id_oras));
            var indexModelTemp = new CreateIndexModel<Temperature>(keysTemp, new CreateIndexOptions { Unique = true });
            _db.GetCollection<Temperature>("temperaturi").Indexes.CreateOne(indexModelTemp);
        }

        public IMongoCollection<Country> Countries => _db.GetCollection<Country>("tari");
        public IMongoCollection<Town> Towns => _db.GetCollection<Town>("orase");
        public IMongoCollection<Temperature> Temperatures => _db.GetCollection<Temperature>("temperaturi");
    }
}
