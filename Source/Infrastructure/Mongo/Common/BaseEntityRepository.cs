using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public abstract class BaseEntityRepository<TEntity, TMapping> : IBaseEntityRepository<TEntity>
    where TEntity : BaseEntity
    where TMapping : Mapping<TEntity>
{
    private readonly IMongoCollection<TEntity> _collection;

    protected BaseEntityRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<TEntity>(MappingSetup.GetCollectionNameFromMapping<TEntity, TMapping>());
    }

    public async Task DeleteOne(Expression<Func<TEntity, bool>> filter, TEntity entity) =>
        await UpdateOne(filter, entity);

    public async Task InsertOne(TEntity entity) => await _collection.InsertOneAsync(entity);

    public async Task<TEntity> SelectOneBy(Expression<Func<TEntity, bool>> filter = null) =>
        await _collection.Find(filter).FirstOrDefaultAsync();

    public async Task UpdateOne(Expression<Func<TEntity, bool>> filter, TEntity entity) =>
        await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions {IsUpsert = true});
}