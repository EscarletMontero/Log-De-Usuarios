using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ProductosAPI.Models;
using ProductosAPI.Usuarios;

public class UsuarioService
{
    private readonly string logFilePath = "logs.txt"; 
    private readonly ContextDB _context;

    public UsuarioService(ContextDB context)
    {
        _context = context;
    }

    public async Task GuardarUsuario(Usuario usuario)
    {
        // Guardar al usuario en la base de datos
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // pone en el log la fecha y la informacion del usuario
        var log = new
        {
            FechaRegistro = DateTime.Now,
            Usuario = usuario
        };

        // Serializar el objeto log a JSON
        string logJson = JsonConvert.SerializeObject(log);

        // Verificar si el archivo de log existe, si no lo creamos
        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose(); 
        }

        File.AppendAllText(logFilePath, logJson + Environment.NewLine);
    }
}

