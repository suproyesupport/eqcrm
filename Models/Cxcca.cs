using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class Cxcca
    {
        public string codabono { get; set; } //id_movi
        public string idcliente { get; set; } //id de Cliente
        public string cliente { get; set; } //nombre de cliente
        public string cobradores { get; set; } //cobradores                                        
        public string nodoc { get; set; }
        public string saldo { get; set; }
        public string saldoactual { get; set; }
        public string abono { get; set; }
        public string fechaa { get; set; }
        public string fechai { get; set; }
        public string concepto { get; set; }
        public string obs { get; set; }
        public string serie { get; set; }
        public string tipodocto { get; set; }
        public string nodocumento { get; set; }
        public string formapago { get; set; }
    }
}