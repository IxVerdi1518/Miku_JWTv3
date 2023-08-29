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
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosDAO _usuariosDAO;
        public IConfiguration _configuration;
        public UsuariosController(UsuariosDAO usuariosDAO, IConfiguration configuration)
        {
            _usuariosDAO = usuariosDAO;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login(Usuarios user)
        {
            ((int id_usuario, int id_rol_user), int id_cliente) = _usuariosDAO.ValidarUsuario(user);
            if (id_usuario != 0)
            {
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim> // Se crean las reclamaciones para el usuario autenticado
                    {
                    new Claim(ClaimTypes.Name, user.correo_elec),
                    new Claim("id_usuario", id_usuario.ToString()),
                    new Claim("id_cliente", id_cliente.ToString()),
                    new Claim("id_rol_user", id_rol_user.ToString())

                    };
                // Agregar una reclamación específica para el rol del usuario
                if (id_rol_user == 1)
                {
                    claims.Add(new Claim("id_rol_user", "1")); // Administrador
                }
                else
                {
                    claims.Add(new Claim("id_rol_user", "2")); // Usuario normal
                }
                // Se crea la identidad del usuario y se realiza la autenticación
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (id_rol_user == 1)
                {
                    var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(4),
                    signingCredentials: singIn
                    );
                    return new JsonResult(new
                    {
                        success = true,
                        message = "SOS ADMIN",
                        result = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                    // return RedirectToAction("Administrador", "Administrador"); // Redirige al panel de administración si es un administrador
                }
                else
                {
                    // Se registra una auditoría y se redirige a la página principal
                    _usuariosDAO.RegistrarAuditoria(id_usuario, DateTime.Now, true);
                    // return RedirectToAction("Index", "Home");
                    var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(4),
                    signingCredentials: singIn
                    );
                    return new JsonResult(new
                    {
                        success = true,
                        message = "SOS CLIENTE",
                        result = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
            }
            else
            {
                return new JsonResult( new
                {
                    success = false,
                    message = "Creedenciales Incorrectas",
                    result = ""
                });
            }
        }
    }
}
