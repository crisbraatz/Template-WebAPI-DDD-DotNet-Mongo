using Domain.Entities.Users;
using Infrastructure.Mongo.Common;
using Infrastructure.Mongo.Mappings.Users;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Repositories.Users;

public class UserRepository : BaseEntityRepository<User, UserMapping>
{
    public UserRepository(IMongoDatabase database) : base(database)
    {
    }
}