using ClientApp.Configurations;
using GrpcDocumentService;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Development.json");

UriConfig ConfigurationUri = new UriConfig();
builder.Configuration.GetSection("ConfigGrpc").Bind(ConfigurationUri);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<DocumentGrpc.DocumentGrpcClient>(opt => opt.Address = ConfigurationUri.uri);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
