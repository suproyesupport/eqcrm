using Dapper;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Xpo.DB.Helpers;
using EqCrm.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EqCrm.Models;

namespace EqCrm.Controllers
{
    public class ModClientesController : Controller
    {
        // GET: ModClientes
        public ActionResult ModClientes()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string db = (string)this.Session["StringConexion"];
            string query = "";
            string cError = "";

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            dapperConnect dapper = new dapperConnect();

            if (dapper.ValidaTablas(db, "statusclientes") == false )
            {
                query = "CREATE TABLE statusclientes(id_status varchar(2) primary key not null, nombre varchar(30) not null);";
                dapper.EqExecute(query, db, ref cError);
            }

            List<SelectListItem> status = new List<SelectListItem>();
            query = "SELECT id_status, nombre FROM statusclientes";
            stringConexionMySql.LLenarDropDownList(query, db, status);
            ViewData["status"] = status;

            return View();
        }



        







        // Función de tipo sting que devuelve el listado
        public string GetList()
        {
            // Variable que guarda el stringconexion de una variable de sesión llamada "StringConexion"
            string db = (string)this.Session["StringConexion"];
            
            //Variable que guarda el query, se utiliza string.Format para evitar SQL Injection
            string query = string.Format("SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} FROM clientes LIMIT 111111111;", 
                "id_codigo", "cliente", "nit", "status", "facturar", "direccion", "telefono", "email");

            // Instancia de la clase dapperConnect en el objeto dapper
            dapperConnect dapper = new dapperConnect();

            var resultado = dapper.ExecuteList<dynamic>(db, query);

            var json = JsonConvert.SerializeObject(resultado);

            return json.ToString();
        }



        public string GetData(string id_codigo)
        {
            // Variable que guarda el stringconexion de una variable de sesión llamada "StringConexion"
            string db = (string)this.Session["StringConexion"];

            //Variable que guarda el query, se utiliza string.Format para evitar SQL Injection
            string query = string.Format("SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9} FROM clientes WHERE id_codigo = {10};",
                "id_codigo", "status", "nit", "cliente", "direccion", "telefono", "email", "atencion", "clasifica", "id_ruta", id_codigo);

            // Instancia de la clase dapperConnect en el objeto dapper
            dapperConnect dapper = new dapperConnect();

            var resultado = dapper.ExecuteList<dynamic>(db, query);

            var json = JsonConvert.SerializeObject(resultado);

            return json.ToString();
        }



        public string GetListComboStatus()
        {
            return "";

        }




    }
}