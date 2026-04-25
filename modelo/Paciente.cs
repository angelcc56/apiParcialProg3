using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace webApiParcial.modelo
{
    [Table("pacientes_7392")]
    public class Paciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int InternalId { get; set; }

        [Column("codigo_paciente")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CodigoPaciente { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio")]
        public string Nombre { get; set; }

        [Range(1, 5, ErrorMessage = "La gravedad debe estar entre 1 (Estable) y 5 (Crítico)")]
        public int Gravedad { get; set; }

        [Required]
        // Valores permitidos: En espera, Atendido o Derivado
        public string Estado { get; set; }

        [Required]
        [Column("carnet_medico")]
        public string CarnetMedico { get; set; }

        [JsonIgnore] // Se asigna automáticamente en el servidor
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
    }
}