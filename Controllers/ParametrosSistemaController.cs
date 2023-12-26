using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EqCrm.Controllers
{
    public class ParametrosSistemaController : Controller
    {
        // GET: ParametrosSistema
        public ActionResult ParametrosSistema()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            CargarPermiso();

            string permisofactsinexist = (string)this.Session["Permiso"];


            if (permisofactsinexist == "S")
            {
                ViewBag.permiso = "true";
            } else
            {
                ViewBag.permiso = "false";
            }

            return (ActionResult)this.View();
        }



        [HttpPost]
        public string ActivarPermiso(ParametrosSistema parsis)
        {
            string str = "";
            //string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];



            try
            {
                    if (parsis.chckfactsinexist == "S")
                    {
                        string cInstKar = "UPDATE parametros SET lfsinexist = 'S'";                       
                        lError = insertar.ExecCommand(cInstKar, DB, ref cError);
                        this.Session["Permiso"] = (object)str;

                    if (lError == true)
                        {
                            cMensaje = cError;
                            insertar.CerrarConexion();
                            str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                            return str;
                        }
                    } else if (parsis.chckfactsinexist == "N")
                {
                    string cInstKar = "UPDATE parametros SET lfsinexist = 'N'";
                    lError = insertar.ExecCommand(cInstKar, DB, ref cError);

                    if (lError == true)
                    {
                        cMensaje = cError;
                        insertar.CerrarConexion();
                        str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                        return str;
                    }
                }
                
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            CargarPermiso();
            return str;
        }



        [HttpPost]
        public void CargarPermiso()
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT lfsinexist FROM parametros";
                str = stringConexionMySql.Consulta(cInst, DB);
                this.Session["Permiso"] = (object)str;

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
        }



    }
}