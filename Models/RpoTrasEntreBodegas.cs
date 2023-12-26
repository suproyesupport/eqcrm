using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class RpoTrasEntreBodegas
    {
        public string NO_TRASLADO { get; set; }
        public string SERIE { get; set; }
        public string STATUS { get; set; }
        public string FECHA { get; set; }
        public string AGENCIA_TRASLADO { get; set; }
        public string AGENCIA_RECIBE { get; set; }
        public string NOMBRE { get; set; }
        public string OBSERVACIONES { get; set; }
        public string REALIZADO_POR { get; set; }
        public string ANULADO_POR { get; set; }
        public string RECEPCIONADO_POR { get; set; }
    }
}