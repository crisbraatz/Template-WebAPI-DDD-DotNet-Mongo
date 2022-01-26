using Presentation.DTOs.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.SwaggerDocs.Requests;

public class AuthenticationRequestExample : IExamplesProvider<AuthenticationRequest>
{
    public AuthenticationRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Password = "Example123"
    };
}