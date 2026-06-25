using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Front_ApiSoap_DentalFlow.Models.Tratamiento
{
    public class SesionTratameintoVM
    {
        public int idSesion;
        public string nombrePaciente;
        public string apellidoPaciente;
        public string dni;
        public DateTime fechaProgramada;
        public DateTime fechaRealizada;
        public decimal costoParcial;
        public string observaciones;
        public string estado;
        public bool asistenciaPaciente;
    }
}
