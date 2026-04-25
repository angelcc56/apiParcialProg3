using Microsoft.EntityFrameworkCore;
using webApiParcial.modelo;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Paciente> Pacientes { get; set; }
}