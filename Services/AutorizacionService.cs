using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductosAPI.Models.Cliente;
using Microsoft.Extensions.Configuration;
using ProductosAPI.Models;

namespace ProductosAPI.Services
{
    public class AutorizacionService : IAutorizacionService
    {
        private readonly ContextDB _context;
        private readonly IConfiguration _configuration;
        // Guardo tokens de refresco
        private static Dictionary<string, string> _refreshTokens = new Dictionary<string, string>(); 

        public AutorizacionService(ContextDB context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Devolvuelve el token cuando el usuario se autentica
        public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x =>
                x.Correo == autorizacion.Correo && x.PassWord == autorizacion.PassWord);

            if (usuario == null)
                return await Task.FromResult<AutorizacionResponse>(null);

            return GenerarTokens(usuario.Id.ToString());
        }




        // Genero el access token y refresh token
        private AutorizacionResponse GenerarTokens(string userId)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            // Pongo el refresh token que expire en 3 minutos
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            _refreshTokens[refreshToken] = userId;

            return new AutorizacionResponse()
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                Resultado = true,
                Msg = "Token generado correctamente"
            };


        }


        public async Task<AutorizacionResponse> RefreshToken(string refreshToken)
        {
            if (!_refreshTokens.ContainsKey(refreshToken))
                return null;
            // Token invalido
            var userId = _refreshTokens[refreshToken];
            // Obtengo usuario del refresh token

            // Volver a generar el token
            var response = GenerarTokens(userId);

            return response;
        }


    }
}
