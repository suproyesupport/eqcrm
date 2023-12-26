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

namespace EqCrm.Controllers.POS
{
    public class ReporteGeneralFacturassController : Controller
    {
        // GET: Consultas
        public ActionResult ReporteGeneralFacturass()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> agencias = new List<SelectListItem>();
            string sentenciaSQLlineas = "SELECT id_agencia, nombre FROM catagencias";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLlineas, DB, agencias);
            ViewData["agencias"] = agencias;

            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GenerarReporteGenFacturas(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

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


            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                //string queryFacturas = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, a.fecha AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS FROM facturas a";
                //queryFacturas += " LEFT JOIN vendedores b on (a.id_vendedor = b.id_codigo)";
                //queryFacturas += "WHERE a.status != 'B'";

                string queryFacturas = "SELECT * FROM generalfacturas";


                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " WHERE fecha >= '" + fecha + "'";
                    }

                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        queryFacturas += " AND fecha <= '" + fecha + "'";
                    }
                }

                if (doctos.agencia != "" && !string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " AND id_agencia = " + doctos.agencia;
                }
                else if (doctos.agencia != "" && string.IsNullOrEmpty(doctos.fecha2) && string.IsNullOrEmpty(doctos.fecha1))
                {
                    queryFacturas += " WHERE id_agencia = " + doctos.agencia;
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