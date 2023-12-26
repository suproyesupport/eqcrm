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
    public class SeguimientoTicketController : Controller
    {
        // GET: SeguimientoTicket
        public ActionResult SeguimientoTicket()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> usuarior = new List<SelectListItem>();
            string sentenciaSQLusuarior = "SELECT usuario, nombre FROM dlempresa.usuarios_web WHERE intercompany != 0";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLusuarior, DB, usuarior);
            ViewData["UsuarioResponsable"] = usuarior;
            stringConexionMySql.CerrarConexion();

            return View();
        }



        [HttpPost]
        public object GetTicketsSeguimiento (Models.FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string id_tipo = "";
                string DB = (string)this.Session["StringConexion"];
                string cInst = "";

                if (string.IsNullOrEmpty(cUserConected))
                {
                    return (ActionResult)this.RedirectToAction("Login", "Account");
                }

                ConexionMySQL conexionMySql = new ConexionMySQL();
                string sentenciaSQL = "SELECT id_tipo FROM usuarios_web WHERE usuario ='" + cUserConected.ToString() + "'";
                string str = conexionMySql.EjecutarLectura(sentenciaSQL, "dlempresa");
                if (conexionMySql.consulta.HasRows)
                {
                    while (conexionMySql.consulta.Read())
                    {
                        id_tipo = conexionMySql.consulta.GetString(0).ToString();
                    }
                }
                else
                {
                    conexionMySql.CerrarConexion();
                }
                conexionMySql.CerrarConexion();

                StringConexionMySQL llenar = new StringConexionMySQL();

                if (id_tipo == "1")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, UPPER(t.usuarioAsignado) AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.idEstado = 1 ";
                    cInst += "OR t.idEstado = 2 ";
                    cInst += "ORDER BY 1;";
                }
                else if (id_tipo == "2")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, UPPER(t.usuarioAsignado) AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.idEstado = 1 ";
                    cInst += "OR t.idEstado = 2 ";
                    cInst += "HAVING USUARIOASIGNADO = '" + cUserConected.ToString() + "' ";
                    cInst += "ORDER BY 1;";
                
                } else if (id_tipo == "3")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, UPPER(t.usuarioAsignado) AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.intercompany = 3 ";
                    cInst += "AND t.idTicket IN (SELECT a.idTicket FROM tickets.ticket a WHERE a.idEstado = 1 OR a.idEstado = 2 ) ";
                    cInst += "ORDER BY 1;";
                }
                //llenar.CerrarConexion();
                LlenarListaDocumentos.tickets = llenar.SegTickets(cInst, DB, LlenarListaDocumentos.tickets);
                

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.tickets);
        }

        

        [HttpPost]
        public string IniciarTicket(Ticket oticket)
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
                string cInst = "UPDATE tickets.ticket SET fecha_inicio = NOW(), ";
                cInst += "idEstado = 2 ";
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
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            iniciaticket.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string IngSeguimientoTicket(Ticket oticket)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL segticket = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO tickets.seguimiento_ticket (id_ticket, usuario, fecha, seguimiento) VALUES (";
                cInst += oticket.id + ", ";
                cInst += "'" + cUserConected + "', ";
                cInst += "CURTIME(), ";
                cInst += "'" + oticket.seguimiento + "');";

                lError = segticket.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    segticket.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            segticket.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string FinalizarTicket(Ticket oticket)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL finticket = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE tickets.ticket SET fecha_finalizacion = NOW(), ";
                cInst += "solucion = '" + oticket.solucion + "', ";
                cInst += "idEstado = 3, ";
                cInst += "usuarioCerro = '" + cUserConected + "' ";
                cInst += "WHERE idTicket = " + oticket.id + ";";
                lError = finticket.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    finticket.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            finticket.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string ReasignarTicket(Ticket oticket)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL reasignaticket = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE tickets.ticket SET ";
                cInst += "usuarioAsignado = '" + oticket.usuarior + "' ";
                cInst += "WHERE idTicket = " + oticket.id + ";";
                lError = reasignaticket.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    reasignaticket.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            reasignaticket.CerrarConexion();
            return str;
        }


        //GetSeguimientoTicket

        [HttpPost]
        public string GetSeguimientoTicket(Ticket oticket)
        {
            string str = "";

            bool lError;
            string cError = "";
            string cMensaje = "";

            StringConexionMySQL conexion = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];

            try
            {
                DataTable dt = new DataTable();
                string cInst = "SELECT fecha AS FECHA, seguimiento AS SEGUIMIENTO, usuario AS USUARIO FROM tickets.seguimiento_ticket WHERE id_ticket = " + oticket.id + ";";
                ViewBag.Tabla = conexion.LlenarTablaSeguimientoTicket(cInst, dt, DB);

                lError = conexion.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    conexion.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }


            conexion.CerrarConexion();
            return ViewBag.Tabla;
        }


    }
}