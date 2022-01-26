using Presentation.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.SwaggerDocs.Responses;

public class AuthorizationResponseExample : IExamplesProvider<AuthorizationResponse>
{
    public AuthorizationResponse GetExamples() => new() {Token = "Bearer a0b1c2.d3e4f5g6.h7i8j9"};
}