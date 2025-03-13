using ProductosAPI.Models.Cliente;

namespace ProductosAPI.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion);
        Task<AutorizacionResponse> RefreshToken(string refreshToken); 
    }

}
