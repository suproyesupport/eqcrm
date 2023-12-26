using System.ComponentModel.DataAnnotations;

namespace EqCrm.Models.POS
{
    public class Caja
    {

        public string id_caja { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Asignada { get; set; }
        public string id { get; set; }
        
        public string cTipoDocto { get; set; }

        public string factsinexist { get; set; }
        public string incoterm { get; set; }

    }
}