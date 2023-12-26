using System;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class RespuestaFelDigifact
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public string AcuseRecibidoSAT { get; set; }
        public string CodigosSAT { get; set; }
        public string ResponseDATA1 { get; set; }
        public string ResponseDATA2 { get; set; }
        public string ResponseDATA3 { get; set; }
        public string Autorizacion { get; set; }
        public string Serie { get; set; }
        public string NUMERO { get; set; }
        public string Fecha_DTE { get; set; }
        public string NIT_EFACE { get; set; }
        public string NOMBRE_EFACE { get; set; }
        public string NIT_COMPRADOR { get; set; }
        public string BACKPROCESOR { get; set; }
        public string Fecha_de_certificacion { get; set; }

    }
}
