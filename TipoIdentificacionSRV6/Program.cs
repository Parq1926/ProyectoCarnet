using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
<<<<<<< HEAD
using SRV6_TipoIdentificacion.Data;
using SRV6_TipoIdentificacion.Endpoints;
using SRV6_TipoIdentificacion.Interfaces;
using SRV6_TipoIdentificacion.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// DbContext - Conexión a la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔥 Registrar los servicios (para inyección de dependencias)
builder.Services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();

// Configurar autenticación JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? "EstaEsMiClaveSecretaParaJWT1234567890");
=======
using TipoIdentificacionSRV6.Data;
using TipoIdentificacionSRV6.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Agregar Razor Pages
builder.Services.AddRazorPages();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// JWT AUTHENTICATION
// ========================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");
>>>>>>> a7a79ac (Actualizacion del Login)

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
<<<<<<< HEAD
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
=======
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "CUC",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "CUCAapp",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        // Capturar el token de la URL
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
>>>>>>> a7a79ac (Actualizacion del Login)
    });

builder.Services.AddAuthorization();

<<<<<<< HEAD
var app = builder.Build();

// Middleware pipeline
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapear los endpoints
=======
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

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Middleware para capturar token de la URL y agregarlo al header
app.Use(async (context, next) =>
{
    var token = context.Request.Query["token"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }
    await next();
});

// Middleware de redirección para usuarios no autenticados
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401 && !context.Request.Path.StartsWithSegments("/api"))
    {
        var loginUrl = "https://localhost:7019/Login";
        context.Response.Redirect(loginUrl);
    }
});

app.MapRazorPages();
>>>>>>> a7a79ac (Actualizacion del Login)
app.MapTipoIdentificacionEndpoints();

app.Run();