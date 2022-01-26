using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using Application.Services.Users.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddJwt();
        services.AddSwagger();
    }

    private static void AddJwt(this IServiceCollection services)
    {
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "DEFAULTJWTAUDIENCE";
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "DEFAULTJWTISSUER";
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = Token.GetSecurityKey(),
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidAudience = audience,
                ValidIssuer = issuer
            };
        });
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Description = "JWT authorization header using the bearer scheme.",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = "Bearer",
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    ImmutableArray<string>.Empty
                }
            });
            options.ExampleFilters();
            var filePath =
                Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            options.IncludeXmlComments(filePath);
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Contact = new OpenApiContact
                {
                    Email = "example@template.com"
                },
                Description = "API to manage the Template application.",
                License = new OpenApiLicense
                {
                    Name = "© All rights reserved."
                },
                Title = "Template",
                Version = "v1"
            });
        });
    }
}