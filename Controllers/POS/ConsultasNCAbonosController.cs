using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.POS
{
    public class ConsultasNCAbonosController : Controller
    {
        // GET: ConsultasNCAbonos
        public ActionResult ConsultasNCAbonos()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }



    }

}