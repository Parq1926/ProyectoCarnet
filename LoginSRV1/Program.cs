<<<<<<< HEAD
// Program.cs - Agregar al inicio
=======
>>>>>>> a7a79ac (Actualizacion del Login)
using LoginSRV1.Data;
using LoginSRV1.Endpoints;
using LoginSRV1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD

builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

=======
>>>>>>> a7a79ac (Actualizacion del Login)
// Add services to the container.
builder.Services.AddRazorPages();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();

// HttpClient para consumir la Minimal API
builder.Services.AddHttpClient<ILoginApiClient, LoginApiClient>(client =>
{
    var baseUrl = builder.Configuration["LoginApi:BaseUrl"]
                  ?? throw new InvalidOperationException("LoginApi:BaseUrl no configurado");
=======
// ✅ Registrar servicios
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();  // ← AGREGAR ESTO
builder.Services.AddHttpContextAccessor();

// HttpClient para consumir UsuariosSRV4
builder.Services.AddHttpClient<IUsuarioApiClient, UsuarioApiClient>(client =>
{
    var baseUrl = builder.Configuration["UsuariosApi:BaseUrl"]
                  ?? throw new InvalidOperationException("UsuariosApi:BaseUrl no configurado");
>>>>>>> a7a79ac (Actualizacion del Login)
    client.BaseAddress = new Uri(baseUrl);
});

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

var app = builder.Build();

<<<<<<< HEAD
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // ← Esto muestra errores detallados
=======
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
>>>>>>> a7a79ac (Actualizacion del Login)
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

<<<<<<< HEAD
// Configurar la ruta raíz para que vaya a Login
=======
>>>>>>> a7a79ac (Actualizacion del Login)
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Login");
    await Task.CompletedTask;
});

<<<<<<< HEAD
// Map endpoints - Minimal API
app.MapLoginEndpoints();

// Map Razor Pages
app.MapRazorPages();

// Program.cs - Agregar antes de app.Run()


=======
app.MapLoginEndpoints();
app.MapRazorPages();

>>>>>>> a7a79ac (Actualizacion del Login)
app.Run();