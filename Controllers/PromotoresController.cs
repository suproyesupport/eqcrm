using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class PromotoresController : Controller
    {
        // GET: Promotores
        public ActionResult Promotores()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

        [HttpPost]
        public string BuscarID()
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {

                string cInst = "SELECT JSON_OBJECT('CODIGO',max(a.id_codigo)) AS CODIGO FROM vendedores a";




                llenar.EjecutarLectura(cInst, DB);
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

            llenar.CerrarConexion();
            return str;
        }
        [HttpPost]
        public string InsertarPromotor(Promotores oPromo)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            Funciones f = new Funciones();
            try
            {
                
                
                string cInst = "INSERT INTO vendedores ";
                cInst += "( id_codigo,nombre, id_agencia, status, telefono, celular, email) ";
                cInst += "VALUES ( ";
                cInst += oPromo.id_vendedor + ",";
                cInst += "'" + oPromo.nombre + "',";
                cInst += "'" + oPromo.id_vendedor + "',";
                cInst += "'A',";
                cInst += "'" + oPromo.telefono + "',";
                cInst += "'" + oPromo.celular + "',";
                cInst += "'" + oPromo.email + "')";
                

                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }

                if(f.DamePermisoApp("2202",DB)==true)
                {
                    cInst = "INSERT INTO catagencias ";
                    cInst += "( id_agencia,nombre, consolida) ";
                    cInst += "VALUES ( ";
                    cInst += oPromo.id_vendedor + ",";
                    cInst += "'" + oPromo.nombre + "',";
                    cInst += "'S')";
                    

                    lError = insertar.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {
                        cMensaje = cError;
                        insertar.CerrarConexion();
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

            insertar.CerrarConexion();
            return str;
        }

        [HttpPost]
        public string ConsultaPromotores()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            instruccionSQL = "SELECT id_codigo AS ID,nombre AS NOMBRE, status AS STATUS, telefono AS TELEFONO,celular AS CELULAR, email AS EMAIL FROM vendedores";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;




        }
    }
}