using System.ComponentModel.DataAnnotations;

namespace EqCrm.Models
{
    public class Cotizacion
    {
        public string no_factura { get; set; }
        public string ndocto { get; set; }
        public string serie { get; set; }
        public string cserie { get; set; }
        public string fecha { get; set; }
        public string cliente { get; set; }
        public string nit { get; set; }
        public string vendedor { get; set; }
        public string direccion { get; set; }
        public string total { get; set; }
        public string tdescto { get; set; }
        public string obs { get; set; }
        public string producto { get; set; }
        public string cantidad { get; set; }
        public string precio { get; set; }
        public string subtotal { get; set; }
        public string url { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string total_letras { get; set; }
        public string nombre_vendedor { get; set; }
        public string telefono_vendedor { get; set; }
        public string correo_vendedor { get; set; }
        public string tasa { get; set; }

        public string atencion { get; set; }


    }
}