using EqCrm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ReporteTrasladoBodegasController : Controller
    {
        // GET: ReporteTrasladoBodegas
        public ActionResult ReporteTrasladoBodegas()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GenerarReporte(FiltroGenerico doctos)
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

                //string queryFacturas = "SELECT * FROM generalfacturas";
                string query = "SELECT a.no_factura AS NO_TRASLADO, a.serie AS SERIE, " +
                    "CASE " +
                    "    WHEN (SELECT 'A' FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 4 LIMIT 1) = 'A' THEN 'TRASLADO ANULADO' " +
                    "    WHEN (SELECT 'R' FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 7 LIMIT 1) = 'R' THEN 'TRASLADO RECEPCIONADO' " +
                    "ELSE 'PENDIENTE' " +
                    "END  AS STATUS, fecha AS FECHA, a.id_agencia AS AGENCIA_TRASLADO, a.id_cliente AS AGENCIA_RECIBE, a.cliente AS NOMBRE, a.obs AS OBSERVACIONES,a.hechopor AS REALIZADO_POR, " +
                    "CASE WHEN (SELECT 'A' FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 4 LIMIT 1) = 'A' THEN  IFNULL((SELECT hechopor FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 4 LIMIT 1),0) END AS ANULADO_POR, " +
                    "CASE WHEN (SELECT 'R' FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 7 LIMIT 1) = 'R' THEN (SELECT hechopor FROM movtraslados b WHERE b.docto = a.no_factura AND  b.serie = a.serie AND b.id_movi = 7 LIMIT 1) END AS RECEPCIONADO_POR " +
                    "FROM traslados a ";
                    //"WHERE a.fecha >= '2023-02-01' ";


                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        query += " WHERE a.fecha >= '" + fecha + "'";
                    }
                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        query += " AND a.fecha <= '" + fecha + "'";
                    }
                }

                query += "ORDER BY a.fecha DESC ";

                LlenarLista.listaRpoTrasEntreBodegas = llenar.ListaRpoTrasladoBodegas(query, DB, LlenarLista.listaRpoTrasEntreBodegas);
            }
            return JsonConvert.SerializeObject(LlenarLista.listaRpoTrasEntreBodegas);
        }
    }
}