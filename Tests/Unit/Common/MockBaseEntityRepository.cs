using System;
using System.Linq.Expressions;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Unit.Common;

public static class MockBaseEntityRepository
{
    public static void AssertDidNotReceiveDeleteOne<T>(this IBaseEntityRepository<T> repository)
        where T : BaseEntity =>
        repository.DidNotReceive().DeleteOne(Arg.Any<Expression<Func<T, bool>>>(), Arg.Any<T>());

    public static void AssertDidNotReceiveInsertOne<T>(this IBaseEntityRepository<T> repository)
        where T : BaseEntity =>
        repository.DidNotReceive().InsertOne(Arg.Any<T>());

    public static void AssertDidNotReceiveSelectOneBy<T>(this IBaseEntityRepository<T> repository)
        where T : BaseEntity =>
        repository.DidNotReceive().SelectOneBy(Arg.Any<Expression<Func<T, bool>>>());

    public static void AssertDidNotReceiveUpdateOne<T>(this IBaseEntityRepository<T> repository)
        where T : BaseEntity =>
        repository.DidNotReceive().UpdateOne(Arg.Any<Expression<Func<T, bool>>>(), Arg.Any<T>());

    public static void AssertReceivedDeleteOne<T>(this IBaseEntityRepository<T> repository,
        Expression<Func<T, bool>> expectedFilter) where T : BaseEntity =>
        repository.Received().DeleteOne(Arg.Do<Expression<Func<T, bool>>>(sentFilter =>
            sentFilter.Should().BeEquivalentTo(expectedFilter)), Arg.Any<T>());

    public static void AssertReceivedInsertOne<T>(this IBaseEntityRepository<T> repository,
        Expression<Predicate<T>> expectedArg) where T : BaseEntity =>
        repository.Received().InsertOne(Arg.Is(expectedArg));

    public static void AssertReceivedSelectOneBy<T>(this IBaseEntityRepository<T> repository,
        Expression<Func<T, bool>> expectedFilter) where T : BaseEntity =>
        repository.Received().SelectOneBy(Arg.Do<Expression<Func<T, bool>>>(sentFilter =>
            sentFilter.Should().BeEquivalentTo(expectedFilter)));

    public static void AssertReceivedUpdateOne<T>(this IBaseEntityRepository<T> repository,
        Expression<Func<T, bool>> expectedFilter) where T : BaseEntity =>
        repository.Received().UpdateOne(Arg.Do<Expression<Func<T, bool>>>(sentFilter =>
            sentFilter.Should().BeEquivalentTo(expectedFilter)), Arg.Any<T>());

    public static void SelectOneByReturn<T>(this IBaseEntityRepository<T> repository,
        Expression<Func<T, bool>> expectedFilter, T entityReturned) where T : BaseEntity =>
        repository.SelectOneBy(Arg.Do<Expression<Func<T, bool>>>(sentFilter =>
                sentFilter.Should().BeEquivalentTo(expectedFilter)))
            .Returns(entityReturned);
}