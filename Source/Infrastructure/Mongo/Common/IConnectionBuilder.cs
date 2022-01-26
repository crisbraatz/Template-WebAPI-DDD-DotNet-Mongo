using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public interface IConnectionBuilder
{
    string GetConnectionString();
    IMongoDatabase GetDatabase();
}