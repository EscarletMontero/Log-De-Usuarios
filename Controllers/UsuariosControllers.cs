using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductosAPI.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductosAPI.Usuarios;

namespace ProductosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ContextDB _context;

        public UsuariosController(ContextDB context)
        {
            _context = context;
        }

        // Endpoint para obtener todos los usuarios (necesitan autenticacion)
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Endpoint para crear un nuevo usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Guardo en logs
            GuardarLog(usuario);

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // Endpoint para obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // Endpoint para actualizar un usuario por ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Metodo para ver si un usuario existe
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        // Endpoint para eliminar un usuario atraves de un id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Endpoint para obtener los logs guardados
        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");  

            if (!System.IO.File.Exists(logFilePath))
            {
                return NotFound("No se encontro el archivo de logs.");
            }

            // Leer todas las lineas del archivo de log
            var logLines = System.IO.File.ReadAllLines(logFilePath);

            // Si el archivo esta vicio, retornara NoContent
            if (logLines.Length == 0)
            {
                return NoContent();
            }

            // Deserializo poniendo el JSon en un object

                var logs = new List<Log>();

                foreach (var line in logLines)
                {
                    try
                    {
                        var log = JsonConvert.DeserializeObject<Log>(line);
                        logs.Add(log);
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest($"Error al procesar el log: {ex.Message}");
                    }
                }

                return Ok(logs);  
        }





        // Metodo que guardar logs cuando se crea un usuario
        private void GuardarLog(Usuario usuario)
        {
            var log = new
            {
                FechaRegistro = DateTime.Now,
                Usuario = usuario
            };

            // Serializar el log a JSON
            string logJson = JsonConvert.SerializeObject(log);

            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");  

            if (!System.IO.File.Exists(logFilePath))
            {
                // Crear el archivo si no existe
                System.IO.File.Create(logFilePath).Dispose();
            }

            // Agregar el log al archivo de logs
            System.IO.File.AppendAllText(logFilePath, logJson + Environment.NewLine);
        }
    }
}
