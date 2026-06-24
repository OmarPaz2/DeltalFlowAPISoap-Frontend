using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.Models.Tratamiento
{
    public class TratamientoViewModel
    {

        public int IdTratamiento { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int OdontologoId { get; set; }

        [Required]
        public string Diagnostico { get; set; } = string.Empty;

        [Required]
        public string TipoTratamiento { get; set; } = string.Empty;

        [Required]
        public decimal CostoEstimado { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public int CantSesiones { get; set; }

        public string Estado { get; set; } = "Planificado";

        public string NombresPaciente { get; set; } = string.Empty;
        public string ApellidosPaciente { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string NombresOdontologo { get; set; } = string.Empty;
        public string ApellidosOdontologo { get; set; } = string.Empty;
        public int SesionesRestantes { get; set; }
        public decimal MontoPagado { get; set; }
        public bool Pagado { get; set; }
    }
}
