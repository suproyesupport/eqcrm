using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
   public class REQUESTDATA
    {
        public int Respuesta { get; set; }
        public string Codigo { get; set; }
        public string Procesador { get; set; }
        public object Mensaje { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class RESPONSE
    {
        public string NIT_EMISOR { get; set; }
        public string TIPO_DTE { get; set; }
        public string GUID { get; set; }
        public string SERIE { get; set; }
        public int NUMERO { get; set; }
        public string ESTATUS { get; set; }
        public DateTime FECHA_DE_EMISION { get; set; }
        public DateTime FECHA_DE_CERTIFICACION { get; set; }
        public string NIT_COMPRADOR { get; set; }
        public string NOMBRE_COMPRADOR { get; set; }
        public double SUBTOTAL_SIN_DESCUENTO { get; set; }
        public double DESCUENTO { get; set; }
        public double SUBTOTAL_CON_DESCUENTO { get; set; }
        public double IVA { get; set; }
        public double TOTAL { get; set; }
        public int ITEMS { get; set; }
        public string DTE { get; set; }
        public string ACUSE_RECIBO_SAT_DTE { get; set; }
        public string ACUSE_RECIBO_ANULACION { get; set; }
        public string ReferenciaInterna { get; set; }
    }

    public class ResponseGet
    {
        public List<REQUESTDATA> REQUEST_DATA { get; set; }
        public List<RESPONSE> RESPONSE { get; set; }
    }


}
