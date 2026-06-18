using SRV14_CarnetQR;
using SRV14_CarnetQR.Auth;
using SRV14_CarnetQR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHttpClient<ITokenValidator, TokenValidator>();
// Sin base de datos: el servicio usa datos simulados en memoria.
builder.Services.AddScoped<ICarnetQRService, CarnetQRService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("ReactDev");
app.MapCarnetQREndpoints();
app.Run();
