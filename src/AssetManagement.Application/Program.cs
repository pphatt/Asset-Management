using AssetManagement.Application.Middlewares;
using AssetManagement.Application.Services;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Data;
using AssetManagement.Data.Helpers.Seeding;
using AssetManagement.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Application.Configurations;
using AssetManagement.Domain.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles(cfg => cfg.RootPath = "FrontEnd/build");
builder.Services.AddOpenApi();
builder.Services.AddSwaggerConfig();
builder.Services.AddSqlServerConfig(builder.Configuration);
builder.Services.AddAuthConfig(builder.Configuration);

// TODO: move these injection code to some separate files
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddTransient<DataSeeder>();

var app = builder.Build();


app.UseCors(builder =>
    builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

// Test database connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AssetManagementDbContext>();
    try
    {
        dbContext.Database.OpenConnection();
        Console.WriteLine("Database connected successfully.");
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}

// Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seeding data in development mode
    using (var scope = app.Services.CreateScope())
    {
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await dataSeeder.SeedAsync();
    }
}

// Errors handling
app.UseMiddleware<ErrorHandlingMiddleware>();

// Security and static files
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "FrontEnd", "build")),
    RequestPath = ""
});

// SPA fallback
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "FrontEnd", "build"))
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();