using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngresoProblemaTicketController : Controller
    {
        // GET: IngresoProblemaTicket
        public ActionResult IngresoProblemaTicket ()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public string InsertarProblemaTicket(TicketProblema oticketproblema)
        {
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO tickets.categoria VALUES (";
                cInst += "NULL, ";
                cInst += "'" + oticketproblema.problema + "', ";
                cInst += "1);";

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
        public object GetProblemasTicket(Models.FiltroGenerico doctos)
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

                StringConexionMySQL llenar = new StringConexionMySQL();

                cInst = "SELECT IdCategoria AS ID, categoria AS PROBLEMA ";
                cInst += "FROM tickets.categoria ";
                cInst += "ORDER BY 1;";
                       
                llenar.KillAllMySQL(DB);

                LlenarListaDocumentos.listaTicketProblema = llenar.ListadoProblemaTicket(cInst, DB, LlenarListaDocumentos.listaTicketProblema);

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.listaTicketProblema);
        }





        [HttpPost]
        public string EliminarProblemaTicket(TicketProblema oticketproblema)
        {
            StringConexionMySQL eliminar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "DELETE FROM tickets.categoria WHERE ";
                cInst += "IdCategoria = " + oticketproblema.id + "; ";

                lError = eliminar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    eliminar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            eliminar.CerrarConexion();
            return str;
        }

    }
}