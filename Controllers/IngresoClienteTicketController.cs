using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngresoClienteTicketController : Controller
    {
        // GET: IngresoClienteTicket
        public ActionResult IngresoClienteTicket()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            string inter = (string)(Session["Intercompany"]);

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            List<SelectListItem> intercompany = new List<SelectListItem>();
            string sentenciaSQLintercompany = "SELECT id_empresa, name FROM tickets.intercompany";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLintercompany, DB, intercompany);
            ViewData["Intercompany"] = intercompany;
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> intercompany2 = new List<SelectListItem>();
            string sentenciaSQLintercompany2 = "SELECT id_empresa, name FROM tickets.intercompany";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLintercompany2, DB, intercompany2);
            ViewData["Intercompany2"] = intercompany2;
            stringConexionMySql.CerrarConexion();

            return (ActionResult)this.View();
        }



        [HttpPost]
        public object GetClienteTicket(Models.FiltroGenerico doctos)
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

                cInst = "SELECT a.IdCliente AS ID, a.cliente AS CLIENTE, a.nit AS NIT, b.name AS INTERCOMPANY ";
                cInst += "FROM tickets.clientes a ";
                cInst += "INNER JOIN tickets.intercompany b ON a.idIntercompany = b.id_empresa ";
                cInst += "ORDER BY 1;";

                llenar.KillAllMySQL(DB);

                LlenarListaDocumentos.listaClienteTicket = llenar.ListadoClienteTicket(cInst, DB, LlenarListaDocumentos.listaClienteTicket);

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.listaClienteTicket);
        }



        [HttpPost]
        public string InsertarClienteTicket(Ticket oticket)
        {
            string DB = (string)this.Session["StringConexion"];
            string str = "";
            bool lError;
            string cError = "";
            string cMensaje = "";

            StringConexionMySQL insertar = new StringConexionMySQL();

            try
            {
                string cInst = "INSERT INTO tickets.clientes VALUES (";
                cInst += "NULL, ";
                cInst += "'" + oticket.cliente + "', ";
                cInst += "'" + oticket.nit + "', ";
                cInst += "1, ";
                cInst += oticket.intercompany + ");";

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
        public string ModificarClienteTicket(Ticket oticket)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL iniciaticket = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                if (oticket.intercompany2 != "")
                {
                    string cInst = "UPDATE tickets.clientes SET ";
                    cInst += "cliente = '" + oticket.clientee + "' ";
                    cInst += "nit = '" + oticket.nitt + "', ";
                    cInst += "IdEstado = 1, ";
                    cInst += "idIntercompany = " + oticket.intercompany2 + ", ";
                    cInst += "WHERE idTicket = " + oticket.id + ";";

                    lError = iniciaticket.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {
                        cMensaje = cError;
                        iniciaticket.CerrarConexion();
                        str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                        return str;
                    }

                } else
                {
                    string cInst = "UPDATE tickets.clientes SET ";
                    cInst += "cliente = '" + oticket.clientee + "' ";
                    cInst += "nit = '" + oticket.nitt + "', ";
                    cInst += "IdEstado = 1  0 ";
                    cInst += "WHERE idTicket = " + oticket.id + ";";
                    lError = iniciaticket.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {
                        cMensaje = cError;
                        iniciaticket.CerrarConexion();
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

            iniciaticket.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string EliminarClienteTicket(Ticket oticket)
        {
            StringConexionMySQL eliminar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "DELETE FROM tickets.clientes WHERE ";
                cInst += "idCliente = " + oticket.id + "; ";

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