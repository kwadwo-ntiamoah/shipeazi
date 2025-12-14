using Scalar.AspNetCore;
using Shipeazi.API.src.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.RegisterStartupExtensions(builder.Configuration);

var app = builder.Build();

app.Urls.Add(builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000");

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
var enableScalar = builder.Configuration.GetValue<bool>("ApiDocumentation:EnableScalar", false);

if (enableScalar || app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Shipeazi API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
