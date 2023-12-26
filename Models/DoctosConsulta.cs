using System;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class DoctosConsulta
  {
     
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }    
        public string id_vendedor { get; set; }
        public string id_agencia  { get; set; }
        public string nit { get; set; }
        public string uuid { get; set; }
        
    }
}
