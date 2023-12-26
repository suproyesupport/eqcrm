using System;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
    public class GetInfoDteG4s
    {
       
        public string FACEID { get; set; }
        public DateTime Fecha_de_emision { get; set; }
        public DateTime Fecha_de_certificacion { get; set; }
        public string Numero_interno { get; set; }
        public string NIT_receptor { get; set; }
        public string Nombre_comprador { get; set; }
        public string Tipo_de_documento { get; set; }
        public double Monto_IVA { get; set; }
        public int Monto_total { get; set; }
        public string Estado_del_documento { get; set; }
        public string Fecha_de_anulacion { get; set; }
        public string Motivo_de_anulacion { get; set; }
        public string Moneda { get; set; }
        
    }
}


