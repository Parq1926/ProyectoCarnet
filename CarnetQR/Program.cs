using Microsoft.OpenApi.Models;
using SRV14_CarnetQR;
using SRV14_CarnetQR.Repository;
using SRV14_CarnetQR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SRV14 Carnet QR", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<CarnetQRRepository>();
builder.Services.AddScoped<ICarnetQRService, CarnetQRService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("ReactDev");
app.MapCarnetQREndpoints();
app.Run();
