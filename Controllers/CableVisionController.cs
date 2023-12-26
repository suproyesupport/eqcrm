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
using MySql.Data.MySqlClient;

namespace EqCrm.Controllers
{
    public class CableVisionController : Controller
    {
        // GET: CableVision
        public ActionResult CableVision()
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
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> tecnicos = new List<SelectListItem>();
            string setenciaSQLtecnicos = "SELECT id_tecnico, nombre FROM tecnico";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtecnicos, DB, tecnicos);
            ViewData["Tecnicos"] = tecnicos;

            stringConexionMySql.CerrarConexion();

            List<SelectListItem> tipos = new List<SelectListItem>();
            string setenciaSQLtipos = "SELECT id_tipoorden, descripcion FROM cattipoorden";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtipos, DB, tipos);
            ViewData["Tipos"] = tipos;

            stringConexionMySql.CerrarConexion();

            List<SelectListItem> rutas = new List<SelectListItem>();
            string setenciaSQLrutas = "SELECT id_ruta, descripcion FROM catrutaclie";
            stringConexionMySql.LLenarDropDownList(setenciaSQLrutas, DB, rutas);
            ViewData["Rutas"] = rutas;

            stringConexionMySql.CerrarConexion();

            DataTable dt = new DataTable();

            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO,telefono AS TELEFONO FROM clientes ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteOS(cInst, dt, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }


        [HttpPost]
        public string BuscarOrden(Models.CableVision oOrden)
        {
            //string cUserConected = (string)(Session["Usuario"]);
            

            string DB = (string)this.Session["StringConexion"];
            //StringConexionMySQL llenar = new StringConexionMySQL();
            //string cEjecturar = "";
            string str = "";
            string cInst = "SELECT JSON_OBJECT('ID', ifnull(max(a.id_orden),0)) AS ID FROM ordentecnica a WHERE serie = 'O'";


            try
            {
                using (MySqlConnection connection = new MySqlConnection(DB))
                {

                    connection.Open();

                    MySqlCommand command = new MySqlCommand(cInst, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            str = reader[0].ToString();

                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                string cError = "";
                cError = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
                return cError;
            }

            return str;

           


            
            
        }


        [HttpPost]
        public string InsertarOrden(Models.CableVision oOrden)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            string cInst = "";
            int nMayor = 0;

            //oOrden.id_orden = Funciones.NumeroMayor("ordentecnica", "id_orden", "serie ='O'", oDb).ToString();

            string instruccionSQL = "";
            instruccionSQL += String.Format("SELECT IFNULL(max({0}), 0) ", "id_orden");
            instruccionSQL += String.Format("FROM {0} ", "ordentecnica");
            instruccionSQL += string.Format("WHERE {0} ", "serie ='O'");

            insertar.EjecutarLectura(instruccionSQL, oDb);

            
            // Verificamos si la consulta obtuvo resultados
            if (insertar.consulta != null)
            {
                // Recorremos los datos encontrados
                while (insertar.consulta.Read())
                {
                    // Asignamos el registro a la variable global
                    nMayor = insertar.consulta.GetInt32(0);
                    nMayor = nMayor + 1;
                }
                // Cerramos la consulta
                insertar.CerrarConexion();
            }
            else
            {
                nMayor = 1;
            }

            oOrden.id_orden = nMayor.ToString();

            try
            {
                cInst = "INSERT INTO ordentecnica";
                cInst += "(id_orden, serie,id_cliente, cliente, direccion, telefono, id_ruta, id_tecnico, id_tipoorden, obs, fechaa)";
                cInst += "VALUES (";
                cInst += oOrden.id_orden + ",'O', ";
                cInst += oOrden.id + ", "; //ID Cliente
                cInst += "'" + oOrden.cliente + "', ";
                cInst += "'" + oOrden.direccion + "', ";
                cInst += "'" + oOrden.telefono + "', ";
                cInst += "'" + oOrden.id_ruta.ToString() + "', ";
                cInst += oOrden.id_tecnico.ToString() + ", ";
                cInst += "'" + oOrden.id_tipoorden.ToString() + "', ";
                cInst += "'" + oOrden.obs + "', ";
                cInst += "NOW()" + ");";

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
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }
    }
}