using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Mongo.Common;

public class BaseEntityMapping : Mapping<BaseEntity>
{
    public override string CollectionName => string.Empty;

    protected override void MapProperties(BsonClassMap<BaseEntity> mapper)
    {
        mapper.SetIdMember(mapper.GetMemberMap(x => x.Id)
            .SetSerializer(new StringSerializer(BsonType.ObjectId))
            .SetIgnoreIfDefault(true));

        mapper.MapMember(x => x.Id).SetElementName("id").SetIsRequired(true);
        mapper.MapMember(x => x.CreatedAt).SetElementName("created_at").SetIsRequired(true);
        mapper.MapMember(x => x.CreatedBy).SetElementName("created_by").SetIsRequired(true);
        mapper.MapMember(x => x.LastUpdatedAt).SetElementName("last_updated_at").SetIsRequired(true);
        mapper.MapMember(x => x.LastUpdatedBy).SetElementName("last_updated_by").SetIsRequired(true);
        mapper.MapMember(x => x.Active).SetElementName("active").SetIsRequired(true);

        mapper.SetIgnoreExtraElements(true);
        mapper.SetIsRootClass(true);
    }

    protected override void MapIndexes(IndexesManager<BaseEntity> indexesManager)
    {
    }
}