using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngProductosController : Controller
    {
        // GET: IngProductos
        public ActionResult IngProductos()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySQL = new StringConexionMySQL();
            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = "SELECT id_linea,descripcion FROM catlineasi";
            stringConexionMySQL.LLenarDropDownList(sentenciaSQL, DB, lineas);
            ViewData["Lineas"] = lineas;

            stringConexionMySQL.CerrarConexion();
            return (ActionResult)this.View();
        }
    }
}