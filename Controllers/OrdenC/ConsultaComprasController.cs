using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.OrdenC
{
    public class ConsultaComprasController : Controller
    {
        // GET: ConsultaCompras
        public ActionResult ConsultaCompras()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GetCompras(FiltroGenerico doctos)
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

                string queryCotizaciones = "Select p.NO_FACTURA, p.SERIE, p.FECHA, p.PROVEEDOR, p.NIT,  p.DIRECCION, p.TOTAL, p.OBS  FROM compras as P ";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

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

                LLenarListaCotizaciones.listaCompras = llenar.ListaCompras(queryCotizaciones, DB, LLenarListaCotizaciones.listaCompras);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LLenarListaCotizaciones.listaCompras);
        }

        [Route("/{id}")]
        public ActionResult VerCompra(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string[] strIdInterno = id.Split('|');

            string no_factura = strIdInterno[0].ToString();
            string serie = strIdInterno[1].ToString();


            ViewData["ORDEN"] = no_factura.ToString();
            ViewData["FIRMA"] = serie.ToString();

            string cInst = "SELECT fecha,nit,obs,proveedor,direccion,status FROM compras ";
            cInst += "WHERE no_factura =" + no_factura.ToString();
            cInst += " AND serie ='" + serie.ToString() + "'";


            stringConexionMySql.EjecutarLectura(cInst, DB);
            if (stringConexionMySql.consulta.HasRows)
            {
                while (stringConexionMySql.consulta.Read())
                {
                    if (stringConexionMySql.consulta.GetString(5) == "A")
                    {
                        ViewData["AUTORIZADA"] = "ANULADA";
                    }
                    else
                    {
                        ViewData["AUTORIZADA"] = "";
                    }
                    ViewData["DEPTO"] = stringConexionMySql.consulta.GetString(3);
                    ViewData["FECHA"] = stringConexionMySql.consulta.GetString(0);
                    ViewData["REQUIRENTE"] = stringConexionMySql.consulta.GetString(1);
                    ViewData["USO"] = stringConexionMySql.consulta.GetString(2);
                    ViewData["CORREO"] = stringConexionMySql.consulta.GetString(4);

                }


            }

            DataTable dt = new DataTable();

            cInst = "SELECT a.no_cor as ITEM,a.id_codigo as CODIGO,(SELECT b.codigoe FROM inventario b WHERE b.id_codigo = a.id_codigo) AS CODIGOE, obs AS PRODUCTO, cantidad AS CANTIDAD FROM detcompras a ";
            cInst += "WHERE no_factura =" + no_factura.ToString();
            cInst += " AND serie ='" + serie.ToString() + "'";

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlN(cInst, dt, DB);


            stringConexionMySql.CerrarConexion();




            return (ActionResult)this.View();


        }
    }
}