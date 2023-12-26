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
using System.Globalization;

namespace EqCrm.Controllers
{
    public class TicketsFinalizadosController : Controller
    {
        // GET: TicketsFinalizados
        public ActionResult TicketsFinalizados()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }



        [HttpPost]
        public object GetTicketsFinalizados(Models.FiltroGenerico doctos)
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
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    //cInst += "AND t.idTicket IN (SELECT a.idTicket FROM tickets.ticket a WHERE a.idEstado = 3 ) ";
                    //cInst += "WHERE t.idEstado = 3 ";
                    cInst += "WHERE t.idTicket IN (SELECT a.idTicket FROM tickets.ticket a WHERE a.idEstado = 3 ) ";
                    cInst += "ORDER BY 1;";
                }
                else if (id_tipo == "2")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.idEstado = 3 ";
                    cInst += "AND t.usuarioAsignado = '" + cUserConected.ToString() + "'";
                    cInst += "ORDER BY 1;";

                }
                else if (id_tipo == "3")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.intercompany = 3 ";
                    cInst += "AND t.idTicket IN (SELECT a.idTicket FROM tickets.ticket a WHERE a.idEstado = 3 ) ";
                    cInst += "ORDER BY 1;";
                }

                llenar.KillAllMySQL(DB);

                LlenarListaDocumentos.tickets = llenar.SegTickets(cInst, DB, LlenarListaDocumentos.tickets);

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.tickets);
        }



        [HttpPost]
        public object GetFiltrarxFecha(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string id_tipo = "";

            if (string.IsNullOrEmpty(doctos.fecha1))
            {
                doctos.fecha1 = "";
            }

            if (string.IsNullOrEmpty(doctos.fecha2))
            {
                doctos.fecha2 = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {

                ConexionMySQL conexionMySql = new ConexionMySQL();
                string sentenciaSQL = "SELECT id_tipo FROM usuarios_web WHERE usuario ='" + cUserConected.ToString() + "'";
                string str = conexionMySql.EjecutarLectura(sentenciaSQL, "dlempresa");
                string cInst = "";

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

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;
                string queryfecha = "";

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryfecha += "AND fecha >= '" + fecha + "' ";
                    }
                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        queryfecha += " AND fecha <= '" + fecha + "'";
                    }
                }

                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                if (id_tipo == "1")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.idEstado = 3 ";
                    cInst += queryfecha + " ";
                    cInst += "ORDER BY 1;";
                }
                else if (id_tipo == "2")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.idEstado = 3 ";
                    cInst += "AND t.usuarioAsignado = '" + cUserConected.ToString() + "' ";
                    cInst += queryfecha + " ";
                    cInst += "ORDER BY 1;";

                }
                else if (id_tipo == "3")
                {
                    cInst = "SELECT t.IdTicket AS ID, DATE_FORMAT(t.fecha_finalizacion, '%m-%d-%Y') AS FECHAF,TIMESTAMPDIFF(DAY,t.fecha,curdate()) AS DIAS, t.usuarioAsignado AS USUARIOASIGNADO, ";
                    cInst += "c.categoria AS CATEGORIA, p.problema AS PROBLEMA, s.cliente AS CLIENTE, t.reporta AS REPORTA, e.Estado AS ESTADO,  t.descripcion AS DESCRIPCION, t.solucion AS SOLUCION, DATE_FORMAT(t.fecha, '%m-%d-%Y') AS FECHAA, DATE_FORMAT(t.fecha_inicio, '%m-%d-%Y') AS FECHAI ";
                    cInst += "FROM tickets.ticket t ";
                    cInst += "INNER JOIN tickets.problemas p ON t.IdProblema  = p.IdProblema ";
                    cInst += "INNER JOIN tickets.categoria c ON p.IdCategoria = c.IdCategoria ";
                    cInst += "INNER JOIN tickets.clientes s ON t.IdCliente = s.IdCliente ";
                    cInst += "INNER JOIN tickets.estado_ticket e ON t.IdEstado = e.IdEstado ";
                    cInst += "WHERE t.intercompany = 3 ";
                    cInst += "AND t.idTicket IN (SELECT a.idTicket FROM tickets.ticket a WHERE a.idEstado = 3 ) ";
                    cInst += queryfecha + " ";
                    cInst += "ORDER BY 1;";
                }

                LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas = llenar.ListaRpoGenFacturas(cInst, DB, LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
            }
            return JsonConvert.SerializeObject(LlenarListaRorteGeneralFacturas.listaReporteGeneralFacturas);
        }



    }
}