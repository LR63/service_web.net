using Scalar.AspNetCore;
using GameServerApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder => builder.WithOrigins("https://csharp.nouvet.fr").AllowAnyHeader().AllowAnyMethod());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BDDContext>();
builder.Services.AddScoped<PasswordHasher<User>>();
var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
