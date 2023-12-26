using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace EqCrm.Controllers
{
    public class IngresoPreciosGASController : Controller
    {

        // GET: IngresoPreciosGAS
        public ActionResult IngresoPreciosGAS()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }



        [HttpPost]
        public string InsertarPrecioGAS(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO inventario ";
                cInst += "(id_codigo, codigoe, producto, costo1, costo2, precio1, precio2, unisalida, tipo) VALUES (";
                cInst += oInven.idcodigoeq + ", ";
                cInst += oInven.idcodigoeq + ", ";
                cInst += "'" + oInven.producto + "', ";
                cInst += oInven.costoeq1 + ", ";
                cInst += oInven.costoeq2 + ", ";
                cInst += oInven.precioeq1 + ", ";
                cInst += oInven.precioeq2 + ", ";
                cInst += oInven.idp + ", ";
                cInst += "'G');"; 

                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }



    }
}