using System;
using System.Threading.Tasks;
using Application.Services.Users.Security;
using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Presentation.Controllers;

/// <response code="401">Unauthorized access.</response>
/// <response code="500">An internal server error has occurred.</response>
[ApiController]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    private readonly IBaseEntityRepository<User> _repository;

    public BaseController(IBaseEntityRepository<User> repository)
    {
        _repository = repository;
    }

    protected string GetClaimFromAuthorization()
    {
        var authorization = Request.Headers[HeaderNames.Authorization];

        return string.IsNullOrWhiteSpace(authorization) ? null : Token.GetClaimFrom(authorization);
    }

    protected async Task ValidateJwtToken()
    {
        var userDb = await _repository.SelectOneBy(x => x.Email == GetClaimFromAuthorization() && x.Active);
        if (userDb == null)
            throw new UnauthorizedAccessException("Unauthorized access.");
    }
}