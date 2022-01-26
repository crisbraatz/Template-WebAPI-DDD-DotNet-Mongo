using System.Threading.Tasks;
using Application.Services.Users.Security;

namespace Application.Services.Users;

public interface IUsersService
{
    Task<AuthorizationDto> Authenticate(AuthenticationDto authentication, string requestedBy = null);
    Task Inactivate(InactivationDto inactivation, string requestedBy);
    Task Register(RegistrationDto registration, string requestedBy = null);
    Task UpdatePassword(UpdatePasswordDto updatePassword, string requestedBy);
}