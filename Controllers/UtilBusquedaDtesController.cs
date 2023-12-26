using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class UtilBusquedaDtesController : Controller
    {
        // GET: UtilBusquedaDtes
        public ActionResult UtilBusquedaDtes()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
           
            
           
            return (ActionResult)this.View();
        }
    }
}