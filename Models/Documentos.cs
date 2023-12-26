using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class Encabezado_Documentos
  {
        [Display(Name = "Datos Identificion de Factura")]
        public string uuid { get; set; }
        public string no_docto_fel { get; set; }
        public string serie_fel { get; set; }
        public string ndocto { get; set; }
        public string cserie_docto { get; set; }
        public string nid_interno { get; set; }
        [Display(Name = "Fechas para imprimir")]
        public string fecha { get; set; }
        public string fecha_certificacion { get; set; }
        public string fecha_pago { get; set; }
        [Display(Name = "Totales")]
        public string tdescto { get; set; }
        public string total { get; set; }
        [Display(Name = "Información Adicional")]
        public string vendedor { get; set; }
        public string dias_credito { get; set; }
        public string no_orden { get; set; }
        public string orden_compra { get; set; }
        public string nit { get; set; }
        public string codigo_cliente { get; set; }
        public string cliente { get; set; }
        public string direccion { get; set; }
        public string hechopor { get; set; }
        public string observaciones { get; set; }
        public string no_abono { get; set; }
        public string id_agencia { get; set; }
        public string id_vendedor { get; set; }

    }
}
