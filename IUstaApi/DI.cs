using Azure.Core;
using IUstaApi.Auth;
using IUstaApi.Data;
using IUstaApi.Models.Entities;
using IUstaApi.Providers;
using IUstaApi.Services.Concrete;
using IUstaApi.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace IUstaApi;

public static class DI
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "My Api - V1",
                    Version = "v1",
                }
            );

            setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Jwt Authorization header using the Bearer scheme/ \r\r\r\n Enter 'Bearer' [space] and then token in the text input below. \r\n\r\n Example : \"Bearer askjhgdjkashdjkasd\""
            });

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id ="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IRequestUserProvider, RequestUserProvider>();

        services.AddIdentity<AppUser, IdentityRole>(setup => { }).AddEntityFrameworkStores<UstaDbContext>();

        services.AddScoped<IJwtService, JwtService>();

        var jwtConfig = new JwtConfig();
        configuration.GetSection("JWT").Bind(jwtConfig);

        services.AddSingleton(jwtConfig);


        // Add Authentication  after Identity

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, setup =>
        {
            setup.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtConfig.Audience,
                ValidIssuer = jwtConfig.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
            };
        });

        services.AddAuthorization();

        return services;
    }
    public async static Task<IServiceProvider> AddRoles(this IServiceProvider services, IConfiguration configuration)
    {
        var container = services.CreateScope();
        var userManager = container.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = container.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("admin"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("admin"));
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

        }        
        if (!await roleManager.RoleExistsAsync("worker"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("worker"));
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

        }
        if (!await roleManager.RoleExistsAsync("customer"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("customer"));
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
        }
        var adminEmail = configuration["AdminRole:Email"];
        var user = await userManager.FindByEmailAsync(adminEmail!);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, configuration["AdminRole:Password"]!);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
            result = await userManager.AddToRoleAsync(user, "admin");
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
        }
        return services;
    }
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IRequestUserProvider, RequestUserProvider>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IWorkerService, WorkerService>();
        //services.AddHostedService<NotifyUserService>();
        return services;
    }


    //public static IServiceCollection AddLoggingPath(this IServiceCollection services, IConfiguration configuration)
    //{
    //    var directoryInfo = new DirectoryInfo("logs");
    //    var fullpath = directoryInfo.FullName;
    //    string logFileName = "log.txt";
    //    string logFilePath = Path.Combine(fullpath, logFileName);

    //    configuration["Serilog:WriteTo:1:Args:path"] = logFilePath;

    //    return services;
    //}
}
