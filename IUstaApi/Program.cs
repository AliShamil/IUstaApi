using FluentValidation;
using IUstaApi.Models.DTOs.Validations;
using IUstaApi;
using Microsoft.EntityFrameworkCore;
using Serilog;
using IUstaApi.Data;
using IUstaApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using IUstaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddDomainServices();

//loglamada bax
//builder.Services.AddLoggingPath(builder.Configuration);

//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<UstaDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UstaDbConnectionString"));
});

builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:5000").AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);


builder.Services.AddSwagger();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.Services.AddRoles(builder.Configuration);

app.UseCors("NgOrigins");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();