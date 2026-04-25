using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApiParcial.modelo;

namespace WebApiHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Listado oficial de médicos (Requerimiento B.1)
        private readonly string[] MedicosAutorizados = { "MED-1010", "MED-2020", "MED-3030", "MED-4040", "MED-5050" };

        public PacientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Paciente>> PostPaciente([FromBody] Paciente nuevoPaciente)
        {
            // 1. Validación de Autorización
            if (!MedicosAutorizados.Contains(nuevoPaciente.CarnetMedico))
            {
                return Unauthorized(new { mensaje = "Médico no autorizado." });
            }

            // 2. Validación de Capacidad Crítica (Gravedad 5)
            if (nuevoPaciente.Gravedad == 5)
            {
                int criticosEsperando = await _context.Pacientes
                    .CountAsync(p => p.Gravedad == 5 && p.Estado == "En espera");

                if (criticosEsperando >= 5)
                {
                    return BadRequest("Capacidad máxima alcanzada. Redirección inmediata a otro hospital sugerida");
                }
            }

            // 3. Generación de ID Lógico (PAC-2026-XXX)
            int totalPacientes = await _context.Pacientes.CountAsync() + 1;
            nuevoPaciente.CodigoPaciente = $"PAC-2026-{totalPacientes:D3}";
            nuevoPaciente.FechaIngreso = DateTime.Now;

            _context.Pacientes.Add(nuevoPaciente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPacientes), new { id = nuevoPaciente.InternalId }, nuevoPaciente);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            // Extraer a lista (Sin ORDER BY en SQL)
            var lista = await _context.Pacientes.ToListAsync();

            // 4. Algoritmo de Ordenamiento Manual (Burbuja)
            // Criterio: Gravedad (Desc) y luego Fecha (Asc)
            for (int i = 0; i < lista.Count - 1; i++)
            {
                for (int j = 0; j < lista.Count - i - 1; j++)
                {
                    bool intercambiar = false;

                    // Comparar Gravedad
                    if (lista[j].Gravedad < lista[j + 1].Gravedad)
                    {
                        intercambiar = true;
                    }
                    // Si la gravedad es igual, el más antiguo tiene prioridad
                    else if (lista[j].Gravedad == lista[j + 1].Gravedad)
                    {
                        if (lista[j].FechaIngreso > lista[j + 1].FechaIngreso)
                        {
                            intercambiar = true;
                        }
                    }

                    if (intercambiar)
                    {
                        var temp = lista[j];
                        lista[j] = lista[j + 1];
                        lista[j + 1] = temp;
                    }
                }
            }

            return lista;
        }
    }
}