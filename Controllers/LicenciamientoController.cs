using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExtreme.AspNet.Mvc;
using DevExpress.Data;
using DevExtreme.AspNet.Data;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using EqCrm.Models;

namespace EqCrm.Controllers
{
    public class LicenciamientoController : Controller
    {
        // GET: Licenciamiento
        public ActionResult Licenciamiento()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> intercompany = new List<SelectListItem>();
            string sentenciaSQLinter = "SELECT id_empresa, name FROM licenciamiento.intercompany";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLinter, DB, intercompany);
            ViewData["Intercompany"] = intercompany;

            List<SelectListItem> intercompanym = new List<SelectListItem>();
            string sentenciaSQLinterm = "SELECT id_empresa, name FROM licenciamiento.intercompany";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLinterm, DB, intercompanym);
            ViewData["Intercompanym"] = intercompanym;

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }

        [HttpPost]
        public string GetDataCliente(Licenciamiento oLicencias)
        {
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            StringConexionMySQL stringConexionMySql2 = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]); //(string)this.Session["StringConexion"];

            string sentenciaSQL1 = "SELECT json_object('CLIENTE', cliente," + "'DIRECCION', direccion," + "'NIT', nit," + "'DIASCRED',diascred, " + "'CORREO', email) FROM clientes " + " WHERE id_codigo = " + oLicencias.nit;

            string sentenciaSQL2 = "SELECT json_object('CLIENTE', cliente, 'DIRECCION', direccion, 'NIT', nit, 'DIASCRED',diascred, " + "'CORREO', email) FROM clientes WHERE nit = '" + oLicencias.nit + "'";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);

            if (stringConexionMySql.consulta.Read())
            {
                str = stringConexionMySql.consulta.GetString(0).ToString();
            }
            else if (!stringConexionMySql.consulta.Read())
            {
                stringConexionMySql.EjecutarLectura(sentenciaSQL2, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                }
                else
                {
                    Funciones f = new Funciones();
                    str = f.GetDataNit(oLicencias.nit);
                }
            }
            else
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }
            stringConexionMySql.CerrarConexion();
            return str;
        }

        [HttpPost]
        public string InsertarLicencia(Licenciamiento oLicencias)
        {
            Guid g = Guid.NewGuid();
            string auto = g.ToString().ToUpper();

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
                string cInst = "INSERT INTO licenciamiento.empresa (nombre_empresa, nombre_comercial, nit, autorizacion, email, actividad, fechavencimiento, fel) VALUES (";
                cInst += "'" + oLicencias.nombre + "', ";
                cInst += "'" + oLicencias.nombrec + "', ";
                cInst += "'" + oLicencias.nit + "', ";
                cInst += "'" + auto + "', ";
                cInst += "'" + oLicencias.email + "', ";
                cInst += "'" + oLicencias.intercompany + "', ";
                cInst += "'" + oLicencias.fechavencimiento + "', ";
                cInst += "'" + oLicencias.fel + "'); ";

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




        [HttpPost]
        public object GetLicencias(Licenciamiento oLicencias)
        {
             string cUserConected = (string)(Session["Usuario"]);
             string IntercompanyConnected = (string)(Session["Intercompany"]);

            if (string.IsNullOrEmpty(oLicencias.nitb))
            {
                oLicencias.nitb = "";
            }

            if (string.IsNullOrEmpty(oLicencias.nombrecb))
            {
                oLicencias.nombrecb = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string oDb = (string)(Session["StringConexion"]);
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                //string cQueryInterC = "SELECT name FROM licenciamiento.intercompany WHERE id_empresa = " + IntercompanyConnected;
                //string intercompany = llenar._EjecutarLectura(cQueryInterC, oDb);


                string cInst = "SELECT CONCAT(id_empresa,' ','') AS DATOS, id_empresa as CODIGO, nombre_empresa AS EMPRESA, nombre_comercial AS COMERCIAL, nit AS NIT, autorizacion AS AUTORIZACION, fechavencimiento AS VENCIMIENTO, actividad AS INTERCOMPANY ";
                cInst += "FROM licenciamiento.empresa ";
                cInst += "WHERE id_empresa != '' ";

                if (IntercompanyConnected.ToString() == "3")
                {
                    cInst += "AND actividad = 'INTEC' ";
                }


                if (oLicencias.nitb.ToString() != "")
                {
                    cInst += " AND nit  = " + oLicencias.nitb.ToString();
                }

                if (oLicencias.nombrecb.ToString() != "")
                {
                    cInst += " AND nombre_comercial = '" + oLicencias.nombrecb.ToString() + "'";
                }

                LlenarListaLicencias.lista = llenar.ListaLincenciasEmpresa(cInst, DB, LlenarListaLicencias.lista);
            }

            return JsonConvert.SerializeObject(LlenarListaLicencias.lista);
        }



        [HttpPost]
        public string GetDataLicencias(Licenciamiento oLicencias)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            if (string.IsNullOrEmpty(oLicencias.id_empresa))
            {
                oLicencias.id_empresa = "";
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                return str;
            }

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT JSON_OBJECT('CODIGO', id_empresa," + "'EMPRESA', nombre_empresa," + "'COMERCIAL', nombre_comercial," + "'NIT', nit," + "'AUTORIZACION', autorizacion," + "'VENCIMIENTO', fechavencimiento, " + "'INTERCOMPANY', actividad) ";
                cInst += "FROM licenciamiento.empresa ";
                cInst += "WHERE id_empresa != '' ";

                if (oLicencias.id_empresa.ToString() != "")
                {
                    cInst += "AND id_empresa  = " + oLicencias.id_empresa.ToString();
                }

                llenar.EjecutarLectura(cInst, DB);
                if (llenar.consulta.Read())
                {
                    str = llenar.consulta.GetString(0).ToString();
                }
                else
                {
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            llenar.CerrarConexion();
            return str;
        }





        [HttpPost]
        public string ModificarLicencia(Licenciamiento oLicencias)
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
                string cInst = "UPDATE licenciamiento.empresa SET ";
                cInst += "nombre_empresa = " + "'" + oLicencias.nombre + "', ";
                cInst += "nombre_comercial = " + "'" + oLicencias.nombrec + "', ";
                cInst += "fechavencimiento = " + "'" + oLicencias.fechavencimiento + "', ";
                cInst += "fel = " + "'" + oLicencias.fel + "' ";
                cInst += "WHERE nit = '" + oLicencias.nit+ "';";

                //cInst += "WHERE id_codigo = " + "'" + oInven.idcodigoeq + "'" + ";";

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