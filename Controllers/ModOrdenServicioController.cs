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
    public class ModOrdenServicioController : Controller
    {
        // GET: CableVision
        public ActionResult ModOrdenServicio()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQLlineas = "SELECT id_codigo, cliente FROM clientes";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLlineas, DB, lineas);
            ViewData["Clientes"] = lineas;

            List<SelectListItem> tecnicos = new List<SelectListItem>();
            string setenciaSQLtecnicos = "SELECT id_tecnico, nombre FROM tecnico";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtecnicos, DB, tecnicos);
            ViewData["Tecnicos"] = tecnicos;

            List<SelectListItem> tipos = new List<SelectListItem>();
            string setenciaSQLtipos = "SELECT id_tipoorden, descripcion FROM cattipoorden";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtipos, DB, tipos);
            ViewData["Tipos"] = tipos;

            List<SelectListItem> rutas = new List<SelectListItem>();
            string setenciaSQLrutas = "SELECT id_ruta, descripcion FROM catrutaclie";
            stringConexionMySql.LLenarDropDownList(setenciaSQLrutas, DB, rutas);
            ViewData["Rutas"] = rutas;

            DataTable dt = new DataTable();

            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO,telefono AS TELEFONO FROM clientes ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteOS(cInst, dt, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }

        [HttpPost]
        public object ConsultarOrdenServicio()
        {
            string cUserConected = (string)(Session["Usuario"]);
            string DB = (string)this.Session["StringConexion"];

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                //string cInst = "SELECT a.id_orden AS ID, a.status AS STATUS, a.cliente AS CLIENTE, a.direccion AS DIRECCION, c.descripcion AS RUTA, a.fechaa AS FECHA, a.obs AS OBSERVACION, b.nombre AS TECNICO ";
                string cInst = "SELECT a.id_orden AS ID, a.status AS STATUS, a.cliente AS CLIENTE, a.direccion AS DIRECCION, c.descripcion AS RUTA, a.fechaa AS FECHA, a.fechai AS INICIO, a.fechaf AS FINALIZACION, a.obs AS OBSERVACION, a.descripcion AS DESCRIPCION, b.nombre AS TECNICO ";
                cInst += "FROM ordentecnica a ";
                cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                cInst += "WHERE a.status IN ('INGRESADA', 'PROCESO')";

                LlenarListaDocumentos.listaOrdenesServicio = llenar.ListaOrdenesServicio(cInst, DB, LlenarListaDocumentos.listaOrdenesServicio);
            }
            return JsonConvert.SerializeObject(LlenarListaDocumentos.listaOrdenesServicio);
        }




        //CONTROLADOR QUE SIRVE PARA ABRIR EL MODAL CON LA INFORMACION
        [HttpPost]
        public object GetDataOrdenServicio(CableVision oCableVision)
        {
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                try
                {
                    string cInst = "SELECT JSON_OBJECT('ID', a.id_orden, 'IDCLIENTE', a.id_cliente, 'CLIENTE', a.cliente, 'TECNICO', b.nombre, 'DIRECCION', a.direccion, 'TELEFONO', a.telefono, 'RUTA' , c.descripcion, 'TIPOORDEN', d.descripcion, 'FECHAA', a.fechaa,'OBSERVACION', a.obs, 'STATUS', a.status, 'DESCRIPCION', a.descripcion, 'FECHAI', a.fechai, 'FECHAF', a.fechaf, 'IDRUTA', a.id_ruta, 'IDTIPOORDEN', a.id_tipoorden, 'IDTECNICO', a.id_tecnico) ";
                    cInst += "FROM ordentecnica a ";
                    cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                    cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                    cInst += "INNER JOIN cattipoorden d ON (a.id_tipoorden = d.id_tipoorden) ";
                    //cInst += "WHERE a.status IN( 'INGRESADA','PROCESO') ";

                    if (oCableVision.id_orden.ToString() != "")
                    {
                        cInst += " AND a.id_orden  = " + oCableVision.id_orden.ToString();
                    }
                    //cInst += " GROUP BY a.id_orden ";

                    llenar.EjecutarLectura(cInst, oDb);
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
                    //str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
                }
            }
            llenar.CerrarConexion();
            return str;
        }




        //CONTROLADOR PARA EDITRAR LAS ORDENES DE SERVICIO
        [HttpPost]
        public string ModificarOrdenServicio(CableVision oCableVision)
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
                //if (Convert.ToInt32(oCableVision.id_ruta) >= 1 && Convert.ToInt32(oCableVision.id_ruta) <= 9) {
                //    oCableVision.id_ruta.ToString();

                //    oCableVision.id_ruta = "0"+ oCableVision.id_ruta;
                //}

                string cInst = "UPDATE ordentecnica SET ";
                cInst += "direccion = " + "'" + oCableVision.direccion + "', ";
                cInst += "telefono = " + "'" + oCableVision.telefono + "', ";
                cInst += "id_ruta = '" + oCableVision.id_ruta + "', ";
                cInst += "id_tecnico = " + oCableVision.id_tecnico.ToString() + ", ";
                cInst += "id_tipoorden = " + oCableVision.id_tipoorden.ToString() + ", ";
                cInst += "obs = " + "'" + oCableVision.obs + "' ";
                cInst += "WHERE id_orden = " + oCableVision.id_orden + ";";

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
                //str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            modificar.CerrarConexion();
            return str;
        }


        //CONTOLADOR PARA ANULAR LA ORDEN DE SERVICIO
        [HttpPost]
        public string AnularOrdenServicio(CableVision oCableVision)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL anular = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE ordentecnica SET ";
                cInst += "status = 'ANULADA'";
                cInst += "WHERE id_orden = " + oCableVision.id_orden + ";";

                lError = anular.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    anular.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                //str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";
            
            anular.CerrarConexion();
            return str;
        }


    }
}