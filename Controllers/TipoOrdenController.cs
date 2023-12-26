using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;


namespace EqCrm.Controllers
{
    public class TipoOrdenController : Controller
    {
        // GET: TipoOrden
        public ActionResult TipoOrden()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }


            return (ActionResult)this.View();
        }


        [HttpPost]
        public string InsertarTipoOrden(Models.TipoOrden oTipoOrden)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            //oTecnico.id_orden = Funciones.NumeroMayor("ordentecnica", "id_orden", "serie ='O'", oDb).ToString();

            try
            {
                string cInst = "INSERT INTO cattipoorden ";
                cInst += "VALUES (NULL, '" + oTipoOrden.descripcion + "');";

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
        public string ConsultaTipoOrden()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";

            instruccionSQL = "SELECT id_tipoorden AS ID, ifnull(descripcion,'TIPO NO REGISTRADO') AS DESCRIPCION FROM cattipoorden";

            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();
            lectura.CerrarConexion();

            return cResultado;
        }
    }
}