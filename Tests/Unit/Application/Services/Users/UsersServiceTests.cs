using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Services.Users;
using Application.Services.Users.Security;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Unit.Common;
using Xunit;

namespace Unit.Application.Services.Users;

public class UsersServiceTests
{
    private User _user;
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUsersService _service;
    private Expression<Func<User, bool>> _expectedFilter;
    private const string DifferentRequestedBy = "different.requestedby@template.com";

    public UsersServiceTests()
    {
        _repository = Substitute.For<IBaseEntityRepository<User>>();
        var logger = Substitute.For<ILogger<UsersService>>();
        _service = new UsersService(_repository, logger);
    }

    [Fact]
    public async Task Should_authenticate_user()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == authentication.Email && x.Active;
        _repository.SelectOneByReturn(
            _expectedFilter,
            new User(
                authentication.Email,
                "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
                new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
                authentication.Email));

        var authorization = await _service.Authenticate(authentication);

        authorization.Token.Should().StartWith("Bearer").And.HaveLength(268);
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
    }

    [Fact]
    public async Task
        Should_throw_exception_if_authenticated_email_different_from_request_email_when_authenticating()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };

        var task = () => _service.Authenticate(authentication, DifferentRequestedBy);

        await task.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated email {DifferentRequestedBy} different from request email {authentication.Email}.");
        _repository.AssertDidNotReceiveSelectOneBy();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_email_format_when_authenticating()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com.",
            Password = "Example123"
        };

        var task = () => _service.Authenticate(authentication);

        await task.Should().ThrowAsync<InvalidEmailFormatException>().WithMessage("Invalid email format.");
        _repository.AssertDidNotReceiveSelectOneBy();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_format_when_authenticating()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com",
            Password = "Example"
        };

        var task = () => _service.Authenticate(authentication);

        await task.Should().ThrowAsync<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
        _repository.AssertDidNotReceiveSelectOneBy();
    }

    [Fact]
    public async Task Should_throw_exception_if_user_not_found_when_authenticating()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == authentication.Email && x.Active;
        _repository.SelectOneByReturn(_expectedFilter, null);

        var task = () => _service.Authenticate(authentication);

        await task.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"User {authentication.Email} not found.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_when_authenticating()
    {
        var authentication = new AuthenticationDto
        {
            Email = "example@template.com",
            Password = "Example1234"
        };
        _expectedFilter = x => x.Email == authentication.Email && x.Active;
        _repository.SelectOneByReturn(
            _expectedFilter,
            new User(
                authentication.Email,
                "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
                new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
                authentication.Email));

        var task = () => _service.Authenticate(authentication);

        await task.Should().ThrowAsync<InvalidPasswordException>().WithMessage("Invalid password.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
    }

    [Fact]
    public async Task Should_inactivate_user()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == inactivation.Email && x.Active;
        _user = new User(
            inactivation.Email,
            "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
            new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
            inactivation.Email);
        _repository.SelectOneByReturn(_expectedFilter, _user);

        await _service.Inactivate(inactivation, inactivation.Email);

        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertReceivedDeleteOne(x => x.Id == _user.Id);
    }

    [Fact]
    public async Task
        Should_throw_exception_if_authenticated_email_different_from_request_email_when_inactivating()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };

        var task = () => _service.Inactivate(inactivation, DifferentRequestedBy);

        await task.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated email {DifferentRequestedBy} different from request email {inactivation.Email}.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveDeleteOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_email_format_when_inactivating()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com.",
            Password = "Example123"
        };

        var task = () => _service.Inactivate(inactivation, inactivation.Email);

        await task.Should().ThrowAsync<InvalidEmailFormatException>().WithMessage("Invalid email format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveDeleteOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_format_when_inactivating()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com",
            Password = "Example"
        };

        var task = () => _service.Inactivate(inactivation, inactivation.Email);

        await task.Should().ThrowAsync<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveDeleteOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_user_not_found_when_inactivating()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == inactivation.Email && x.Active;
        _repository.SelectOneByReturn(_expectedFilter, null);

        var task = () => _service.Inactivate(inactivation, inactivation.Email);

        await task.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"User {inactivation.Email} not found.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertDidNotReceiveDeleteOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_when_inactivating()
    {
        var inactivation = new InactivationDto
        {
            Email = "example@template.com",
            Password = "Example1234"
        };
        _expectedFilter = x => x.Email == inactivation.Email && x.Active;
        _repository.SelectOneByReturn(
            _expectedFilter,
            new User(
                inactivation.Email,
                "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
                new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
                inactivation.Email));

        var task = () => _service.Inactivate(inactivation, inactivation.Email);

        await task.Should().ThrowAsync<InvalidPasswordException>().WithMessage("Invalid password.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertDidNotReceiveDeleteOne();
    }

    [Fact]
    public async Task Should_register_user()
    {
        var registration = new RegistrationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == registration.Email && x.Active;
        _repository.SelectOneByReturn(_expectedFilter, null);

        await _service.Register(registration);

        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertReceivedInsertOne(x =>
            x.Email == registration.Email &&
            x.Active);
    }

    [Fact]
    public async Task Should_throw_exception_if_authenticated_user_when_registering()
    {
        var registration = new RegistrationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };

        var task = () => _service.Register(registration, DifferentRequestedBy);

        await task.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Authenticated user can not request registration.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveInsertOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_email_format_when_registering()
    {
        var registration = new RegistrationDto
        {
            Email = "example@template.com.",
            Password = "Example123"
        };

        var task = () => _service.Register(registration);

        await task.Should().ThrowAsync<InvalidEmailFormatException>().WithMessage("Invalid email format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveInsertOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_format_when_registering()
    {
        var registration = new RegistrationDto
        {
            Email = "example@template.com",
            Password = "Example"
        };

        var task = () => _service.Register(registration);

        await task.Should().ThrowAsync<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveInsertOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_user_already_registered_when_registering()
    {
        var registration = new RegistrationDto
        {
            Email = "example@template.com",
            Password = "Example123"
        };
        _expectedFilter = x => x.Email == registration.Email && x.Active;
        _repository.SelectOneByReturn(
            _expectedFilter,
            new User(
                registration.Email,
                "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
                new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
                registration.Email));

        var task = () => _service.Register(registration);

        await task.Should().ThrowAsync<UserAlreadyRegisteredException>()
            .WithMessage($"User {registration.Email} already registered.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertDidNotReceiveInsertOne();
    }

    [Fact]
    public async Task Should_update_user_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example123",
            NewPassword = "Example1234"
        };
        _expectedFilter = x => x.Email == updatePassword.Email && x.Active;
        _user = new User(
            updatePassword.Email,
            "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
            new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
            updatePassword.Email);
        _repository.SelectOneByReturn(_expectedFilter, _user);

        await _service.UpdatePassword(updatePassword, updatePassword.Email);

        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertReceivedUpdateOne(x => x.Id == _user.Id);
    }

    [Fact]
    public async Task
        Should_throw_exception_if_authenticated_email_different_from_request_email_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example123",
            NewPassword = "Example1234"
        };

        var task = () => _service.UpdatePassword(updatePassword, DifferentRequestedBy);

        await task.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated email {DifferentRequestedBy} different from request email {updatePassword.Email}.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_email_format_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com.",
            OldPassword = "Example123",
            NewPassword = "Example1234"
        };

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<InvalidEmailFormatException>().WithMessage("Invalid email format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_old_password_format_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example",
            NewPassword = "Example1234"
        };

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_new_password_format_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example123",
            NewPassword = "Example"
        };

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_old_and_new_password_are_equal_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example123",
            NewPassword = "Example123"
        };

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<InvalidPasswordException>().WithMessage("Invalid password.");
        _repository.AssertDidNotReceiveSelectOneBy();
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_user_not_found_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example123",
            NewPassword = "Example1234"
        };
        _expectedFilter = x => x.Email == updatePassword.Email && x.Active;
        _repository.SelectOneByReturn(_expectedFilter, null);

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"User {updatePassword.Email} not found.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertDidNotReceiveUpdateOne();
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_password_when_updating_password()
    {
        var updatePassword = new UpdatePasswordDto
        {
            Email = "example@template.com",
            OldPassword = "Example12",
            NewPassword = "Example1234"
        };
        _expectedFilter = x => x.Email == updatePassword.Email && x.Active;
        _repository.SelectOneByReturn(
            _expectedFilter,
            new User(
                updatePassword.Email,
                "9WJGwsbkWSuMQunGmxTenQrmyEiGYWWVMz8UlQGP84g=",
                new byte[] {123, 242, 165, 203, 169, 250, 254, 34, 155, 93, 39, 160, 81, 232, 115, 194},
                updatePassword.Email));

        var task = () => _service.UpdatePassword(updatePassword, updatePassword.Email);

        await task.Should().ThrowAsync<InvalidPasswordException>().WithMessage("Invalid password.");
        _repository.AssertReceivedSelectOneBy(_expectedFilter);
        _repository.AssertDidNotReceiveUpdateOne();
    }
}