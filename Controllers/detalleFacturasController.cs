using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class detalleFacturasController : Controller
    {
        // GET: detalleFacturas
        public ActionResult detalleFacturas()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public string detFacturas(linea clinea)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            instruccionSQL = " SELECT \"FACTURA\", a.no_factura AS ID_INTERNO,a.serie AS SERIE,a.fecha AS FECHA,a.firmaelectronica AS UUID,a.cliente AS CLIENTE, IFNULL((SELECT nombre FROM coordinador WHERE coordinador.id_codigo = a.nrc), '') AS TECNICO, c.nombre AS VENDEDOR,d.id_codigo AS ID, d.codigoe AS CODIGOE, d.producto AS PRODUCTO, b.cantidad AS CANTIDAD,(b.cantidad * b.precio) as PRECIO, b.tdescto AS DESCTO, b.SUBTOTAL as TOTAL, a.id_agencia AS AGENCIA,0 AS NOFACT,'' AS SERIEFAC";
            instruccionSQL += " FROM facturas a";
            instruccionSQL += " LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
            instruccionSQL += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
            instruccionSQL += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
            instruccionSQL += " WHERE a.fecha Between '" + clinea.fecha1 + "' AND '" + clinea.fecha2 + "' AND a.status != 'A'";
          //  instruccionSQL += " ORDER BY a.no_factura, a.serie,a.id_agencia ";
            instruccionSQL += " UNION";
            instruccionSQL += " SELECT \"NCREDITO\",a.no_factura AS ID_INTERNO,a.serie AS SERIE,a.fecha AS FECHA,a.firmaelectronica AS UUID,a.cliente AS CLIENTE, '' AS TECNICO, c.nombre AS VENDEDOR,b.id_codigo AS ID, d.codigoe AS CODIGOE, d.producto AS PRODUCTO, b.cantidad AS CANTIDAD,(b.cantidad * b.precio) as PRECIO, b.tdescto AS DESCTO, b.SUBTOTAL as TOTAL, a.id_agencia AS AGENCIA,a.no_nota AS NOFACT,a.serie_nota AS SERIEFAC";
            instruccionSQL += " FROM notadecredito a";
            instruccionSQL += " LEFT JOIN detnotadecredito b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
            instruccionSQL += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
            instruccionSQL += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
            instruccionSQL += " WHERE a.fecha Between '" + clinea.fecha1 + "' AND '" + clinea.fecha2 + "' AND a.status != 'A'";
           // instruccionSQL += " ORDER BY a.no_factura, a.serie,a.id_agencia";

                                                    

            cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;




        }
       
        
    }
}