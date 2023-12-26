using EqCrm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace EqCrm.Controllers
{
    public class LineaInventarioController : Controller
    {
        // GET: LineaInventario
        public ActionResult LineaInventario()
        {
            return View();
        }
        
        public string Guardar(LineaInventario oLinea)
        {
            string db = (string)this.Session["StringConexion"];
            dapperConnect dapper = new dapperConnect();
            bool lError = false;
            string cError = "";
            //string query = String.Format("INSERT INTO catlineasi (id_linea, descripcion) VALUES ({0}, '{1}')", oLinea.Id, oLinea.Descripcion);

            string query = "INSERT INTO catlineasi (id_linea, descripcion) VALUES (@Id, @descripcion)";
            object[] parametros = { new { Id = oLinea.Id, descripcion = oLinea.Descripcion } };

            // Creamos los parametros para factura
            
            lError = dapper.EqExecuteParam(query, db, ref cError, parametros);
            if (lError == true)
            {
                return "error";
            }



            try
            {
                dapper.EqExecute(query, db, ref cError);

            } catch (Exception ex) { 
            
            }

           





            return "";
        
        }

    }
}