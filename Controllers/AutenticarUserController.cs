using Microsoft.AspNetCore.Mvc;
using ProductosAPI.Models.Cliente;
using ProductosAPI.Services;
using System.Threading.Tasks;

namespace ProductosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticarUserController : ControllerBase
    {
        private readonly IAutorizacionService _autorizacionService;

        public AutenticarUserController(IAutorizacionService autorizacionService)
        {
            _autorizacionService = autorizacionService;
        }

        // Endpoint para autenticar y obtener los tokens (Access Token y Refresh Token)
        [HttpPost("Autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] AutorizacionRequest autorizacion)
        {
            var resultado_autorizacion = await _autorizacionService.DevolverToken(autorizacion);
            if (resultado_autorizacion == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            return Ok(resultado_autorizacion);
        }

        // Endpoint para refrescar el token con el Refresh Token
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var resultado_autorizacion = await _autorizacionService.RefreshToken(refreshTokenRequest.RefreshToken);
            if (resultado_autorizacion == null)
                return Unauthorized(new { mensaje = "Refresh token inválido o expirado" });

            return Ok(resultado_autorizacion);
        }
    }

    // Clase para el request de Refresh Token
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
