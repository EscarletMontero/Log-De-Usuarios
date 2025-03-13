namespace ProductosAPI.Models.Cliente
{
    //Clase que devolvera una respuesta al cliente con el token
    public class AutorizacionResponse
    {
        public string Token { get; set; }
        public bool Resultado { get; set; }
        public string RefreshToken { get; set; } 
        public string Msg { get; set; }
    }
}