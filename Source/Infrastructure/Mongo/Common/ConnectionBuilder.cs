using System;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Common;

public class ConnectionBuilder : IConnectionBuilder
{
    private static string Host => Environment.GetEnvironmentVariable("MONGO_HOST") ?? "127.0.0.1";
    private static int Port => int.Parse(Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017");
    private static string Database => Environment.GetEnvironmentVariable("MONGO_DATABASE") ?? "template";
    private static string Username => Environment.GetEnvironmentVariable("MONGO_USERNAME") ?? "mongo";
    private static string Password => Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? "mongo";

    public string GetConnectionString() => $"mongodb://{Username}:{Password}@{Host}:{Port}/";

    public IMongoDatabase GetDatabase()
    {
        var database = new MongoClient(GetConnectionString()).GetDatabase(Database);

        MappingSetup.Setup(database);

        return database;
    }
}