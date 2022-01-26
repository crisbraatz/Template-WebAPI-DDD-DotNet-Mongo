using System.Threading.Tasks;
using Application.Services.Users;
using Application.Services.Users.Security;
using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs.Requests;
using Presentation.DTOs.Responses;

namespace Presentation.Controllers;

public class UsersController : BaseController
{
    private readonly IUsersService _service;

    public UsersController(IUsersService service, IBaseEntityRepository<User> repository) : base(repository)
    {
        _service = service;
    }

    /// <summary>
    /// Authenticate the user by providing its email and password.
    /// </summary>
    /// <response code="200">Returns the token.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [AllowAnonymous]
    [HttpPost(nameof(Authenticate))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<AuthorizationResponse> Authenticate([FromBody] AuthenticationRequest request)
    {
        var authorization = await _service.Authenticate(new AuthenticationDto
        {
            Email = request.Email,
            Password = request.Password
        }, GetClaimFromAuthorization());

        return new AuthorizationResponse {Token = authorization.Token};
    }

    /// <summary>
    /// Inactivate the user by providing its email and password.
    /// </summary>
    /// <response code="200">User inactivted.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete(nameof(Inactivate))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inactivate([FromBody] InactivationRequest request)
    {
        await ValidateJwtToken();

        await _service.Inactivate(new InactivationDto
        {
            Email = request.Email,
            Password = request.Password
        }, GetClaimFromAuthorization());

        return Ok(new {Message = $"Inactivated user {request.Email}."});
    }

    /// <summary>
    /// Register the user by providing its email and password.
    /// </summary>
    /// <response code="200">User registered.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="409">User already registered.</response>
    [AllowAnonymous]
    [HttpPost(nameof(Register))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        await _service.Register(new RegistrationDto
        {
            Email = request.Email,
            Password = request.Password
        }, GetClaimFromAuthorization());

        return Ok(new {Message = $"User {request.Email} registered."});
    }

    /// <summary>
    /// Update the user's password by providing its email, old and new passwords.
    /// </summary>
    /// <response code="200">User's password updated.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [HttpPut(nameof(UpdatePassword))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        await ValidateJwtToken();

        await _service.UpdatePassword(new UpdatePasswordDto
        {
            Email = request.Email,
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword
        }, GetClaimFromAuthorization());

        return Ok(new {Message = $"Updated password for user {request.Email}."});
    }

    /// <summary>
    /// Check if the user is authorized.
    /// </summary>
    /// <response code="200">User authorized.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet(nameof(Test))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Test()
    {
        await ValidateJwtToken();

        return Ok();
    }
}