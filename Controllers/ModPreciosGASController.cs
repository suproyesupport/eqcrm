using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ModPreciosGASController : Controller
    {


        // GET: ModPreciosGAS
        public ActionResult ModPreciosGAS()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }



        [HttpPost]
        public object GetPreciosGAS(Models.FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string DB = (string)this.Session["StringConexion"];
                string cInst = "";

                if (string.IsNullOrEmpty(cUserConected))
                {
                    return (ActionResult)this.RedirectToAction("Login", "Account");
                }

                StringConexionMySQL llenar = new StringConexionMySQL();

                cInst = "SELECT a.id_codigo AS ID, a.producto AS PRODUCTO, a.unisalida AS IDP, a.costo1 AS COSTO1, a.precio1 AS PRECIO1, a.costo2 AS COSTO2, a.precio2 AS PRECIO2 ";
                cInst += "FROM inventario a ";
                cInst += "WHERE a.tipo = 'G' ";
                cInst += "ORDER BY 1;";

                LlenarListaDocumentos.listaPreciosGAS = llenar.ListadoPreciosGAS(cInst, DB, LlenarListaDocumentos.listaPreciosGAS);


            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.listaPreciosGAS);
        }



        [HttpPost]
        public string ModificarPreciosGAS(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL modificar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE inventario SET ";
                cInst += "producto = " + "'" + oInven.producto + "', ";
                cInst += "costo1 = " + oInven.costoeq1 + ", ";
                cInst += "precio1 = " + oInven.precioeq1 + ", ";
                cInst += "costo2 = " + oInven.costoeq2 + ", ";
                cInst += "precio2 = " + oInven.precioeq2 + ", ";
                cInst += "unisalida = " + oInven.idp + " ";
                cInst += "WHERE id_codigo = " + oInven.idcodigoeq + ";";

                lError = modificar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    modificar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            modificar.CerrarConexion();
            return str;
        }



    }
}