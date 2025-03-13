using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.Models.Cliente
{
    //Clase para recibir los datos del cliente y autorizarlo
        public class AutorizacionRequest
        {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo invalido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string PassWord { get; set; }
    }
    }