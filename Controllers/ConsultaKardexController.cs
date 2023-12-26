using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using EqCrm.Models;

namespace EqCrm.Controllers
{
    public class ConsultaKardexController : Controller
    {
        // GET: ConsultaKardex
        public ActionResult ConsultaKardex()
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
        public object GetKardex(Inventario oInven)
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


                string cInst = "SELECT  CONCAT(a.id_codigo,' ','') AS DATOS,a.id_codigo as CODIGO, a.codigoe AS CODIGOE,a.producto AS PRODUCTO,a.linea AS LINEA,ifnull(sum(b.entrada-b.salida),0) AS EXISTENCIA,a.precio1 AS PRECIO1,a.precio2 AS PRECIO2,a.precio3 AS PRECIO3,a.precio4 AS PRECIO4, a.OBS ";
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
        public object GetListKardex(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            dapperConnect dapper = new dapperConnect();
            string DB = (string)this.Session["StringConexion"];

            string query = "SELECT  CONCAT(a.id_codigo,' ','') AS DATOS,a.id_codigo as CODIGO, a.codigoe AS CODIGOE,a.producto AS PRODUCTO,a.linea AS LINEA,IFNULL(sum(b.entrada-b.salida),0) AS EXISTENCIA, " +
                "a.precio1 AS PRECIO1,a.precio2 AS PRECIO2,a.precio3 AS PRECIO3,a.precio4 AS PRECIO4, a.OBS " +
                "FROM inventario a " +
                "LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) " +
                "WHERE status = 'A' ";

            if (!string.IsNullOrEmpty(oInven.id_codigo))
            {
                query += "AND a.id_codigo  = " + oInven.id_codigo.ToString() + " ";
            }

            if (!string.IsNullOrEmpty(oInven.codigoe))
            {
                query += "AND a.codigoe = '" + oInven.codigoe.ToString() + "' ";
            }

            if (!string.IsNullOrEmpty(oInven.linea))
            {
                query += "AND a.linea  = '" + oInven.linea.ToString() + "' ";
            }

            query += "GROUP BY a.id_codigo;";

            var resultado = dapper.ExecuteList<dynamic>(DB, query);

            string json = JsonConvert.SerializeObject(resultado);

            return json;













        }



        [HttpPost]
        public string ConsultaKardexCodigo(string id_codigo)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            instruccionSQL = "SET @nExistencia=0; ";
            instruccionSQL += "SELECT kardexinven.fecha AS FECHA,catkardex.nombre AS MOVI,kardexinven.docto AS DOCTO,kardexinven.serie AS SERIE, ";
            instruccionSQL += "kardexinven.entrada AS ENTRADA,kardexinven.salida AS SALIDA,@nExistencia:=(kardexinven.entrada-kardexinven.salida+@nExistencia) as EXIST, kardexinven.costo1 AS COSTO,kardexinven.obs AS OBS,kardexinven.hechopor AS HECHOPOR ";
            instruccionSQL += "FROM kardexinven ";
            instruccionSQL += "LEFT JOIN catkardex ON ( catkardex.id_movi = kardexinven.id_movi ) WHERE kardexinven.id_codigo = " + id_codigo.ToString() ;


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;
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

                string cInst = "SELECT JSON_OBJECT('CODIGO',a.id_codigo," + "'CODIGOE',a.codigoe," + "'PRODUCTO',a.producto," + "'LINEA',c.descripcion," + "'LINEAP',d.descripcion," + "'EXISTENCIA',ifnull(sum(b.entrada-b.salida),0)," + "'PRECIO1',a.precio1," + "'PRECIO2',a.precio2," + "'PRECIO3',a.precio3," + "'PRECIO4',a.precio4," + "'COSTO1',a.costo1," + "'COSTO2',a.costo2," + "'COSTO3',a.costo3," + "'COSTO4',a.costo4," + "'OBS',a.obs," + "'STOCKMIN',a.stockmin," + "'STOCKMAX',a.stockmax," + "'SERVICIO',a.servicio," + "'ALIAS',a.numero_departe ) ";
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
    }
}