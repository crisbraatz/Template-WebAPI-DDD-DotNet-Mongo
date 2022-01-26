using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation;
using Presentation.Middlewares;

var webApplicationBuilder = WebApplication.CreateBuilder(args);
webApplicationBuilder.Services.AddPresentation();
webApplicationBuilder.Services.AddApplication();
webApplicationBuilder.Services.AddDomain();
webApplicationBuilder.Services.AddInfrastructure();

var webApplication = webApplicationBuilder.Build();
webApplication.UseMiddleware<ExceptionHandlerMiddleware>();
webApplication.UseSwagger();
webApplication.UseSwaggerUI();
webApplication.UseHttpsRedirection();
webApplication.UseRouting();
webApplication.UseAuthentication();
webApplication.UseAuthorization();
webApplication.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
webApplication.MapControllers();
webApplication.MapHealthChecks("/healthcheck/liveness", new HealthCheckOptions {Predicate = _ => false});
webApplication.MapHealthChecks("/healthcheck/readiness");
webApplication.Run();