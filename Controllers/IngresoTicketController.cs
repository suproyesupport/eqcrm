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
    public class IngresoTicketController : Controller
    {
        // GET: IngresoTicket
        public ActionResult IngresoTicket()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            string inter = (string)(Session["Intercompany"]);

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> usuarior = new List<SelectListItem>();
            string sentenciaSQLusuarior = "SELECT usuario, nombre FROM dlempresa.usuarios_web WHERE intercompany != 0";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLusuarior, DB, usuarior);
            ViewData["UsuarioResponsable"] = usuarior;
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> medio = new List<SelectListItem>();
            string sentenciaSQLmedio = "SELECT idmedio, nombre FROM tickets.catmedios";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLmedio, DB, medio);
            ViewData["Medio"] = medio;
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> problema = new List<SelectListItem>();
            string sentenciaSQLproblema = "SELECT idCategoria, categoria FROM tickets.categoria";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLproblema, DB, problema);
            ViewData["Problema"] = problema;
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> tipoproblema = new List<SelectListItem>();
            string sentenciaSQLtipoProblema = "SELECT idProblema, problema FROM tickets.problemas WHERE idEstado = 1";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLtipoProblema, DB, tipoproblema);
            ViewData["TipoProblema"] = tipoproblema;

            stringConexionMySql.CerrarConexion();

            if (inter != "3")
            {
                List<SelectListItem> intercompany = new List<SelectListItem>();
                string sentenciaSQLintercompany = "SELECT id_empresa, name FROM tickets.intercompany";
                stringConexionMySql.LLenarDropDownList(sentenciaSQLintercompany, DB, intercompany);
                ViewData["Intercompany"] = intercompany;
                stringConexionMySql.CerrarConexion();

                DataTable dt = new DataTable();
                string cInst = "SELECT idCliente AS CODIGO, cliente AS CLIENTE, nit AS NIT FROM tickets.clientes;";
                ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteCXC(cInst, dt, DB);
                

            } else
            {
                List<SelectListItem> intercompany = new List<SelectListItem>();
                string sentenciaSQLintercompany = "SELECT id_empresa, name FROM tickets.intercompany WHERE id_empresa = 3";
                stringConexionMySql.LLenarDropDownList(sentenciaSQLintercompany, DB, intercompany);
                ViewData["Intercompany"] = intercompany;
                stringConexionMySql.CerrarConexion();

                DataTable dt = new DataTable();
                string cInst = "SELECT idCliente AS CODIGO, cliente AS CLIENTE, nit AS NIT FROM tickets.clientes WHERE idIntercompany = 3;";
                ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteCXC(cInst, dt, DB);
            }

            
            return (ActionResult)this.View();
        }



        [HttpPost]
        public string Cargar(Ticket oticket)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                DataTable dt = new DataTable();
                string cInst = "SELECT idProblema AS ID, problema AS PROBLEMA FROM tickets.problemas WHERE idEstado = 1 AND idCategoria = " + oticket.problema ;
                str = stringConexionMySql.LlenarDDLTipoProblema(cInst, dt, DB);                
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }
            
            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string GetDataCliente(Ticket oticket)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];
            

            try
            {
                string sentenciaSQL1 = "SELECT json_object('CODIGO', idCliente," + "'CLIENTE', cliente) " + "FROM tickets.clientes " + "WHERE idCliente = " + oticket.id_cliente;

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                }

                else
                {
                    sentenciaSQL1 = "SELECT json_object('CODIGO', idCliente," + "'CLIENTE', cliente) " + "FROM tickets.clientes " + "WHERE idCliente = " + oticket.id_cliente;
                    stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                    if (stringConexionMySql.consulta.Read())
                    {
                        str = stringConexionMySql.consulta.GetString(0).ToString();
                    }
                    else
                    {
                        str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                    }
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }
            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string InsertarTicket(Ticket oticket)
        {
            Guid g = Guid.NewGuid();
            string auto = g.ToString().ToUpper();

            string cUserConected = (string)(Session["Usuario"]);
            string inter = (string)(Session["Intercompany"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO tickets.ticket (fecha, idCliente, reporta, idCategoria, idProblema, error, descripcion, idMedio, usuarioAsignado, IdEstado, usuarioCreo, usuarioCerro, solucion, intercompany, fecha_finalizacion) VALUES (";
                cInst += "'" + oticket.fecha + "', ";
                cInst += oticket.id_cliente + ", ";
                cInst += "'" + oticket.reporta + "', ";
                cInst += oticket.problema + ", "; //Categoria
                cInst += oticket.tipoproblema + ", ";//Problema
                cInst += "'" + oticket.error + "', ";
                cInst += "'" + oticket.descripcion + "', ";
                cInst += oticket.medio + ", ";
                cInst += "'" + oticket.usuarior + "', ";
                cInst += "1, "; //Estado
                cInst += "'" + cUserConected + "', ";
                cInst += "'" + "', ";//usuarioCerro
                cInst += "'" + "', ";//solucion
                cInst += oticket.intercompany + ", ";//intercompany
                cInst += "'" + "'); ";//fecha_finalizacion

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
