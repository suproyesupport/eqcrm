//CLASE PARA LLEVAR ESTRUCTURA DE DATOS DE PARAMETROS FEL

namespace EqCrm
{
    public class FelEstructura
    {
        struct ParametrosFEL
        {
            public static string NombreEmisor = "";
            public static string NombreComercial = "";
            public static string Direccion = "";
            public static string AfiliacionIva = "";
            public static string Nit = "";
            public static string Email = "";
            public static string Url = "";
            public static string Requestor = "";
            public static string User = "";
            
        }



        public struct DatosEmisor
        {
            public string Tipo;
            public string FechaHoraEmision;
            public string CodigoMoneda;
            public string NitEmisor;
            public string NombreEmisor;
            public string CodigoEstablecimiento;
            public string NombreComercial;
            public string CorreoEmisor;
            public string AfiliacionIva;
            public string Direccion;
            public string CodigoPostal;
            public string Municipio;
            public string Departamento;
            public string Pais;
            public string cExp;
            public string cCorreos;
            public string cAsunto;
            public string TipoPersoneria ;
        }

        public struct DatosReceptor
        {
            public string IdReceptor;
            public string NombreReceptor;
            public string CorreoReceptor;
            public string Direccion;
            public string CodigoPostal;
            public string Municipio;
            public string Departamento;
            public string Pais;
            public string TipoEspecial;
        }

        public struct DatosFrases
        {
            public string TipoFrase;
            public string CodigoEscenario;
        }


        public struct Items
        {
            public string NumeroLinea;
            public string BienOServicio;
            public string Cantidad;
            public string UnidadMedida;
            public string Descripcion;
            public string PrecioUnitario;
            public string Precio;
            public string Descuento;
            public string NombreCorto;
            public string CodigoUnidadGravable;
            public string MontoGravable;
            public string MontoImpuesto;
            public string Total;

        }

        public struct Idp
        {
            public string NombreCorto;
            public string CodigoUnidadGravable;
            public string CantidadUnidadesGravables;
            public string MontoImpuesto;
            public string Total;

        }

        public struct Totales
        {
            public string NombreCorto;
            public string TotalMontoImpuesto;
            public string GranTotal;
        }

        public struct TotalIdp
        {
            public string NombreCorto;
            public string TotalMontoImpuesto;
        }

        public struct ComplementoFacturaCambiaria
        {
            public string NumeroAbono;
            public string FechaVencimiento;
            public string MontoAbono;
        }

        public struct ComplementoNotadeCredito
        {
            public string NumeroAutorizacionDocumentoOrigen;
            public string FechaEmisionDocumentoOrigen;
            public string MotivoAjuste;
            public string SerieDocumentoOrigen;
            public string NumeroDocumentoOrigen;
        }


        public struct ComplementoFacturaEspecial

        {
            public string RetencionISR;
            public string RetencionIVA;
            
            public string TotalMenosRetenciones;
            
        }

        public struct ComplementoFacturaExportacion
        {
            public string NombreDistanatario;
            public string DireccionDestinatario;
            public string Incoterm;
            public string CodigoConsignatario;
            public string NombreComprador;
            public string DireccionComprador;
            public string CodigoComprador;
            public string NombreExportador;
            public string CodigoExportador;
            public string OtraReferencia;

        }

        public struct Adenda
        {
            public string leyendafacc;

        }
    }
}
