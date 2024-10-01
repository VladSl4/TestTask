using ServerApp.Configurations;
using ServerApp.Data;
using ServerApp.Models;
using ServerApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json");

// Add services to the container.

builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(nameof(DatabaseConfig)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGrpcService<DocumentService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
