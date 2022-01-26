using Presentation.DTOs.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.SwaggerDocs.Requests;

public class RegistrationRequestExample : IExamplesProvider<RegistrationRequest>
{
    public RegistrationRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Password = "Example123"
    };
}