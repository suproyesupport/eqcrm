using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class FiltroGenerico
    {
        public string idcliente { get; set; }
        public string cliente { get; set; }
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }
        public int consulta { get; set; }
        public string linea { get; set; }
        public string agencia { get; set; }

        public string facturas { get; set; }
        public string envios { get; set; }
        public string consolidado { get; set; }

        //PARA EL CORTE DE CAJA Y PROCEDIMIENTO INTERNO
        public int id_caja { get; set; }
        public string fecha { get; set; }
        public double efectivocontado { get; set; }
        public double tarjetacontado { get; set; }
        public double chequecontado { get; set; }
        public double valecontado { get; set; }
        public double efectivocalculado { get; set; }
        public double tarjetacalculado { get; set; }
        public double chequecalculado { get; set; }
        public double valecalculado { get; set; }
        public double efectivodiferencia { get; set; }
        public double tarjetadiferencia { get; set; }
        public double chequediferencia { get; set; }
        public double valediferencia { get; set; }
        public double efectivoretiro { get; set; }
        public double tarjetaretiro { get; set; }
        public double chequeretiro { get; set; }
        public double valeretiro { get; set; }
        public string uuidcaja { get; set; }
    
        public string filtro { get; set; }
        public string problema { get; set; }
        public string vendedor { get; set; }
        public string firma { get; set; }

    }
}