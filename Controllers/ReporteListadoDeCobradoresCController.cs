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
    public class ReporteListadoDeCobradoresCController : Controller
    {
        // GET: ReporteListadoDeCobradoresC
        public ActionResult ReporteListadoDeCobradoresC()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }

        [HttpPost]
        public object GenerarReporteListaDeCobradores()
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

                string query = "SELECT a.id_codigo AS CODIGO,b.cliente AS CLIENTE,b.fechain AS FECHAI,c.descripcion AS RUTA,b.direccion AS DIRECCION,b.telefono AS TELEFONO,a.fechaa AS FECHA,month(a.fechaa)AS MES, a.tipodocto as MESACOBRAR, FORMAT(a.importe,2) AS IMPORTE, FORMAT(a.saldo,2) AS SALDO ";
                query += " FROM ctacc a ";
                query += "INNER JOIN clientes b ON(a.id_codigo= b.id_codigo) ";
                query += "INNER JOIN catrutaclie c ON (c.id_ruta = b.id_ruta) ";
                query += "WHERE a.saldo >= 1 AND b.status = 'A' ORDER BY b.cliente";

                LlenarListaDeCobradoresC.listaReporteListadoCobradores = llenar.ListaRpoListadoCobradoresC(query, DB, LlenarListaDeCobradoresC.listaReporteListadoCobradores);
            }

            var deserialize = JsonConvert.SerializeObject(LlenarListaDeCobradoresC.listaReporteListadoCobradores);

            return deserialize.ToString().Replace("12:00:00", "").Replace("00:00:00", "");
            //return deserialize.ToString().Replace("12:00:00", "").Replace("AM","").Replace("00:00:00","");
        }
    }
}