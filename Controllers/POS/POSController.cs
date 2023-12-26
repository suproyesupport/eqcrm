using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EqCrm.Models.POS;

namespace EqCrm.Controllers
{
    public class POSController : Controller
    {
        // GET: POS_Caja
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }


            string cUserConected = (string)(Session["Usuario"]);
            string cEmpresa = (string)(Session["cNombreEmisor"]);            
            string year = "";
            string oDb = (string)(Session["StringConexion"]);

            string cInst = "SELECT CURDATE()";
            StringConexionMySQL consultar = new StringConexionMySQL();
            year = consultar.CurdateYear(cInst, oDb);

            ViewBag.Footer = "<span class=\"hidden-md-down fw-700\">" + year.Substring(0, 4).ToString() + " © EqSoftware by&nbsp;<a href = 'http://www.intec.com.gt/wp/' class='text-primary fw-500' title='gotbootstrap.com' target='_blank'>https://www.cgtecsa.com</a> &nbsp;&nbsp;" + cEmpresa.ToUpper() + "</span>";
            ViewBag.User = " USUARIO: " + cUserConected.ToUpper();
            ViewBag.Encabezado = cUserConected.ToUpper();
            ViewBag.Encabezado2 = cEmpresa.ToUpper();
            ViewData["User"] = "USUARIO CONECTADO:" + cUserConected.ToString().ToUpper();

            return (ActionResult)this.View();
        }

    }
}