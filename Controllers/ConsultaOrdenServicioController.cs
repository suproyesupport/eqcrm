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
    public class ConsultaOrdenServicioController : Controller
    {
        // GET: ConsultaOrdenServicio -- prueba git
        public ActionResult ConsultaOrdenServicio()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return (ActionResult)this.View();
        }


        [HttpPost]
        public string IniciarOrden(CableVision oCableVision)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL inciaorden = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE ordentecnica SET fechai = NOW(), ";
                cInst += "status = 'PROCESO' ";
                cInst += "WHERE id_orden = " + oCableVision.id_orden + ";";

                lError = inciaorden.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    inciaorden.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            inciaorden.CerrarConexion();
            return str;
        }




        [HttpPost]
        public string FinalizarOrden(CableVision oCableVision)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL inciaorden = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE ordentecnica SET fechaf = NOW(), ";
                cInst += " descripcion = '" + oCableVision.descripcion + "', ";
                cInst += " status = 'FINALIZADA' ";
                cInst += "WHERE id_orden = " + oCableVision.id_orden + ";";
                lError = inciaorden.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    inciaorden.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            inciaorden.CerrarConexion();
            return str;
        }




        [HttpPost]
        public object ConsultarOrdenServicio()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];

                string cInst = "SELECT a.id_orden AS ID, a.status AS STATUS, a.cliente AS CLIENTE, a.direccion AS DIRECCION, c.descripcion AS RUTA, a.fechaa AS FECHA, a.obs AS OBSERVACION, b.nombre AS TECNICO ";
                cInst += "FROM ordentecnica a ";
                cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                cInst += "WHERE a.status IN( 'INGRESADA','PROCESO') AND b.nombre = '" + cUserConected + "'";

                LlenarListaDocumentos.listaOrdenesServicio = llenar.ListaOrdenesServicio(cInst, DB, LlenarListaDocumentos.listaOrdenesServicio);
            }
            return JsonConvert.SerializeObject(LlenarListaDocumentos.listaOrdenesServicio);
        }


        //[HttpPost]
        //public object ListaOrdenServicio()
        //{
        //    string cUserConected = (string)(Session["Usuario"]);
        //    string DB = (string)this.Session["StringConexion"];
        //    string cInst = "";


        //    if (string.IsNullOrEmpty(cUserConected))
        //    {
        //        return (ActionResult)this.RedirectToAction("Login", "Account");
        //    }
        //    else
        //    {
        //        StringConexionMySQL llenar = new StringConexionMySQL();

        //            cInst = "SELECT a.id_orden AS ID, a.status AS STATUS, a.cliente AS CLIENTE, a.direccion AS DIRECCION, c.descripcion AS RUTA, a.fechaa AS FECHA, a.obs AS OBSERVACION, b.nombre AS TECNICO ";
        //            cInst += "FROM ordentecnica a ";
        //            cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
        //            cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
        //            cInst += "WHERE a.status IN( 'INGRESADA','PROCESO') AND b.nombre = '" + cUserConected + "'";
                

        //        LlenarListaDocumentos.listaOrdenesServicio = llenar.ListaOrdenesServicio(cInst, DB, LlenarListaDocumentos.listaOrdenesServicio);
        //    }
        //    return JsonConvert.SerializeObject(LlenarListaDocumentos.listaOrdenesServicio);
        //}



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
                    string cInst = "SELECT JSON_OBJECT('ID', a.id_orden, 'IDCLIENTE', a.id_cliente, 'CLIENTE', a.cliente, 'TECNICO', b.nombre, 'DIRECCION', a.direccion, 'TELEFONO', a.telefono, 'RUTA' , c.descripcion, 'TIPOORDEN', d.descripcion, 'FECHAA', a.fechaa,'OBSERVACION', a.obs, 'STATUS', a.status) ";
                    cInst += "FROM ordentecnica a ";
                    cInst += "INNER JOIN tecnico b ON (a.id_tecnico = b.id_tecnico) ";
                    cInst += "INNER JOIN catrutaclie c ON (a.id_ruta = c.id_ruta) ";
                    cInst += "INNER JOIN cattipoorden d ON (a.id_tipoorden = d.id_tipoorden) ";
                    cInst += "WHERE a.status IN( 'INGRESADA','PROCESO') ";

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
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                }
            }
            llenar.CerrarConexion();
            return str;
        }
    }
}