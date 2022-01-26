using Domain.Entities.Users;
using Infrastructure.Mongo.Common;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Mappings.Users;

public class UserMapping : Mapping<User>
{
    public override string CollectionName => "users";

    protected override void MapProperties(BsonClassMap<User> mapper)
    {
        mapper.MapMember(x => x.Email).SetElementName("email").SetIsRequired(true);
        mapper.MapMember(x => x.Password).SetElementName("password").SetIsRequired(true);
        mapper.MapMember(x => x.Key).SetElementName("key").SetIsRequired(true);
    }

    protected override void MapIndexes(IndexesManager<User> indexesManager) => indexesManager.AddIndex(IndexBuilder
        .Ascending(x => x.Email)
        .Ascending(x => x.Active));
}