using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class EqTasaWebController : Controller
    {
        // GET: EqTasaWeb
        public ActionResult EqTasaWeb()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public string PostDataTasa(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]);
            bool lResult = false;

            string sentenciaSQL1 = "UPDATE parametros SET nTasaWeb = " + datos.id;



            lResult = stringConexionMySql.EjecutarCommando(sentenciaSQL1, DB);

            if (lResult == false)
            {
                str = "{\"RESULTADO\": \"SE ACTUALIZO CORRECTAMENTE LA TASA\"}";
            }
            else
            {
                str = "{\"RESULTADO\": \"ERROR\"}";
            }

            stringConexionMySql.CerrarConexion();


            return str;
        }

        [HttpPost]
        public string PostDataDescto(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]);
            bool lResult = false;

            string sentenciaSQL1 = "UPDATE parametros SET nfdescto = " + datos.id;



            lResult = stringConexionMySql.EjecutarCommando(sentenciaSQL1, DB);

            if (lResult == false)
            {
                str = "{\"RESULTADO\": \"SE ACTUALIZO CORRECTAMENTE LA TASA MAXIMA DE DESCUENTO\"}";
            }
            else
            {
                str = "{\"RESULTADO\": \"ERROR\"}";
            }

            stringConexionMySql.CerrarConexion();


            return str;
        }
    }
}