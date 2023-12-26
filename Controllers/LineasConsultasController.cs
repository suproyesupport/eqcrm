using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.UI;

namespace EqCrm.Controllers
{
    public class LineasConsultasController : Controller
    {
  

        // GET: LineasConsultas
        public ActionResult LineasConsultas()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public string ConsultaLinea(linea clinea)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";

            if (clinea.lEnvio.ToString() == "Si")
            {
                instruccionSQL = " SELECT * FROM(  SELECT d.linea AS LINEA,ifnull(e.descripcion,'LINEA NO REGISTRADA') AS DESCRIPCION,";
                instruccionSQL += " FORMAT(SUM(b.subtotal),2) AS TOTAL,FORMAT(SUM(d.costo2 * b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal - d.costo2 * b.cantidad),2) AS MARGEN";
                instruccionSQL += "  FROM facturas a";
                instruccionSQL += " LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                instruccionSQL += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
                instruccionSQL += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
                instruccionSQL += " LEFT JOIN catlineasi e ON(d.linea = e.id_linea)";
                instruccionSQL += " WHERE a.fecha Between '" + clinea.fecha1 + "' AND '" + clinea.fecha2 + "' AND a.status != 'A'";
                instruccionSQL += " GROUP BY d.linea";
                instruccionSQL += " UNION";
                instruccionSQL += " SELECT d.linea AS LINEA,ifnull(e.descripcion, 'LINEA NO REGISTRADA') AS DESCRIPCION,FORMAT(SUM(b.subtotal),2) AS TOTAL,FORMAT(SUM(d.costo2 * b.cantidad),2),FORMAT(SUM(b.subtotal - d.costo2 * b.cantidad),2) AS MARGEN";
                instruccionSQL += " FROM envios a";
                instruccionSQL += " LEFT JOIN detalle_envios b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                instruccionSQL += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
                instruccionSQL += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
                instruccionSQL += " LEFT JOIN catlineasi e ON(d.linea = e.id_linea)";
                instruccionSQL += " WHERE  a.fecha Between '" + clinea.fecha1 + "' AND '" + clinea.fecha2 + "' AND a.status != 'A'";
                instruccionSQL += "  GROUP BY d.linea ) X GROUP BY x.linea";
            }
            else
            {
                instruccionSQL = "SELECT d.linea AS LINEA,ifnull(e.descripcion,'LINEA NO REGISTRADA') AS DESCRIPCION,";
                instruccionSQL += " FORMAT(SUM(b.subtotal),2) AS TOTAL,FORMAT(SUM(d.costo2 * b.cantidad),2) AS COSTO,FORMAT(SUM(b.subtotal - d.costo2 * b.cantidad),2) AS MARGEN";
                instruccionSQL += "  FROM facturas a";
                instruccionSQL += " LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                instruccionSQL += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
                instruccionSQL += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
                instruccionSQL += " LEFT JOIN catlineasi e ON(d.linea = e.id_linea)";
                instruccionSQL += " WHERE a.fecha Between '" + clinea.fecha1 + "' AND '" + clinea.fecha2 + "' AND a.status != 'A'";
                instruccionSQL += " GROUP BY d.linea";
                
            }
            
             cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            
             lectura.CerrarConexion();

             return cResultado;
            

          

        }

        
    }
}