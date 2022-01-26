using System.Linq;
using Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public abstract class Mapping<T> : IMapping where T : BaseEntity
{
    protected IndexKeysDefinitionBuilder<T> IndexBuilder => Builders<T>.IndexKeys;

    public abstract string CollectionName { get; }
    protected abstract void MapProperties(BsonClassMap<T> mapper);
    protected abstract void MapIndexes(IndexesManager<T> indexesManager);

    public void Map()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            BsonClassMap.RegisterClassMap<T>(mapper =>
            {
                mapper.AutoMap();
                MapProperties(mapper);
                mapper.SetIgnoreExtraElements(true);
            });
    }

    public void CreateIndexes(IMongoDatabase database)
    {
        var indexesManager = new IndexesManager<T>();

        MapIndexes(indexesManager);

        var indexes = indexesManager.GetIndexes()
            .Select(x =>
                new CreateIndexModel<T>(x.Index, new CreateIndexOptions<T> {Background = true, Unique = x.IsUnique}))
            .ToList();
        if (indexes.Any())
            database.GetCollection<T>(CollectionName).Indexes.CreateMany(indexes);
    }
}