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

namespace EqCrm.Controllers
{
    public class ModInventarioController : Controller
    {
        // GET: ModInventario
        public ActionResult ModInventario()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = " SELECT id_linea, descripcion FROM catlineasi";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL, DB, lineas);
            ViewData["Lineas"] = lineas;

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GetInventario(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);


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


                string cInst = "SELECT  CONCAT(a.id_codigo,' ','') AS DATOS,a.id_codigo as CODIGO, a.codigoe AS CODIGOE, a.numero_departe AS ALIAS, a.producto AS PRODUCTO,a.linea AS LINEA,ifnull(sum(b.entrada-b.salida),0) AS EXISTENCIA,a.precio1 AS PRECIO1,a.precio2 AS PRECIO2,a.precio3 AS PRECIO3,a.precio4 AS PRECIO4, a.costo1 AS COSTO1, a.costo2 AS COSTO2, a.OBS ";
                cInst += " FROM inventario a ";
                cInst += " LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) ";

                cInst += " WHERE status = 'A'";

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




        [HttpPost]
        public string GetDataInv(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            if (string.IsNullOrEmpty(oInven.id_codigo))
            {
                oInven.id_codigo = "";
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                return str;
            }


            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {

                string cInst = "SELECT JSON_OBJECT('CODIGO',a.id_codigo," + "'CODIGOE',a.codigoe," + "'PRODUCTO',a.producto," + "'LINEA',c.descripcion," + "'LINEAP',d.descripcion," + "'EXISTENCIA',ifnull(sum(b.entrada-b.salida),0)," + "'PRECIO1',a.precio1," + "'PRECIO2',a.precio2," + "'PRECIO3',a.precio3," + "'PRECIO4',a.precio4," + "'COSTO1',a.costo1," + "'COSTO2',a.costo2," + "'COSTO3',a.costo3," + "'COSTO4',a.costo4," + "'OBS',a.obs," + "'STOCKMIN',a.stockmin," + "'STOCKMAX',a.stockmax," + "'SERVICIO',a.servicio,"+"'ALIAS',a.numero_departe ) ";
                cInst += " FROM inventario a ";
                cInst += " LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) ";
                cInst += " LEFT JOIN catlineasi c ON (a.linea= c.id_linea)";
                cInst += " LEFT JOIN catlineasp d ON (a.lineac= d.id_linea)";
                cInst += " WHERE status = 'A'";

                if (oInven.id_codigo.ToString() != "")
                {
                    cInst += " AND a.id_codigo  = " + oInven.id_codigo.ToString();

                }
                cInst += " GROUP BY a.id_codigo ";

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
        public string ModificarProducto(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL modificar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE inventario SET ";
                cInst += "codigoe = " + "'" + oInven.codigoeq + "', ";
                cInst += "producto = " + "'" + oInven.producto + "', ";
                cInst += "linea = " + "'" + oInven.id_linea + "', ";
                cInst += "lineac = " + "'" + oInven.lineaeq + "', ";
                cInst += "costo1 = " + oInven.costoeq1 + ", ";
                cInst += "costo2 = " + oInven.costoeq2 + ", ";
                cInst += "costo3 = " + oInven.costoeq3 + ", ";
                cInst += "costo4 = " + oInven.costoeq4 + ", ";
                cInst += "servicio = " + "'" + oInven.servicio + "', ";
                cInst += "stockmin = " + oInven.stockmin + ", ";
                cInst += "stockmax = " + oInven.stockmax + ", ";
                cInst += "precio1 = " + oInven.precioeq1 + ", ";
                cInst += "precio2 = " + oInven.precioeq2 + ", ";
                cInst += "precio3 = " + oInven.precioeq3 + ", ";
                cInst += "precio4 = " + oInven.precioeq4 + ", ";
                cInst += "obs = " + "'" + oInven.obs + "', ";
                cInst += "numero_departe = " + "'" + oInven.alias + "' ";
                cInst += "WHERE id_codigo = " + oInven.idcodigoeq + ";";

                //cInst += "WHERE id_codigo = " + "'" + oInven.idcodigoeq + "'" + ";";

                lError = modificar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    modificar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            modificar.CerrarConexion();
            return str;
        }

    }

}



