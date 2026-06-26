namespace Front_ApiSoap_DentalFlow.Models.Tratamiento
{
    public class SesionTratamientoRqVM
    {

        public int idTratamiento { get; set; }
      
	 public DateTime fechaProgramada { get; set; }
        public decimal costoParcial { get; set; }
        public int tiempoDuracion { get; set; }
    }
}
