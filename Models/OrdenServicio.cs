using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class OrdenServicio
    {
        //ORDEN
        public string id_orden { get; set; }
        public string serie { get; set; }
        public string fechaingreso { get; set; }
        public string fechaentrega { get; set; }
        public string asesores { get; set; }
        public string idcliente { get; set; }
        public string cliente { get; set; }
        public string direccion { get; set; }
        public string nit { get; set; }
        public string tipoVenta { get; set; }
        public string diascredito { get; set; }
        public string contacto { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string obs { get; set; }


        //ASEGURADORA
        public string aseguradoras { get; set; }
        public string poliza { get; set; }
        public string asesoremergencia { get; set; }
        public string reclamo { get; set; }
        public string corredora { get; set; }
        public string deducible { get; set; }
        public string ajustador { get; set; }
        public string opciones { get; set; }

        //VEHICULO
        public string tiposvehiculos { get; set; }
        public string marca { get; set; }
        public string lineavehiculo { get; set; }
        public string modelo { get; set; }
        public string tipoplaca { get; set; }
        public string placa { get; set; }
        public string color { get; set; }
        public string chassis { get; set; }
        public string kilometraje { get; set; }
        public string medida { get; set; }

        //DETALLE
        public string id_codigo { get; set; }
        public string cantidad { get; set; }
        public string existencia { get; set; }
        public string precio { get; set; }
        public string tsubtotal { get; set; }
        public string total { get; set; }
        public string codigoe { get; set; }
        public string producto { get; set; }

        //FILTRO
        public string bid_orden { get; set; }
        public string b_placa { get; set; }
        public string bidcliente { get; set; }
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }

        //CHECKLIST
        public string radio { get; set; }
        public string encendedor { get; set; }
        public string documentos { get; set; }
        public string alfombras { get; set; }
        public string llanta { get; set; }
        public string tricket { get; set; }
        public string llave { get; set; }
        public string herramienta { get; set; }
        public string platos { get; set; }

        public string tipo { get; set; }
        public string resumen { get; set; }


        public List<OrdenServicio> lista { get; set; }
    }
}