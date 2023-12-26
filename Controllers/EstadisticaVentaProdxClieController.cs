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
    public class EstadisticaVentaProdxClieController : Controller
    {
        // GET: EstadisticaVentaProdxClie
        public ActionResult EstadisticaVentaProdxClie()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }



        [HttpPost]
        public object GenerarEstadisticaVentaProdxClie(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string queryunion = "";
            string query1 = "";
            string query2 = "";

            if (string.IsNullOrEmpty(doctos.idcliente))
            {
                doctos.idcliente = "";
            }

            if (string.IsNullOrEmpty(doctos.cliente))
            {
                doctos.cliente = "";
            }

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

                //PRIMERA CONSULTA
                query1 = "SELECT a.id_cliente AS IDCLIENTE, a.cliente AS CLIENTE, d.id_codigo AS CODIGO, d.codigoe AS CODIGOE, d.numero_departe AS ALIAS, b.obs AS PRODUCTO, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 1, b.subtotal , 0)),2) AS ENE, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 2, b.subtotal , 0)),2) AS FEB, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 3, b.subtotal , 0)),2) AS MAR, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 4, b.subtotal , 0)),2) AS ABR, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 5, b.subtotal , 0)),2) AS MAY, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 6, b.subtotal , 0)),2) AS JUN, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 7, b.subtotal , 0)),2) AS JUL, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 8, b.subtotal , 0)),2) AS AGO, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 9, b.subtotal , 0)),2) AS SEP, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 10, b.subtotal , 0)),2) AS OCT, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 11, b.subtotal , 0)),2) AS NOV, ";
                query1 += "FORMAT(SUM(IF(MONTH(a.fecha) = 12, b.subtotal , 0)),2) AS DIC, ";
                query1 += "a.id_vendedor AS IDVENDEDOR,a.fecha AS FECHA,1 as TIPODOCTO ";
                query1 += "FROM facturas a ";
                query1 += "JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie AND b.id_agencia = a.id_agencia) ";
                query1 += "LEFT JOIN vendedores c ON (c.id_codigo = a.id_vendedor) ";
                query1 += "LEFT JOIN inventario d ON (d.id_codigo = b.id_codigo) ";
                query1 += "LEFT JOIN clientes e ON (a.id_cliente = e.id_codigo ) ";
                query1 += "WHERE a.status <> 'A' ";

                //SEGUNDA CONSULTA
                query2 = "SELECT a.id_cliente AS IDCLIENTE, a.cliente AS CLIENTE,  d.id_codigo AS CODIGO,  d.codigoe AS CODIGOE, d.numero_departe AS ALIAS, b.obs AS PRODUCTO, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 1, b.subtotal , 0)),2) AS ENE, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 2, b.subtotal , 0)),2) AS FEB, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 3, b.subtotal , 0)),2) AS MAR, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 4, b.subtotal , 0)),2) AS ABR, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 5, b.subtotal , 0)),2) AS MAY, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 6, b.subtotal , 0)),2) AS JUN, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 7, b.subtotal , 0)),2) AS JUL, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 8, b.subtotal , 0)),2) AS AGO, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 9, b.subtotal , 0)),2) AS SEP, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 10, b.subtotal , 0)),2) AS OCT, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 11, b.subtotal , 0)),2) AS NOV, ";
                query2 += "FORMAT(SUM(IF(MONTH(a.fecha) = 12, b.subtotal , 0)),2) AS DIC, ";
                query2 += "a.id_vendedor AS IDVENDEDOR,a.fecha AS FECHA,2 as TIPODOCTO ";
                query2 += "FROM envios a ";
                query2 += "JOIN detalle_envios b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia) ";
                query2 += "LEFT JOIN vendedores c ON (c.id_codigo = a.id_vendedor) ";
                query2 += "LEFT JOIN inventario d ON (d.id_codigo = b.id_codigo) ";
                query2 += "LEFT JOIN clientes e ON (a.id_cliente =  e.id_codigo ) ";
                query2 += "WHERE a.status <> 'A' ";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.idcliente))
                {
                    query1 += " AND e.id_codigo = '" + doctos.idcliente + "' ";
                    query2 += " AND e.id_codigo = '" + doctos.idcliente + "' ";
                }

                if (!string.IsNullOrEmpty(doctos.cliente))
                {
                    query1 += " AND a.cliente = '%" + doctos.cliente + "%' ";
                    query2 += " AND a.cliente = '%" + doctos.cliente + "%' ";
                }

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        query1 += " AND fecha >= '" + fecha + "' ";
                        query2 += " AND fecha >= '" + fecha + "' ";
                    }
                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        query1 += " AND fecha <= '" + fecha + "' ";                      
                        query2 += " AND fecha <= '" + fecha + "' ";
                    }
                }

                if (doctos.facturas == "S" && doctos.envios == "N")
                {
                    query1 += "GROUP BY a.id_cliente, d.id_codigo ";

                    LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie = llenar.ListaEstadisticaVentaProdxClie(query1, DB, LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie);
                }
                else if (doctos.envios == "S" && doctos.facturas == "N")
                {
                    query2 += " GROUP BY a.id_cliente, d.id_codigo ";

                    LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie = llenar.ListaEstadisticaVentaProdxClie(query2, DB, LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie);

                }
                else if ((doctos.facturas == "S" && doctos.envios == "S") || doctos.consolidado == "S")
                {
                    query1 += " GROUP BY a.id_cliente,d.id_codigo ";
                    query2 += " GROUP BY a.id_cliente,d.id_codigo ";
                    queryunion = query1 + " UNION ALL " + query2;

                    LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie = llenar.ListaEstadisticaVentaProdxClie(queryunion, DB, LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie);
                }
            }

            string prueba = query1 + query2 + queryunion;

            return JsonConvert.SerializeObject(LlenarListaEstadisticaVentaProdxClie.listaLlenarListaEstadisticaVentaProdxClie);
        }
    }
}