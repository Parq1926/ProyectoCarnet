using SRV4_Areas;
using SRV4_Areas.Repository;
using SRV4_Areas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();

var app = builder.Build();

app.MapAreaEndpoints();

app.Run();