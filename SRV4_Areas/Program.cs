using SRV4_Areas;
using SRV4_Areas.Repository;
using SRV4_Areas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

builder.Services.AddHttpClient<IBitacoraService, BitacoraService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapAreaEndpoints();

app.Run();