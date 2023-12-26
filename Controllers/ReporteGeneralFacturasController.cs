using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExtreme.AspNet.Mvc;
using DevExpress.Data;
using DevExtreme.AspNet.Data;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using EqCrm.Models;
using System.Globalization;

namespace EqCrm.Controllers
{
public class ReporteGeneralFacturasController : Controller
    {
        // GET: Consultas
        public ActionResult ReporteGeneralFacturas()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            //DROPDOWNLIST AGENCIA
            List<SelectListItem> agencias = new List<SelectListItem>();
            string queryAgencias = "SELECT id_agencia, nombre FROM catagencias";
            stringConexionMySql.LLenarDropDownList(queryAgencias, DB, agencias);
            ViewData["agencias"] = agencias;

            //DROPDOWNLIST VENDEDORES
            List<SelectListItem> vendedores = new List<SelectListItem>();
            string queryVendedores = "SELECT id_codigo, nombre FROM vendedores";
            stringConexionMySql.LLenarDropDownList(queryVendedores, DB, vendedores);
            ViewData["vendedores"] = vendedores;

            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GenerarReporteGenFacturas(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                if (string.IsNullOrEmpty(doctos.fecha1))
                {
                    doctos.fecha1 = "";
                }

                if (string.IsNullOrEmpty(doctos.fecha2))
                {
                    doctos.fecha2 = "";
                }

                if (string.IsNullOrEmpty(doctos.agencia))
                {
                    doctos.agencia = "";
                }

                if (string.IsNullOrEmpty(doctos.vendedor))
                {
                    doctos.vendedor = "";
                }

                StringConexionMySQL llenar = new StringConexionMySQL();
                string DB = (string)this.Session["StringConexion"];
                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                string queryFacturas = "SELECT a.no_factura AS ID_INTERNO, a.serie AS SERIE_INTERNA, a.fecha AS FECHA, a.nit AS NIT, a.cliente AS CLIENTE, a.status AS STATUS, a.id_vendedor AS VENDEDOR, a.total AS TOTAL, a.id_agencia AS AGENCIA, " +
                    "IFNULL((SELECT z.saldo FROM ctacc z WHERE z.docto = a.no_factura AND z.serie = a.serie AND z.id_codigo = a.id_cliente AND z.id_agencia = a.id_agencia ),0)AS SALDO,a.cont_cred AS CONDICION, a.firmaelectronica AS NOAUTORIZACION,a.no_docto_fel AS NO_DOCTO,a.serie_docto_fel AS SERIE_FEL,a.no_contigencia AS NOACCESO,a.email AS MAIL, " +
                    "IFNULL((SELECT ctaca.docto  FROM ctaca LEFT JOIN cuentasbancarias ON ( cuentasbancarias.id_banco = ctaca.id_banco ) WHERE ctaca.docto_r=a.no_factura AND ctaca.serie_r = a.serie AND ctaca.id_codigo= a.id_cliente AND ctaca.id_agencia = a.id_agencia  ORDER BY ctaca.fechaa DESC LIMIT 1),0) AS RECIBOPAGO, " +
                    "IFNULL((SELECT ctaca.fechaa  FROM ctaca LEFT JOIN cuentasbancarias ON ( cuentasbancarias.id_banco = ctaca.id_banco ) WHERE ctaca.docto_r=a.no_factura AND ctaca.serie_r = a.serie AND ctaca.id_codigo= a.id_cliente AND ctaca.id_agencia = a.id_agencia  ORDER BY ctaca.fechaa DESC LIMIT 1),0) AS FECHAAPLICACION, " +
                    "IFNULL((SELECT ctaca.no_deposito  FROM ctaca LEFT JOIN cuentasbancarias ON ( cuentasbancarias.id_banco = ctaca.id_banco ) WHERE ctaca.docto_r=a.no_factura AND ctaca.serie_r = a.serie AND ctaca.id_codigo= a.id_cliente AND ctaca.id_agencia = a.id_agencia  ORDER BY ctaca.fechaa DESC LIMIT 1),0) AS BOLETA, " +
                    "IFNULL((SELECT IFNULL(cuentasbancarias.nombre,'')  FROM ctaca LEFT JOIN cuentasbancarias ON ( cuentasbancarias.id_banco = ctaca.id_banco ) WHERE ctaca.docto_r=a.no_factura AND ctaca.serie_r = a.serie AND ctaca.id_codigo= a.id_cliente AND ctaca.id_agencia = a.id_agencia  ORDER BY ctaca.fechaa DESC LIMIT 1),0) AS BANCO, " +
                    "IFNULL((SELECT y.docto  FROM contrasenas  y  WHERE y.docto_r = a.no_factura AND y.serie_r = a.serie AND y.id_codigo = a.id_cliente AND y.id_agencia = a.id_agencia LIMIT 1),0) AS CONTRASENA " +
                    "FROM facturas a ";

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " WHERE a.fecha >= '" + fecha + "'";
                    }
                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        queryFacturas += " AND a.fecha <= '" + fecha + "'";
                    }
                }

                if (doctos.agencia != "" && !string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                        queryFacturas += " AND a.id_agencia = " + doctos.agencia;
                } else if (doctos.agencia != "" && string.IsNullOrEmpty(doctos.fecha2) && string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " WHERE a.id_agencia = " + doctos.agencia;
                }
                



                if (doctos.vendedor != "" && doctos.agencia != "" && !string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " AND a.id_vendedor = " + doctos.vendedor;
                }
                else if (doctos.vendedor != "" && doctos.agencia != "" && string.IsNullOrEmpty(doctos.fecha2) && string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " AND a.id_vendedor = " + doctos.vendedor;
                }
                else if (doctos.vendedor != "" && doctos.agencia == "" && string.IsNullOrEmpty(doctos.fecha2) && string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " WHERE a.id_vendedor = " + doctos.vendedor;
                }


                LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas = llenar.ListaRpoGenFacturas(queryFacturas, DB, LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
            }
            return JsonConvert.SerializeObject(LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
        }
    }
}















































//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using DevExtreme.AspNet.Mvc;
//using DevExpress.Data;
//using DevExtreme.AspNet.Data;
//using System.Net;
//using System.Net.Http;
//using Newtonsoft.Json;
//using System.IO;
//using System.Data;
//using EqCrm.Models;

//namespace EqCrm.Controllers
//{
//    public class ReporteGeneralFacturasController : Controller
//    {
//        // GET: ReporteGeneralFacturas
//        public ActionResult ReporteGeneralFacturas()
//        {
//            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
//            {
//                return (ActionResult)this.RedirectToAction("Login", "Account");
//            }
//            return (ActionResult)this.View();
//        }


//        [HttpPost]
//        public object GenerarReporteGenFacturas()
//        {
//            string cUserConected = (string)(Session["Usuario"]);
//            string DB = (string)this.Session["StringConexion"];

//            if (string.IsNullOrEmpty(cUserConected))
//            {
//                return (ActionResult)this.RedirectToAction("Login", "Account");
//            }
//            else
//            {
//                StringConexionMySQL llenar = new StringConexionMySQL();

//                string cInst = "SELECT * FROM generalfacturas";

//                LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas = llenar.ListaRpoGenFacturas(cInst, DB, LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
//            }
//            return JsonConvert.SerializeObject(LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
//        }

//    }
//}