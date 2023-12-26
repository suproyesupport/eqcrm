using EqCrm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.POS
{
    public class ConsultasController : Controller
    {
        // GET: Consultas
        public ActionResult Ventas()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return View("~/Views/POS/Consultas/Ventas.cshtml");
        }


        public ActionResult AnulacionesyAbonos()
        {


            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return View("~/Views/Consultas/AnulacionesyAbonos.cshtml");
        }



        [HttpPost]
        public object GetVentas(FiltroGenerico doctos)
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

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                string queryFacturas = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, DATE_FORMAT(a.fecha, \"%d-%m-%Y\") AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS FROM facturas a";
                queryFacturas += " LEFT JOIN vendedores b on (a.id_vendedor = b.id_codigo)";
                queryFacturas += "WHERE a.status != 'B'";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " AND a.fecha >= '" + fecha + "'";
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

                queryFacturas += " ORDER BY FECHA DESC";

                LlenarListaVentas.listaVentas = llenar.ListaVentas(queryFacturas, DB, LlenarListaVentas.listaVentas);
            }
            string coca = JsonConvert.SerializeObject(LlenarListaVentas.listaVentas);

            return JsonConvert.SerializeObject(LlenarListaVentas.listaVentas);
        }




        [HttpPost]
        public object GetNC(FiltroGenerico doctos)
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

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                string queryFacturas = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, a.fecha AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS FROM notadecredito a";
                queryFacturas += " LEFT JOIN vendedores b on (a.id_vendedor = b.id_codigo)";
                queryFacturas += "WHERE a.status != 'B'";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " AND a.fecha >= '" + fecha + "'";
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

                LlenarListaVentas.listaVentas = llenar.ListaVentas(queryFacturas, DB, LlenarListaVentas.listaVentas);
            }
            return JsonConvert.SerializeObject(LlenarListaVentas.listaVentas);
        }




        /// <summary>
        /// Esta consulta es para hacer las notas de credito pero se hará con un Json conusmiendo el nuevo api
        /// </summary>
        /// <param name="doctos"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetVentasDevoluciones(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string oBase = (string)(Session["oBase"]);
            string cRespuesta = "";

            string cNit = (string)this.Session["cNit"];

            if (string.IsNullOrEmpty(doctos.fecha1))
            {
                doctos.fecha1 = "";
            }

            if (string.IsNullOrEmpty(doctos.fecha2))
            {
                doctos.fecha2 = "";
            }

           
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                string queryFacturas = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, a.fecha AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS FROM facturas a ";
                queryFacturas += " LEFT JOIN vendedores b on (a.id_vendedor = b.id_codigo)";
                queryFacturas += " WHERE a.status != 'B'";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " AND a.fecha >= '" + fecha + "'";
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

                //LlenarListaVentas.listaVentas = llenar.ListaVentas(queryFacturas, DB, LlenarListaVentas.listaVentas);

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = queryFacturas,
                BaseDatos = oBase

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoTablaInventario = new Resultado();
            resultadoTablaInventario = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoTablaInventario.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                
            }


            return cRespuesta;   //sonConvert.SerializeObject(LlenarListaVentas.listaVentas);
        }
    }
}