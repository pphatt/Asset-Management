using AssetManagement.Application.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure to serve static files for SPA (in Production)
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "FrontEnd/build";
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//// Serve static files from FrontEnd/build
app.UseDefaultFiles(); // Look for index.html
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "FrontEnd", "build")),
    RequestPath = ""
});

// Fallback to index.html for React Router
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "FrontEnd", "build"))
});

app.UseAuthorization();

app.MapControllers();

app.Run();
