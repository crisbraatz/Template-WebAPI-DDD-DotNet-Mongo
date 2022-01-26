using System;
using System.Threading;
using Domain.Entities.Users;
using FluentAssertions;
using Xunit;

namespace Unit.Domain.Entities.Users;

public class UserTests
{
    private DateTime _dateTime;
    private const string Email = "example@template.com";
    private const string Password = "Example123";
    private static readonly byte[] Key = {12, 34, 56, 78, 90};
    private User _user;

    [Fact]
    public void Should_create_an_user()
    {
        _dateTime = DateTime.UtcNow;

        _user = new User(Email, Password, Key, Email);

        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(Password);
        _user.Key.Should().BeEquivalentTo(Key);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _user.CreatedBy.Should().Be(Email);
        _user.LastUpdatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _user.LastUpdatedBy.Should().Be(Email);
        _user.Active.Should().BeTrue();
    }

    [Fact]
    public void Should_inactivate_an_user()
    {
        _dateTime = DateTime.UtcNow;
        _user = new User(Email, Password, Key, Email);
        Thread.Sleep(100);

        _user.Inactivate(Email);

        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(Password);
        _user.Key.Should().BeEquivalentTo(Key);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _user.CreatedBy.Should().Be(Email);
        _user.LastUpdatedAt.Should().BeAfter(_user.CreatedAt);
        _user.LastUpdatedBy.Should().Be(Email);
        _user.Active.Should().BeFalse();
    }

    [Fact]
    public void Should_update_an_user_password()
    {
        _dateTime = DateTime.UtcNow;
        _user = new User(Email, Password, Key, Email);
        const string newPassword = "Example1234";
        var newKey = new byte[] {12, 34, 56, 78, 90, 12};
        Thread.Sleep(100);

        _user.Update(newPassword, newKey, Email);

        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(newPassword);
        _user.Key.Should().BeEquivalentTo(newKey);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _user.CreatedBy.Should().Be(Email);
        _user.LastUpdatedAt.Should().BeAfter(_user.CreatedAt);
        _user.LastUpdatedBy.Should().Be(Email);
        _user.Active.Should().BeTrue();
    }
}