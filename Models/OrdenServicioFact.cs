using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class OrdenServicioFact
    {//ORDEN
        public string ID_ORDEN { get; set; }
        public string SERIE { get; set; }
        public string FECHA{ get; set; }
        public string IDCLIENTE { get; set; }
        public string CLIENTE { get; set; }
        public string NIT { get; set; }
        public string OBS { get; set; }
    }
}