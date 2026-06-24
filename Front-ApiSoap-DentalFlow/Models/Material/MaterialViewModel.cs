using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.Models.Material
{
    public class MaterialViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int StockMinimo { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        public decimal CostoUnitario { get; set; }

        public bool EsStockCritico => Stock <= StockMinimo;
    }
}
