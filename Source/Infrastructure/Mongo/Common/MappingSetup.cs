using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public static class MappingSetup
{
    private static bool _alreadySetup;

    public static string GetCollectionNameFromMapping<TEntity, TMapping>()
        where TEntity : BaseEntity where TMapping : Mapping<TEntity>
    {
        var mapping = typeof(TMapping);
        var property = mapping.GetProperty("CollectionName");
        var instance = Activator.CreateInstance(mapping);

        return (string) property?.GetValue(instance);
    }

    public static void Setup(IMongoDatabase database)
    {
        foreach (var mappingType in GetTypesFromMappings())
        {
            var mapping = Activator.CreateInstance(mappingType);

            CreateCollectionIfNotExist(database, mapping);

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public;

            if (!_alreadySetup)
                mappingType.InvokeMember("Map", bindingFlags, null, mapping, null);

            mappingType.InvokeMember("CreateIndexes", bindingFlags, null, mapping, new object[] {database});
        }

        _alreadySetup = true;
    }

    private static void CreateCollectionIfNotExist(IMongoDatabase database, object mapping)
    {
        var collectionName = GetCollectionNameFromMapping(mapping);

        var collectionExist = database.ListCollectionNames(new ListCollectionNamesOptions
        {
            Filter = new BsonDocument("name", collectionName)
        }).Any();

        if (!string.IsNullOrWhiteSpace(collectionName) && !collectionExist)
            database.CreateCollection(collectionName);
    }

    private static string GetCollectionNameFromMapping(object mapping) => (string) mapping.GetType()
        .GetProperty("CollectionName", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
        ?.GetValue(mapping);

    private static IEnumerable<Type> GetTypesFromMappings() => Assembly.GetExecutingAssembly().GetTypes().Where(x =>
        typeof(IMapping).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface && x != typeof(BaseEntityMapping));
}