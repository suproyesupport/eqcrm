using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngresoTipoProblemaTicketController : Controller
    {
        // GET: IngresoTipoProblemaTicket
        public ActionResult IngresoTipoProblemaTicket()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> problema = new List<SelectListItem>();
            string sentenciaSQLproblema = "SELECT idCategoria, categoria FROM tickets.categoria";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLproblema, DB, problema);
            ViewData["Problema"] = problema;
            stringConexionMySql.CerrarConexion();

            return (ActionResult)this.View();
        }



        [HttpPost]
        public string InsertarTipoProblemaTicket(TicketTipoProblema oticketproblema)
        {
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO tickets.problemas VALUES (";
                cInst += "NULL, ";
                cInst += "'" + oticketproblema.tipoproblema + "', ";
                cInst += oticketproblema.problema + ", ";
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
        public object GetTipoProblemasTicket(Models.FiltroGenerico doctos)
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

                if (doctos.filtro == "1")
                {
                    cInst = "SELECT a.IdProblema AS ID, a.problema AS TIPOPROBLEMA, b.categoria AS PROBLEMA ";
                    cInst += "FROM tickets.problemas a ";
                    cInst += "INNER JOIN tickets.categoria b ON b.IdCategoria = a.IdCategoria ";
                    cInst += "WHERE a.IdCategoria = " + doctos.problema + " ";
                    cInst += "ORDER BY 1;";

                    llenar.KillAllMySQL(DB);

                    LlenarListaDocumentos.listaTipoTicketProblema = llenar.ListadoTipoProblemaTicket(cInst, DB, LlenarListaDocumentos.listaTipoTicketProblema);

                } else {

                    cInst = "SELECT a.IdProblema AS ID, a.problema AS TIPOPROBLEMA, b.categoria AS PROBLEMA ";
                    cInst += "FROM tickets.problemas a ";
                    cInst += "INNER JOIN tickets.categoria b ON b.IdCategoria = a.IdCategoria ";
                    cInst += "ORDER BY 1;";

                    llenar.KillAllMySQL(DB);

                    LlenarListaDocumentos.listaTipoTicketProblema = llenar.ListadoTipoProblemaTicket(cInst, DB, LlenarListaDocumentos.listaTipoTicketProblema);

                }


            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.listaTipoTicketProblema);
        }



        [HttpPost]
        public string EliminarTipoProblemaTicket(TicketTipoProblema oticketproblema)
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