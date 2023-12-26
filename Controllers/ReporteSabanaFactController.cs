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
    public class ReporteSabanaFactController : Controller
    {
        // GET: ReporteSabanaFact
        public ActionResult ReporteSabanaFact()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }


        [HttpPost]
        public string GenerarReporteSabanaFact(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string oBase = (string)(Session["oBase"]);
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

                //string queryFacturas = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, a.fecha AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS FROM facturas a";
                //queryFacturas += " LEFT JOIN vendedores b on (a.id_vendedor = b.id_codigo)";
                //queryFacturas += "WHERE a.status != 'B'";
                
                string queryFacturas = "SELECT * FROM ( " +
                    "SELECT facturas.fecha AS FECHA, facturas.no_factura AS NO_FACTURA, facturas.serie AS SERIE, facturas.id_vendedor AS VENDEDOR, detfacturas.id_codigo AS CODIGO, " +
                    "inventario.codigoe AS CODIGOE,detfacturas.obs AS PRODUCTO,inventario.linea AS LINEA,detfacturas.precio AS PRECIO,detfacturas.subtotal AS SUBTOTAL,inventario.costo1 AS UCOSTO, " +
                    "inventario.costo1 AS COSTOP,detfacturas.cantidad AS CANTIDAD,id_cliente AS CODCLIE,clientes.cliente AS CLIENTE,facturas.nit AS NIT,facturas.telefono AS TELEFONO,facturas.fax AS FAX,facturas.email AS EMAIL, " +
                    "ifnull(clientes.departamento,'') AS DEPTO,ifnull(clientes.municipio,'') AS MUNI " +
                    "FROM facturas " +
                    "INNER JOIN detfacturas ON ( detfacturas.no_factura = facturas.no_factura AND detfacturas.serie = facturas.serie   )  " +
                    "INNER JOIN inventario ON ( detfacturas.id_codigo = inventario.id_codigo   ) " +
                    "LEFT JOIN clientes ON ( clientes.id_codigo = facturas.id_cliente ) " +
                    "WHERE  facturas.status != 'A'  " +
                    "UNION ALL " +
                    "SELECT envios.fecha AS FECHA, envios.no_factura AS NO_FACTURA, envios.serie AS SERIE, envios.id_vendedor AS VENDEDOR, detalle_envios.id_codigo AS CODIGO, " +
                    "inventario.codigoe AS CODIGOE,detalle_envios.obs AS PRODUCTO,inventario.linea AS LINEA,detalle_envios.precio AS PRECIO,detalle_envios.subtotal AS SUBTOTAL,inventario.costo1 AS UCOSTO, " +
                    "inventario.costo1 AS COSTOP,detalle_envios.cantidad AS CANTIDAD,id_cliente AS CODCLIE,clientes.cliente AS CLIENTE,envios.nit AS NIT,envios.telefono AS TELEFONO,envios.fax AS FAX,envios.email AS EMAIL, " +
                    "ifnull(clientes.departamento,'') AS DEPTO,ifnull(clientes.municipio,'') AS MUNI " +
                    "FROM envios " +
                    "INNER JOIN detalle_envios ON ( detalle_envios.no_factura = envios.no_factura AND detalle_envios.serie = envios.serie   )  " +
                    "INNER JOIN inventario ON ( detalle_envios.id_codigo = inventario.id_codigo   ) " +
                    "LEFT JOIN clientes ON ( clientes.id_codigo = envios.id_cliente ) " +
                    "WHERE envios.status != 'A' " +
                    "ORDER BY no_factura,serie,fecha) X ";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryFacturas += " WHERE X.FECHA >= '" + fecha + "'";
                    }

                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        queryFacturas += " AND X.FECHA <= '" + fecha + "'";
                    }
                }



            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = queryFacturas,
                BaseDatos = oBase

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            string cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultado = new Resultado();
            resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultado.resultado.ToString() == "true")
            {
                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
            }

            string jeje = cRespuesta.ToString();
            
            return cRespuesta;
        }

    }
}