using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class LineaPInventarioController : Controller
    {
        // GET: LineaPInventario
        public ActionResult LineaPInventario()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

        [HttpPost]
        public string InsertarLinea(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {

                string cInst = "INSERT INTO catlineasp ";
                cInst += "( id_linea,descripcion) ";
                cInst += "VALUES ( ";
                cInst += "'" + oInven.id_linea + "',";
                cInst += "'" + oInven.descripcion + "')";

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

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"REGISTRO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }

        [HttpPost]
        public string ConsultaLinea()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            instruccionSQL = "SELECT id_linea AS LINEAP,ifnull(descripcion,'LINEA NO REGISTRADA') AS DESCRIPCION FROM catlineasp";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;




        }
    }
}