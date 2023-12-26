using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm
{
    public class ListaOrdenesServicio
    {
        public int ID { get; set; }
        public string STATUS { get; set; }
        public string CLIENTE { get; set; }
        public string DIRECCION { get; set; }   
        public string RUTA { get; set; }
        public string FECHA { get; set;  }
        public string FECHAI { get; set; }
        public string FECHAF { get; set; }
        public string OBSERVACION { get; set; }
        public string DESCRIPCION { get; set; }
        public string INICIO { get; set; }
        public string FINALIZACION { get; set; }
        public string TECNICO { get; set; }
    }
}