using Microsoft.AspNetCore.Authentication.Cookies; // Espacio de nombres para autenticaci�n basada en cookies
using Microsoft.AspNetCore.Authentication.JwtBearer; // Espacio de nombres para autenticaci�n basada en tokens JWT
using Microsoft.IdentityModel.Tokens; // Espacio de nombres para manejar tokens de seguridad
using Miku_JWTv2.DAO; // Espacio de nombres que contiene funcionalidad relacionada con la base de datos
using System.Text; // Espacio de nombres para manipular cadenas de caracteres

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor de inyecci�n de dependencias.
builder.Services.AddControllers(); // Agregar controladores al contenedor
builder.Services.AddEndpointsApiExplorer(); // Agregar soporte para Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // Configurar la generaci�n de la documentaci�n Swagger
builder.Services.AddScoped<UsuariosDAO>(); // Agregar el servicio UsuariosDAO al contenedor
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Configurar autenticaci�n basada en cookies
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Tiempo de expiraci�n de la cookie
            options.LoginPath = "/Usuarios/Login"; // Ruta de redirecci�n para iniciar sesi�n
            options.AccessDeniedPath = "/Usuarios/AccesoDenegado"; // Ruta de acceso denegado
            // Otras opciones de configuraci�n seg�n tu requerimiento
        });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Configurar autenticaci�n basada en tokens JWT
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Emisor v�lido del token
            ValidAudience = builder.Configuration["Jwt:Audience"], // Audiencia v�lida del token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Clave para firmar y validar el token
        };
    });

var app = builder.Build();

// Configurar el flujo de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilitar Swagger en entorno de desarrollo
    app.UseSwaggerUI(); // Habilitar interfaz web de Swagger en entorno de desarrollo
}

app.UseHttpsRedirection(); // Redirigir solicitudes HTTP a HTTPS
app.UseAuthentication(); // Habilitar la autenticaci�n
app.UseAuthorization(); // Habilitar la autorizaci�n

app.MapControllers(); // Mapear los controladores

app.Run(); // Iniciar la aplicaci�n

