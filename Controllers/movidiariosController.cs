using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class movidiariosController : Controller
    {
        // GET: movidiarios
        public ActionResult movidiarios()
        {
            string cUserConected = (string)(Session["Usuario"]);
            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public string movdiarios()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            // BUSCA MOVIMIENTOS DE VENTA
            instruccionSQL = "SELECT CODIGO,PRODUCTO,UMEDIDA,SUM(VENTAS) AS VENTAS,SUM(COMPRAS) AS COMPRAS, SUM(ENTRADAS) AS ENTRADAS, SUM(SALIDAS) AS SALIDAS FROM( SELECT a.fecha AS FECHA, b.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, sum(b.cantidad) as VENTAS,0 AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS";
            instruccionSQL += " FROM facturas a ";
            instruccionSQL += " JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia) ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = b.id_codigo) ";
            //instruccionSQL += " WHERE a.fecha Between '" + aGets[1] + "'"
            //instruccionSQL += " AND '" + aGets[2] + "'"
            instruccionSQL += " WHERE a.status !='A' ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            // BUSCAR MOVIMIENTOS DE COMPRAS
            instruccionSQL += " UNION ALL ";

            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada-salida) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE a.fecha <= '2022-12-31'";
            instruccionSQL += " GROUP BY d.id_codigo ";

            instruccionSQL += " UNION ALL ";


            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            ///instruccionSQL += " WHERE a.fecha Between '" + aGets[1] + "'"
            //instruccionSQL += " AND '" + aGets[2] + "'"
            instruccionSQL += " AND a.id_movi in (2) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            instruccionSQL += " UNION ALL ";

            // Buscando Ajustes
            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS ,0 AS COMPRAS,a.entrada AS ENTRADAS,a.salida AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            //instruccionSQL += " WHERE a.fecha Between '" + aGets[1] + "'"
            //instruccionSQL += " AND '" + aGets[2] + "'"
            instruccionSQL += " AND a.id_movi in (3,8,57,56) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ) X GROUP BY CODIGO, UMEDIDA";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString().Replace("00:00:00", "");

            lectura.CerrarConexion();

            return cResultado;




        }

        [HttpPost]
        public string filtromovdiarios(string cFecha1, string cFecha2)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            // BUSCA MOVIMIENTOS DE VENTA
            instruccionSQL = "SELECT CODIGO,PRODUCTO,UMEDIDA,SUM(VENTAS) AS VENTAS,SUM(COMPRAS) AS COMPRAS, SUM(ENTRADAS) AS ENTRADAS, SUM(SALIDAS) AS SALIDAS FROM( SELECT a.fecha AS FECHA, b.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, sum(b.cantidad) as VENTAS,0 AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS";
            instruccionSQL += " FROM facturas a ";
            instruccionSQL += " JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia) ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = b.id_codigo) ";
            instruccionSQL += " WHERE a.fecha Between '" + cFecha1 + "'";
            instruccionSQL += " AND '" + cFecha2 + "'";
            instruccionSQL += " AND a.status !='A' ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            // BUSCAR MOVIMIENTOS DE COMPRAS
            instruccionSQL += " UNION ALL ";

            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada-salida) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE a.fecha <= '2022-12-31'";
            instruccionSQL += " GROUP BY d.id_codigo ";

            instruccionSQL += " UNION ALL ";


            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE a.fecha Between '" + cFecha1 + "'";
            instruccionSQL += " AND '" + cFecha2 + "'";
            instruccionSQL += " AND a.id_movi in (2) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            instruccionSQL += " UNION ALL ";

            // Buscando Ajustes
            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS ,0 AS COMPRAS,a.entrada AS ENTRADAS,a.salida AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE a.fecha Between '" + cFecha1 + "'";
            instruccionSQL += " AND '" + cFecha2 + "'";
            instruccionSQL += " AND a.id_movi in (3,8,57,56) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ) X GROUP BY CODIGO, UMEDIDA";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString().Replace("00:00:00", "");

            lectura.CerrarConexion();

            return cResultado;




        }

        //[HttpPost]
        [Route("/{id}")]
        public ActionResult filtromovdiarioscod(string id)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            // BUSCA MOVIMIENTOS DE VENTA
            instruccionSQL = "SELECT FECHA,CODIGO,PRODUCTO,UMEDIDA,SUM(VENTAS) AS VENTAS,SUM(COMPRAS) AS COMPRAS, SUM(ENTRADAS) AS ENTRADAS, SUM(SALIDAS) AS SALIDAS FROM( SELECT a.fecha AS FECHA, b.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, sum(b.cantidad) as VENTAS,0 AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS";
            instruccionSQL += " FROM facturas a ";
            instruccionSQL += " JOIN detfacturas b ON (b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia) ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = b.id_codigo) ";
            instruccionSQL += " WHERE  d.id_codigo = " + id;
            instruccionSQL += " AND a.status !='A' ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            // BUSCAR MOVIMIENTOS DE COMPRAS
            instruccionSQL += " UNION ALL ";

            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada-salida) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE a.fecha <= '2022-12-31' AND d.id_codigo = " + id;
            instruccionSQL += " GROUP BY d.id_codigo ";

            instruccionSQL += " UNION ALL ";


            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS,sum(a.entrada) AS COMPRAS,0 AS ENTRADAS,0 AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE  d.id_codigo = " + id;
            instruccionSQL += " AND a.id_movi in (2) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ";

            instruccionSQL += " UNION ALL ";

            // Buscando Ajustes
            instruccionSQL += " SELECT a.fecha AS FECHA, a.id_codigo AS CODIGO, d.producto AS PRODUCTO, d.umedida AS UMEDIDA, 0 AS VENTAS ,0 AS COMPRAS,a.entrada AS ENTRADAS,a.salida AS SALIDAS ";
            instruccionSQL += " FROM kardexinven a ";
            instruccionSQL += " JOIN inventario  d ON (d.id_codigo = a.id_codigo) ";
            instruccionSQL += " WHERE  d.id_codigo = " + id;
            instruccionSQL += " AND a.id_movi in (3,8,57,56) ";
            instruccionSQL += " GROUP BY d.id_codigo,a.fecha ) X GROUP BY CODIGO, UMEDIDA,FECHA";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString().Replace("00:00:00", "");

            lectura.CerrarConexion();

            ViewBag.Tabla1 = cResultado;



            return View();




        }
    }
}