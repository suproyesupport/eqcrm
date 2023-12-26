using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class RpoListadoDeCobradoresC
    {
        public string CODIGO { get; set; }
        public string CLIENTE { get; set; }
        public string FECHAI { get; set; }
        public string RUTA { get; set; }
        public string DIRECCION { get; set; }
        public string TELEFONO { get; set; }
        public string FECHA { get; set; }
        public string MES { get; set; }
        public string MESACOBRAR { get; set; }
        public string IMPORTE { get; set; }
        public string SALDO { get; set; }
    }
}