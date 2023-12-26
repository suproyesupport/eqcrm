using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class CerrarSesionController : Controller
    {
        // GET: CerrarSesion
        public ActionResult CerrarSesion()
        {
            Session["Usuario"] = "";
            Session["StringConexion"] = "";

            return View("~/Views/Account/Login.cshtml");
         
        }
    }
}