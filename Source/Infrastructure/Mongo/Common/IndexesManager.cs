using System.Collections.Generic;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public class IndexesManager<T> where T : BaseEntity
{
    private readonly IList<(IndexKeysDefinition<T>, bool)> _indexes = new List<(IndexKeysDefinition<T>, bool)>();

    public void AddIndex(IndexKeysDefinition<T> index, bool isUnique = false) => _indexes.Add((index, isUnique));

    public IEnumerable<(IndexKeysDefinition<T> Index, bool IsUnique)> GetIndexes() => _indexes;
}