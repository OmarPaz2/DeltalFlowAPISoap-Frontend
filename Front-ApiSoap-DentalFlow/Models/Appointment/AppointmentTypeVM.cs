using System.ComponentModel.DataAnnotations;

namespace Front_ApiSoap_DentalFlow.Models.Appointment
{
    public class AppointmentTypeVM
    {
        public long id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        public string name { get; set; }
        [Required(ErrorMessage = "El campo de minutos es obligatorio")]
        public  int minutos { get; set; }
        [Required(ErrorMessage = "El campo precio es obligatorio")]
        public decimal price { get; set; }
    }
}
