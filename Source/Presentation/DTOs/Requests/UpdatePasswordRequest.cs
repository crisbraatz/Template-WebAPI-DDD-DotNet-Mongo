using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs.Requests;

public class UpdatePasswordRequest
{
    /// <summary>
    /// Valid formatted email.
    /// </summary>
    [Required]
    public string Email { get; init; }

    /// <summary>
    /// At least one lower case letter, one upper case letter and one number.
    /// Must have between 8 and 16 characters.
    /// </summary>
    [Required]
    public string OldPassword { get; init; }

    /// <summary>
    /// At least one lower case letter, one upper case letter and one number.
    /// Must have between 8 and 16 characters.
    /// Must be different than old password.
    /// </summary>
    [Required]
    public string NewPassword { get; init; }
}