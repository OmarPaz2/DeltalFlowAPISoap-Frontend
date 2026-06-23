namespace Front_ApiSoap_DentalFlow.Models
{
    public class PacienteRequest
    {
        private String dni { get; set; }

        private String firstName { get; set; }

        private String lastName { get; set; }

        private DateOnly birthDate { get; set; }

        private String gender { get; set; }

        private String phone { get; set; }

        private String email { get; set; }

        private String address { get; set; }
    }
}
