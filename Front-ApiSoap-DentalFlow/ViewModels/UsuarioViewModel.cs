using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        [Required]
        public int RolId { get; set; }

        public bool Activo { get; set; }
    }
}