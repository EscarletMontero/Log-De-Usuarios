using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultarProductos : ControllerBase
    {
        private readonly ContextDB _context;

        public ConsultarProductos(ContextDB context)
        {
            _context = context;
        }



        //Aqui pongo un unico enpoins que me evalue los cuatros puntos mandado
        [HttpGet("Estadisticas")]
        [Authorize]

        public async Task<IActionResult> ObtenerEstadisticasProductos()
        {
            var productos = await _context.Productos.ToListAsync();

            if (!productos.Any())
                return NotFound(new { mensaje = "No hay productos registrados." });

            var productoMasCaro = productos.OrderByDescending(p => p.Precio).FirstOrDefault();
            var productoMasBarato = productos.OrderBy(p => p.Precio).FirstOrDefault();
            var sumaTotalPrecios = productos.Sum(p => p.Precio);
            var precioPromedio = productos.Average(p => p.Precio);

            return Ok(new
            {
                ProductoMasCaro = new
                {
                    productoMasCaro.Id,
                    productoMasCaro.Nombre,
                    productoMasCaro.Precio
                },
                ProductoMasBarato = new
                {
                    productoMasBarato.Id,
                    productoMasBarato.Nombre,
                    productoMasBarato.Precio
                },
                SumaTotalPrecios = sumaTotalPrecios,
                PrecioPromedio = precioPromedio
            });
        }



        // Obtengo  productos por  la categoria
        [Authorize]
        [HttpGet("PorCategoria/{idCategoria}")]
        public async Task<IActionResult> ObtenerProductosPorCategoria(int idCategoria)
        {
            var productos = await _context.Productos
                .Where(p => p.IdCategoria == idCategoria)
                .ToListAsync();

            if (!productos.Any())
                return NotFound(new { mensaje = "No hay productos en esta categoría." });

            return Ok(productos);
        }

        // Obtener los productos por proveedor
        [HttpGet("PorProveedor/{idProveedor}")]
        public async Task<IActionResult> ObtenerProductosPorProveedor(int idProveedor)
        {
            var productos = await _context.Productos
                .Where(p => p.IdProveedor == idProveedor)
                .ToListAsync();

            if (!productos.Any())
                return NotFound(new { mensaje = "No hay productos de este proveedor." });

            return Ok(productos);
        }

        // Obtengo la cantidad total de productos
        [HttpGet("TotalProductos")]
        public async Task<IActionResult> ObtenerTotalProductos()
        {
            int totalProductos = await _context.Productos.CountAsync();
            return Ok(new { total = totalProductos });
        }
    }
}
