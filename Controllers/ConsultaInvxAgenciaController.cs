using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ConsultaInvxAgenciaController : Controller
    {
        // GET: ConsultaInvxAgencia
        public ActionResult ConsultaInvxAgencia()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = "select id_linea,descripcion from catlineasi";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL, DB, lineas);
            ViewData["Lineas"] = lineas;


            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }


        //CONSULTA DE INVENTARIO X AGENCIA 
        [HttpPost]
        public object GetInventarioxAgencia(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string id_agencia = (string)this.Session["Sucursal"];


            if (string.IsNullOrEmpty(oInven.id_codigo))
            {
                oInven.id_codigo = "";
            }

            if (string.IsNullOrEmpty(oInven.codigoe))
            {
                oInven.codigoe = "";
            }

            if (string.IsNullOrEmpty(oInven.linea))
            {
                oInven.linea = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string oDb = (string)(Session["StringConexion"]);
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];


                string cInst = "SELECT  CONCAT(a.id_codigo,' ','') AS DATOS,a.id_codigo as CODIGO, a.codigoe AS CODIGOE,a.producto AS PRODUCTO,a.linea AS LINEA, " +
                    "ifnull((SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= "+ id_agencia + "),0) AS EXISTENCIA,a.precio1 AS PRECIO1,a.precio2 AS PRECIO2,a.precio3 AS PRECIO3,a.precio4 AS PRECIO4, a.OBS ";
                cInst += " FROM inventario a ";
                cInst += " LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) ";
                cInst += " WHERE status = 'A' ";

                if (oInven.id_codigo.ToString() != "")
                {
                    cInst += " AND a.id_codigo  = " + oInven.id_codigo.ToString();
                }

                if (oInven.codigoe.ToString() != "")
                {
                    cInst += " AND a.codigoe = '" + oInven.codigoe.ToString() + "'";
                }

                if (oInven.linea.ToString() != "")
                {
                    cInst += " AND a.linea  = '" + oInven.linea.ToString() + "'";
                }

                cInst += " GROUP BY a.id_codigo ";

                LlenarListaInventario.lista = llenar.ListaIventario(cInst, DB, LlenarListaInventario.lista);

            }

            return JsonConvert.SerializeObject(LlenarListaInventario.lista);
        }



    }
}