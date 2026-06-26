using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}