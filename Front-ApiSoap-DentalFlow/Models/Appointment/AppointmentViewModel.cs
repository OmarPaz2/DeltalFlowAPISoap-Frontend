using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.Models.Appointment
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DentistId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Hora { get; set; } = string.Empty;

        [Required]
        public string Motivo { get; set; } = string.Empty;

        public string Estado { get; set; } = "PENDING";

        // Datos para mostrar en el listado

        public string NombrePaciente { get; set; } = string.Empty;

        public string NombreOdontologo { get; set; } = string.Empty;

        public string TipoCita { get; set; } = string.Empty;

        public decimal Monto { get; set; }
    }
}