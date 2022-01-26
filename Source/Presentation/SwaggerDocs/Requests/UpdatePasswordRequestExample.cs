using Presentation.DTOs.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.SwaggerDocs.Requests;

public class UpdatePasswordRequestExample : IExamplesProvider<UpdatePasswordRequest>
{
    public UpdatePasswordRequest GetExamples() => new()
    {
        Email = "example@template.com",
        OldPassword = "Example123",
        NewPassword = "Example1234"
    };
}