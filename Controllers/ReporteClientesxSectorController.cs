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
using System.Globalization;

namespace EqCrm.Controllers
{
    public class ReporteClientesxSectorController : Controller
    {
        // GET: ReporteClientesxSector
        public ActionResult ReporteClientesxSector()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpPost]
        public object GenerarReporteClientesxSector()
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

                string query = "SELECT @i := @i + 1 AS ID, IFNULL ((SELECT descripcion FROM catrutaclie WHERE catrutaclie.id_ruta = clientes.id_ruta),'NO ASIGNADO' ) AS SECTOR, COUNT(clientes.id_ruta) AS ACTIVOS ";
                query += "FROM clientes ";
                query += "CROSS JOIN (SELECT @i := 0) a ";
                query += "WHERE clientes.status = 'A' ";
                query += "GROUP BY clientes.id_ruta ";
                query += "ORDER BY ID; ";

                LlenarListaDocumentos.listaReporteClientesxSector = llenar.ListaRpoClientesxSector(query, DB, LlenarListaDocumentos.listaReporteClientesxSector);
            }

            var deserialize = JsonConvert.SerializeObject(LlenarListaDocumentos.listaReporteClientesxSector);

            return deserialize.ToString().Replace("12:00:00", "").Replace("00:00:00", "");
            //return deserialize.ToString().Replace("12:00:00", "").Replace("AM","").Replace("00:00:00","");
        }
    }
}