namespace Front_ApiSoap_DentalFlow.Models
{
    public class PacienteRequest
    {
        public string? dni { get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public DateOnly? birthDate { get; set; }

        public string? gender { get; set; }

        public string? phone { get; set; }

        public string? email { get; set; }

        public string? address { get; set; }
    }
}
