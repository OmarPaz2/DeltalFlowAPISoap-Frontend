using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.Models.Pago
{
    public class PagoViewModel
    {

        public int IdPago { get; set; }

        [Required]
        public int IdReferencia { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        public string MetodoPago { get; set; } = string.Empty;

        public string Razon { get; set; } = string.Empty;
        public string NombresPaciente { get; set; } = string.Empty;
        public string ApellidosPaciente { get; set; } = string.Empty;
        public string NombreEspecialidad { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
    }
}
