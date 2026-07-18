// UsuariosSRV4/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsuariosSRV4.Data;
using UsuariosSRV4.Endpoints;
using UsuariosSRV4.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Agregar Razor Pages para la UI
builder.Services.AddRazorPages();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ✅ Agregar soporte para archivos estáticos
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// ✅ Middleware para archivos estáticos
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Razor Pages (UI)
app.MapRazorPages();

// ✅ CORREGIDO: Redirigir a Usuarios en lugar de Login
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Usuarios");
    await Task.CompletedTask;
});

// ✅ Map API Endpoints
app.MapUsuarioEndpoints();

app.Run();