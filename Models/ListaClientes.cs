using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class ListaClientes
    {
        public string CODIGO { get; set; }
        public string CLIENTE { get; set; }
        public string NIT { get; set; }
        public string TELEFONO { get; set; }
        public string CORREO { get; set; }
        public string DIRECCION { get; set; }
        public string OBSERVACION { get; set; }
        public string TIPO { get; set; }
        public string VENDEDOR { get; set; }
        public string ATENCION { get; set; }
        public string FECHA_INICIO { get; set; }
        public string DIAS_CREDITO { get; set; }
        public string LIM_CRED { get; set; }
        public string COMISION { get; set; }
        public string STATUS { get; set; }
    }
}