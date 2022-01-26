using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Entities;

public interface IBaseEntityRepository<T> where T : BaseEntity
{
    Task DeleteOne(Expression<Func<T, bool>> filter, T entity);
    Task InsertOne(T entity);
    Task<T> SelectOneBy(Expression<Func<T, bool>> filter = null);
    Task UpdateOne(Expression<Func<T, bool>> filter, T entity);
}