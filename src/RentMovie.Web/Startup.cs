﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentMovie.Application.Domain;
using RentMovie.Application.Dtos;
using RentMovie.Application.Domain.Validators;
using RentMovie.Application.Domain.ValueObjects;
using RentMovie.Application.Ports;
using RentMovie.Infrastructure.Adapters;
using RentMovie.Web.Filters;
using RentMovie.Web.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RentMovie.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ModelStateAsyncFilter>();
            });

        // API versioning
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        var openApiInfo = new OpenApiInfo();
        Configuration.Bind("OpenApiInfo", openApiInfo);
        services.AddSingleton(openApiInfo);
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
        });

        // Database
        services.AddDbContext<RentMovieContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("RentMovieContext")));

        services.AddScoped<IDatabaseDrivenPort, DatabaseAdapter>();

        // API Validators
        services.AddScoped<IValidatorFactory>(serviceProvider =>
            new ServiceProviderValidatorFactory(serviceProvider));
        services.AddScoped<IValidator<OwnerCredentialDto>, OwnerCredentialValidator>();
        services.AddScoped<IValidator<UserDto>, UserValidator>();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Authentication
        var auth = new Authentication(
            Configuration["TokenCredentials:Audience"],
            Configuration["TokenCredentials:Issuer"],
            Configuration["TokenCredentials:HmacSecretKey"],
            Configuration["TokenCredentials:ExpireInDays"]);
        services.AddSingleton(auth);

        services
            .AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = auth.Issuer,
                    ValidAudience = auth.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(auth.HmacSecretKey))
                }
            );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
        IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            }
        );

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
