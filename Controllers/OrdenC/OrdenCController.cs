using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers.OrdenC
{
    public class OrdenCController : Controller
    {
        // GET: OrdenC
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string cInst = "SELECT no_factura AS ID, fecha AS FECHA, statusordenes.nombre AS STATUS,requirente AS REQUIRENTE, (select nombre from catdeptosconta where id_depto = id_provee) AS DEPARTAMENTO,direccion AS EMAIL, obs AS OBS";
                   cInst += " FROM `ordcompras`";
                   cInst += "LEFT JOIN  statusordenes ON(statusordenes.id_status = ordcompras.status) WHERE ordcompras.status !='E' ";

            DataTable Tabla_para_Datos = new DataTable();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTml(cInst, Tabla_para_Datos, DB);
            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }
    }
}