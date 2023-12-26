using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class MaximosMinimosController : Controller
    {
        // GET: MaximosMinimos
        public ActionResult MaximosMinimos()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string cInst = "SELECT a.id_codigo AS ID ,a.codigoe as CODIGOE,concat (a.producto)  as PRODUCTO,a.linea as LINEA,if(a.id_marca = '','NoMarca',a.id_marca) as MARCA,a.id_medida As MEDIDA ,sum(ifnull(b.entrada - b.salida,0)) as EXISTENCIA,a.stockmin AS MINIMO,a.stockmax  AS MAXIMO ,a.stockmin-sum(ifnull(b.entrada - b.salida,0)) as PEDIDO";
                  cInst += " FROM inventario a";
                  cInst += " LEFT JOIN kardexinven b ON (b.id_codigo = a.id_codigo and b.id_agencia = 1)";
                  cInst += " WHERE a.id_codigo >= 1 GROUP BY a.id_codigo HAVING PEDIDO >=1 ";


            DataTable Tabla_para_Datos = new DataTable();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTml(cInst, Tabla_para_Datos, DB);
            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }
    }
}