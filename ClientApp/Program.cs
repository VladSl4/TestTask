using System;
using ClientApp.Configurations;
using Grpc.Net.Client;
using GrpcDocumentService;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Development.json");

// Add services to the container.
var ConfigGrpc = builder.Configuration.GetSection("ConfigGrpc");

UriConfig ConfigurationUri = new UriConfig();
builder.Configuration.GetSection("ConfigGrpc").Bind(ConfigurationUri);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<DocumentGrpc.DocumentGrpcClient>(opt => opt.Address = ConfigurationUri.uri);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
