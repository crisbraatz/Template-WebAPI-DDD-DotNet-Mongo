# Template-WebAPI-DDD-DotNet-Mongo

## About

This template delivers:
1. A Domain-Driven Design a.k.a DDD WebAPI built with C# / .NET
2. The infrastructure to run on the latest MongoDB version
3. Documented API including Swagger UI
4. Unit tests using the XUnit framework
5. Mutation tests using the Stryker framework
6. Continuous Integration a.k.a. CI workflow for GitHub Actions

### Dependencies to execute the application locally

- [.NET 6.0 sdk](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## How to

### Execute the application locally?

In the root directory, execute the command `docker compose up`.

Then, [click here](http://localhost:8080/swagger/index.html) to view the Swagger UI.

Or, execute the command `docker compose up mongo` to execute the Mongo only.

Then, run the application through your IDE and [click here](https://localhost:5001/swagger/index.html) to view the Swagger UI.

### Execute the applications's mutation tests locally?

Execute the command `dotnet tool install -g dotnet-stryker` (one time only).

Then, in the `Tests\Unit` directory, execute any of the following commands:

```
dotnet stryker -p Application.csproj
dotnet stryker -p Domain.csproj
```
