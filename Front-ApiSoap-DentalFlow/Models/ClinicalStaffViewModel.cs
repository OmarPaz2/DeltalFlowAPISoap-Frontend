
using Microsoft.Extensions.Options;
using System.Xml.Linq;

namespace Front_ApiSoap_DentalFlow.Models
{
    public class ClinicalStaffViewModel
    {
        
    public int id { get; set; }
  
    //private UsuarioVM usuario;
   
    public string specialtyName {  get; set; }
    
    public string licenseNumber { get; set; }
    
    public string firstName { get; set; }
       
    public string lastName { get; set; }
      
    public string phone { get; set; }

        public Boolean disponible { get; set; }
    }
}
