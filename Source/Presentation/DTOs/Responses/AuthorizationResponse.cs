using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs.Responses;

public class AuthorizationResponse
{
    /// <summary>
    /// JWT authorization header using the bearer scheme.
    /// </summary>
    [Required]
    public string Token { get; init; }
}