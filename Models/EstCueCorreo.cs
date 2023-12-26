using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class EstCue
    {
        public string logo_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public string direccion_empresa { get; set; }
        public string correo_empresa { get; set; }
        public string telefono_empresa { get; set; }
        public int id_cliente { get; set; }
        public string cliente_nombre { get; set; }
        public string cliente_nit { get; set; }
        public string cliente_direccion { get; set; }
        public string cliente_correo { get; set; }
        public string cliente_telefono { get; set; }
        public string fecha_corte { get; set; }
        public string observaciones { get; set; }
        public string no_cuenta_banco { get; set; }
        public string banco { get; set; }
        public string tipo_de_cuenta { get; set; }
        public List<Detalle> detalle { get; set; }
        public Vencimiento vencimiento { get; set; }
        public Notas notas { get; set; }
    }

    public class Detalle
    {
        public string id_interno { get; set; }
        public string no_docto { get; set; }
        public string serie { get; set; }
        public string firma_electronica { get; set; }
        public string emision { get; set; }
        public string vencimiento { get; set; }
        public string cargos { get; set; }
        public string abonos { get; set; }
        public string saldo { get; set; }
    }

    public class Notas
    {
        public string nota1 { get; set; }
        public string nota2 { get; set; }
        public string nota3 { get; set; }
        public string nota4 { get; set; }
    }

    public class RenderRequest
    {
        public Template template { get; set; }
        public string asunto { get; set; }
        public string email { get; set; }
        public EstCue data { get; set; }
    }

    public class Root
    {
        public RenderRequest renderRequest { get; set; }
    }

    public class Template
    {
        public string shortid { get; set; }
    }

    public class Vencimiento
    {
        public string saldo_no_vencido { get; set; }
        public string vencido_1_30 { get; set; }
        public string vencido_31_60 { get; set; }
        public string vencido_61_90 { get; set; }
        public string mas_de_90 { get; set; }
        public string saldo_total { get; set; }
    }

}