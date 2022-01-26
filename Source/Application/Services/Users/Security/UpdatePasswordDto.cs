namespace Application.Services.Users.Security;

public class UpdatePasswordDto
{
    public string Email { get; init; }
    public string OldPassword { get; init; }
    public string NewPassword { get; init; }
}