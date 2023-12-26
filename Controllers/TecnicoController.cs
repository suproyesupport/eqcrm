using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class TecnicoController : Controller
    {
        // GET: Tecnico
        public ActionResult Tecnico()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
          
            return (ActionResult)this.View();
        }

        [HttpPost]
        public string InsertarTecnico(Models.Tecnico oTecnico)
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
                string cInst = "INSERT INTO tecnico ";
                cInst += "VALUES (NULL, '" + oTecnico.nombre + "', 'A');";

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
        public string ConsultaTecnico()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";

            instruccionSQL = "SELECT id_tecnico AS ID, ifnull(nombre,'NOMBRE NO REGISTRADO') AS NOMBRE FROM tecnico";

            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();
            lectura.CerrarConexion();

            return cResultado;
        }
    }
}