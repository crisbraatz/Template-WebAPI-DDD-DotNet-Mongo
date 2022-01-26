using System;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Services.Users.Security;
using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Services.Users;

public class UsersService : IUsersService
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IBaseEntityRepository<User> repository, ILogger<UsersService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AuthorizationDto> Authenticate(AuthenticationDto authentication, string requestedBy = null)
    {
        using (_logger.BeginScope(nameof(Authenticate)))
        {
            var email = authentication.Email.ToLower();

            if (!string.IsNullOrWhiteSpace(requestedBy) && requestedBy.ToLower() != email)
                throw new UnauthorizedAccessException(
                    $"Authenticated email {requestedBy} different from request email {email}.");

            ValidateData(email, authentication.Password);

            var userDb = await _repository.SelectOneBy(x => x.Email == email && x.Active);
            if (userDb == null)
                throw new UserNotFoundException($"User {email} not found.");

            if (Password.GenerateHashOnly(authentication.Password, userDb.Key) != userDb.Password)
                throw new InvalidPasswordException("Invalid password.");

            return new AuthorizationDto {Token = $"Bearer {Token.GenerateJwt(email)}"};
        }
    }

    public async Task Inactivate(InactivationDto inactivation, string requestedBy)
    {
        using (_logger.BeginScope(nameof(Inactivate)))
        {
            var email = inactivation.Email.ToLower();

            if (requestedBy.ToLower() != email)
                throw new UnauthorizedAccessException(
                    $"Authenticated email {requestedBy} different from request email {email}.");

            ValidateData(email, inactivation.Password);

            var userDb = await _repository.SelectOneBy(x => x.Email == email && x.Active);
            if (userDb == null)
                throw new UserNotFoundException($"User {email} not found.");

            if (Password.GenerateHashOnly(inactivation.Password, userDb.Key) != userDb.Password)
                throw new InvalidPasswordException("Invalid password.");

            userDb.Inactivate(email);

            await _repository.DeleteOne(x => x.Id == userDb.Id, userDb);
        }
    }

    public async Task Register(RegistrationDto registration, string requestedBy = null)
    {
        using (_logger.BeginScope(nameof(Register)))
        {
            var email = registration.Email.ToLower();

            if (!string.IsNullOrWhiteSpace(requestedBy))
                throw new UnauthorizedAccessException("Authenticated user can not request registration.");

            ValidateData(email, registration.Password);

            var userDb = await _repository.SelectOneBy(x => x.Email == email && x.Active);
            if (userDb != null)
                throw new UserAlreadyRegisteredException($"User {email} already registered.");

            var (key, password) = Password.GenerateSaltAndHash(registration.Password);

            await _repository.InsertOne(new User(email, password, key, email));
        }
    }

    public async Task UpdatePassword(UpdatePasswordDto updatePassword, string requestedBy)
    {
        using (_logger.BeginScope(nameof(UpdatePassword)))
        {
            var email = updatePassword.Email.ToLower();

            if (requestedBy.ToLower() != email)
                throw new UnauthorizedAccessException(
                    $"Authenticated email {requestedBy} different from request email {email}.");

            ValidateData(email, updatePassword.OldPassword);
            Password.ValidateFormat(updatePassword.NewPassword);

            if (updatePassword.OldPassword == updatePassword.NewPassword)
                throw new InvalidPasswordException("Invalid password.");

            var userDb = await _repository.SelectOneBy(x => x.Email == email && x.Active);
            if (userDb == null)
                throw new UserNotFoundException($"User {email} not found.");

            if (Password.GenerateHashOnly(updatePassword.OldPassword, userDb.Key) != userDb.Password)
                throw new InvalidPasswordException("Invalid password.");

            var (key, password) = Password.GenerateSaltAndHash(updatePassword.NewPassword);

            userDb.Update(password, key, email);

            await _repository.UpdateOne(x => x.Id == userDb.Id, userDb);
        }
    }

    private static void ValidateData(string email, string password)
    {
        Email.ValidateFormat(email);
        Password.ValidateFormat(password);
    }
}