using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Miku_JWTv2.DAO;
using Miku_JWTv2.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Miku_JWTv2.Controllers
{
    [Route("api/[controller]")] // Atributo para especificar la ruta base de la API para este controlador
    [ApiController] // Atributo para indicar que esta clase es un controlador de API
    [AllowAnonymous] // Atributo que permite el acceso a esta acción sin requerir autenticación
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosDAO _usuariosDAO; // Instancia de la clase UsuariosDAO para acceder a la lógica de acceso a datos
        public IConfiguration _configuration; // Instancia de IConfiguration para acceder a la configuración de la aplicación

        public UsuariosController(UsuariosDAO usuariosDAO, IConfiguration configuration)
        {
            _usuariosDAO = usuariosDAO; // Inyección de dependencia para UsuariosDAO
            _configuration = configuration; // Inyección de dependencia para IConfiguration
        }

        [HttpPost] // Atributo que indica que esta acción responde a las solicitudes POST
        public async Task<IActionResult> Login(int id_usuario, int id_cliente, int id_rol_user)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>(); // Obtener la configuración JWT de la sección correspondiente
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)); // Crear una clave simétrica para firmar el token
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Crear credenciales de firma

            var claims = new List<Claim> // Crear una lista de reclamaciones (claims) para el usuario autenticado
        {
            new Claim("id_usuario",id_usuario.ToString()), // Agregar la reclamación de ID de usuario
            new Claim("id_cliente",id_cliente.ToString()), // Agregar la reclamación de ID de cliente
            new Claim("id_rol_user",id_rol_user.ToString()) // Agregar la reclamación de ID de rol de usuario
        };

            var token = new JwtSecurityToken(
                  jwt.Issuer, // Emisor del token
                  jwt.Audience, // Audiencia del token
                  claims, // Lista de reclamaciones
                  expires: DateTime.Now.AddMinutes(4), // Tiempo de expiración del token
                  signingCredentials: singIn // Credenciales de firma
            );

            var newToken = new JwtSecurityTokenHandler().WriteToken(token); // Generar el token como cadena

            return new JsonResult(new
            {
                Url = new Uri("https://localhost:7223/Acceso/Login?token=" + newToken), // URL con el token como parámetro
                success = true,
                message = "SOS ADMIN", // Mensaje
                result = newToken // El token generado
            });
        }
    }

}
