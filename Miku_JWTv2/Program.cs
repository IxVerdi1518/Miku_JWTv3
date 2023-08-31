using Microsoft.AspNetCore.Authentication.Cookies; // Espacio de nombres para autenticación basada en cookies
using Microsoft.AspNetCore.Authentication.JwtBearer; // Espacio de nombres para autenticación basada en tokens JWT
using Microsoft.IdentityModel.Tokens; // Espacio de nombres para manejar tokens de seguridad
using Miku_JWTv2.DAO; // Espacio de nombres que contiene funcionalidad relacionada con la base de datos
using System.Text; // Espacio de nombres para manipular cadenas de caracteres

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor de inyección de dependencias.
builder.Services.AddControllers(); // Agregar controladores al contenedor
builder.Services.AddEndpointsApiExplorer(); // Agregar soporte para Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // Configurar la generación de la documentación Swagger
builder.Services.AddScoped<UsuariosDAO>(); // Agregar el servicio UsuariosDAO al contenedor
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Configurar autenticación basada en cookies
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Tiempo de expiración de la cookie
            options.LoginPath = "/Usuarios/Login"; // Ruta de redirección para iniciar sesión
            options.AccessDeniedPath = "/Usuarios/AccesoDenegado"; // Ruta de acceso denegado
            // Otras opciones de configuración según tu requerimiento
        });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Configurar autenticación basada en tokens JWT
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Emisor válido del token
            ValidAudience = builder.Configuration["Jwt:Audience"], // Audiencia válida del token
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
app.UseAuthentication(); // Habilitar la autenticación
app.UseAuthorization(); // Habilitar la autorización

app.MapControllers(); // Mapear los controladores

app.Run(); // Iniciar la aplicación

