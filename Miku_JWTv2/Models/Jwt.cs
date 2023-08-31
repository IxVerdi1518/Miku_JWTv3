namespace Miku_JWTv2.Models
{
    public class Jwt
    {
        public string Key { get; set; } // Clave secreta utilizada para firmar y verificar los tokens JWT
        public string Issuer { get; set; } // Emisor del token JWT (quién emite el token)
        public string Audience { get; set; } // Audiencia a la que está destinado el token JWT (dónde se puede usar)
        public string Subject { get; set; } // Asunto del token JWT, generalmente utilizado para identificar el propósito o contexto
    }

}
