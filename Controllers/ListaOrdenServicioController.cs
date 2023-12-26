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
    public class ListaOrdenServicioController : Controller
    {
        // GET: ListaOrdenServicio
        public ActionResult ListaOrdenServicio()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
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

                string cInst = "SELECT a.id_orden AS ID, a.status AS STATUS, a.cliente AS CLIENTE, a.direccion AS DIRECCION, c.descripcion AS RUTA, a.fechaa AS FECHA, a.fechai AS INICIO, a.fechaf AS FINALIZACION, a.obs AS OBSERVACION, a.descripcion AS DESCRIPCION, b.nombre AS TECNICO ";
                cInst += "FROM ordentecnica a ";
                cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                cInst += "GROUP BY a.id_orden ";

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
                    string cInst = "SELECT JSON_OBJECT('ID', a.id_orden, 'IDCLIENTE', a.id_cliente, 'CLIENTE', a.cliente, 'TECNICO', b.nombre, 'DIRECCION', a.direccion, 'TELEFONO', a.telefono, 'RUTA' , c.descripcion, 'TIPOORDEN', d.descripcion, 'FECHAA', a.fechaa,'OBSERVACION', a.obs, 'STATUS', a.status, 'DESCRIPCION', a.descripcion, 'FECHAI', a.fechai, 'FECHAF', a.fechaf) ";
                    cInst += "FROM ordentecnica a ";
                    cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                    cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                    cInst += "INNER JOIN cattipoorden d ON (a.id_tipoorden = d.id_tipoorden) ";
                    //cInst += "WHERE a.status IN( 'INGRESADA','PROCESO') ";

                    //if (oCableVision.id_orden == null && (oCableVision.status.ToString() != "")) {
                    //    cInst += " AND a.id_orden  = " + oCableVision.status.ToString();
                    //} else if (oCableVision.id_orden != "")
                    //{
                    //    cInst += " AND a.id_orden  = " + oCableVision.id_orden;
                    //}

                    if (oCableVision.id_orden == null && (oCableVision.status != ""))
                    {
                        cInst += " AND a.id_orden  = " + oCableVision.status;
                    }
                    else if (oCableVision.id_orden != "")
                    {
                        cInst += " AND a.id_orden  = " + oCableVision.id_orden;
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
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                }
            }
            llenar.CerrarConexion();
            return str;
        }
    }
}