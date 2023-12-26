using EqCrm.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EqCrm.Controllers
{
    public class SeguimientoCotizacionesController : Controller
    {
        // GET: SeguimientoCotizaciones
        public ActionResult SeguimientoCotizaciones()
        {

            
            string DB = (string)this.Session["StringConexion"];
            
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            StringConexionMySQL llenar = new StringConexionMySQL();

            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = "select id_codigo,nombre from vendedores";
            llenar.LLenarDropDownList(sentenciaSQL, DB, lineas);
            ViewData["Lineas"] = lineas;

            return (ActionResult)this.View();
        }


        [HttpPost]
        public object Perdida(Cotizacion coti)
        {
            string cUserConected = (string)(Session["Usuario"]);
            bool lError;
            string cInst = "";
            string DB = (string)this.Session["StringConexion"];
            string cError = "";
            string cMensaje;
            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                cInst = "UPDATE proformas SET status = 'P', id_ventaperdida= 'PERDIDA' WHERE ";
                cInst += "no_factura=" + coti.ndocto;
                cInst += " AND serie='" + coti.cserie + "'";
                cInst += " AND id_ventaperdida = '' ";

                StringConexionMySQL ganar = new StringConexionMySQL();

                lError = ganar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;

                    ganar.CerrarConexion();
                    return cMensaje;

                }
                ganar.CerrarConexion();

            }
            return "Actualizada Exitosamente";
        }


        [HttpPost]
        public object Ganada(Cotizacion coti)
        {
            string cUserConected = (string)(Session["Usuario"]);
            bool lError;
            string cInst = "";
            string DB = (string)this.Session["StringConexion"];
            string cError = "";
            string cMensaje;
            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                cInst = "UPDATE proformas SET status = 'G' WHERE ";
                cInst += "no_factura=" + coti.ndocto;
                cInst += " AND serie='" + coti.cserie+"'";
                cInst += " AND id_ventaperdida = '' ";

                StringConexionMySQL ganar = new StringConexionMySQL();

                lError = ganar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                   
                   ganar.CerrarConexion();
                    return cMensaje;

                }
                ganar.CerrarConexion();

            }
            return "Actualizada Exitosamente";
        }




        [HttpPost]
        public object GetCotizaciones(FiltroGenerico doctos)
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

                string queryCotizaciones = "Select p.NO_FACTURA, p.SERIE, p.STATUS,p.FECHA, p.CLIENTE, p.NIT, v.NOMBRE, p.DIRECCION, FORMAT(p.TOTAL,2) AS TOTAL, p.TDESCTO, p.OBS from proformas p inner join vendedores v on p.id_vendedor = v.id_codigo";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (string.IsNullOrEmpty(doctos.linea))
                {
                    if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                    {
                        if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                        {
                            string fecha = parsedFecha1.Year + "-" +
                                (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                                "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                            queryCotizaciones += " WHERE p.fecha >= '" + fecha + "'";
                        }

                    }

                    if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                    {
                        if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                        {
                            string fecha = parsedFecha2.Year + "-" +
                                (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                                "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                            queryCotizaciones += " AND p.fecha <= '" + fecha + "'";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                    {
                        if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                        {
                            string fecha = parsedFecha1.Year + "-" +
                                (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                                "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                            queryCotizaciones += " WHERE p.fecha >= '" + fecha + "'";
                        }

                    }

                    if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                    {
                        if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                        {
                            string fecha = parsedFecha2.Year + "-" +
                                (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                                "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                            queryCotizaciones += " AND p.fecha <= '" + fecha + "'";
                        }
                    }

                    queryCotizaciones += " AND v.id_codigo = '" + doctos.linea + "'";
                }

                LLenarListaCotizaciones.listaSegCotizaciones = llenar.SegCotizaciones(queryCotizaciones, DB, LLenarListaCotizaciones.listaSegCotizaciones);



                //queryCotizaciones = "SELECT FORMAT(sum(total),2) FROM proformas WHERE status = 'G'  ";
                //StringConexionMySQL consultar = new StringConexionMySQL();
                //consultar.EjecutarLectura(queryCotizaciones, DB);
                //if (consultar.consulta.Read())
                //{
                //    @ViewBag.Ganada = consultar.consulta[0].ToString();
                //}
                //else
                //{
                //    ViewData["GANADAS"] = "0.00";
                //}

                //consultar.CerrarConexion();




            }
            return JsonConvert.SerializeObject(LLenarListaCotizaciones.listaSegCotizaciones);
        }



    }


}