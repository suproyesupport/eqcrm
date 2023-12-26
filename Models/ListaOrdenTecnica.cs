using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class ListaOrdenTecnica
    {
        public string IDORDEN { get; set; }
        public string SERIE { get; set; }
        public string FECHA { get; set; }
        public string STATUS { get; set; }
        public string IDCLIENTE { get; set; }
        public string CLIENTE { get; set; }
        public string NIT { get; set; }
        public string PLACA { get; set; }
        public string MARCAVEHICULO { get; set; }
        public string TIPOVEHICULO { get; set; }
        public string LINEA { get; set; }
        public string TOTAL { get; set; }
    }
}