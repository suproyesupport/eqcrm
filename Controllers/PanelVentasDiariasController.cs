using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class PanelVentasDiariasController : Controller
    {
        // GET: PanelVentasDiarias
        public ActionResult PanelVentasDiarias()
        {
            string cUserConected = (string)(Session["Usuario"]);
            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpPost]
        public string ConsultaVentasDiarias()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";




            //instruccionSQL = " SELECT a.fecha AS FECHA,FORMAT(SUM(b.subtotal/1.12),2) AS VENTAS,FORMAT(SUM(b.costo*b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal/1.12 - b.costo*b.cantidad),2) AS MARGEN ,SUM(b.costo*b.cantidad)/sum(b.subtotal) AS PORCENTAJE";
            instruccionSQL = " SELECT a.fecha AS FECHA,FORMAT(SUM(b.subtotal/1.12),2) AS VENTAS,FORMAT(SUM(b.costo*b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal/1.12 - b.costo*b.cantidad),2) AS MARGEN ";
            instruccionSQL += " FROM facturas a";
            instruccionSQL += " LEFT JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
            instruccionSQL += " INNER JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
            instruccionSQL += " INNER JOIN inventario d ON(b.id_codigo = d.id_codigo)";
            //instruccionSQL += " WHERE a.fecha Between '" + aGets[1] + "'"
            //instruccionSQL += " AND '" + aGets[2] + "'"
            instruccionSQL += " WHERE a.status !='A' ";
            instruccionSQL += " GROUP BY a.fecha ";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlFreeze(instruccionSQL, dt, Base_Datos).ToString().Replace("00:00:00","");

            lectura.CerrarConexion();

            return cResultado;




        }

        [HttpPost]
        public string ConsultaVentasDiariasCF(string cFecha1, string cFecha2)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";




            //instruccionSQL = " SELECT a.fecha AS FECHA,FORMAT(SUM(b.subtotal/1.12),2) AS VENTAS,FORMAT(SUM(b.costo*b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal/1.12 - b.costo*b.cantidad),2) AS MARGEN ,SUM(b.costo*b.cantidad)/sum(b.subtotal) AS PORCENTAJE";
            instruccionSQL = " SELECT a.fecha AS FECHA,FORMAT(SUM(b.subtotal/1.12),2) AS VENTAS,FORMAT(SUM(b.costo*b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal/1.12 - b.costo*b.cantidad),2) AS MARGEN ";
            instruccionSQL += " FROM facturas a";
            instruccionSQL += " LEFT JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
            instruccionSQL += " INNER JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
            instruccionSQL += " INNER JOIN inventario d ON(b.id_codigo = d.id_codigo)";
            instruccionSQL += " WHERE a.fecha Between '" + cFecha1 + "'";
            instruccionSQL += " AND '" + cFecha2 + "'";
            instruccionSQL += " AND a.status !='A' ";
            instruccionSQL += " GROUP BY a.fecha ";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlFreeze(instruccionSQL, dt, Base_Datos).ToString().Replace("00:00:00", "");

            lectura.CerrarConexion();

            return cResultado;




        }
    }
}