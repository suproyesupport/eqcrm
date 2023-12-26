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
using Rotativa;

namespace EqCrm.Controllers
{
    public class EntradaKardexController : Controller
    {
        // GET: EntradaKardex
        public ActionResult EntradaKardex()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string sucursal = (string)this.Session["Sucursal"];
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            List<SelectListItem> listmovimiento = new List<SelectListItem>();
            string sentenciaSQLmovimiento = "SELECT id_movi, nombre FROM catkardex WHERE id_movi <= 50";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLmovimiento, DB, listmovimiento);
            ViewData["Movimiento"] = listmovimiento;

            DataTable dt = new DataTable();
            string cInst = "SELECT id_codigo AS CODIGO, codigoe AS CODIGOE, existencia AS EXISTENCIAS, costo1 AS COSTO1, producto AS PRODUCTO FROM inventario ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteCXC(cInst, dt, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }



        [HttpPost]
        public string GetDataProducto(Inventario oInv)
        {
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string sentenciaSQL1 = "SELECT JSON_OBJECT('CODIGO', id_codigo," + "'CODIGOE', codigoe" + "'PRODUCTO', producto " + "'COSTO1', costo1) " + "FROM inventario " + "WHERE id_codigo = " + oInv.id_codigo;

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                }
                else
                {
                    sentenciaSQL1 = "SELECT JSON_OBJECT('CODIGO', id_codigo," + "'CODIGOE', codigoe" + "'PRODUCTO', producto " + "'COSTO1', costo1) " + "FROM inventario " + "WHERE id_codigo = " + oInv.id_codigo;
                    stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                    if (stringConexionMySql.consulta.Read())
                    {
                        str = stringConexionMySql.consulta.GetString(0).ToString();
                    }
                    else
                    {
                        str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                    }
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string IngresarKardex(string informacion, Inventario oInv)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //Convertimos el JSON en un DataTable
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(informacion, (typeof(DataTable)));

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            string sucursal = (string)this.Session["Sucursal"];
            int correlativo = 1;

            try
            {
                //Recorremos el DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string codigo = row["codigo"].ToString();
                    string codigoe = row["codigoe"].ToString();
                    string producto = row["producto"].ToString();
                    string cantidad = row["cantidad"].ToString();
                    string costo = row["costo"].ToString();

                    //precio
                    if (codigo != "")
                    {
                        string cInstKar = "INSERT INTO kardexinven (id_codigo, id_agencia, fecha, id_movi, docto, serie, entrada, costo1, precio, correlativo, obs, hechopor) ";
                        cInstKar += "VALUES (";
                        cInstKar += codigo + ", ";                 //id_codigo
                        cInstKar += sucursal + ", ";               //id_agencia
                        cInstKar += "'" + oInv.fecha + "', ";      //fecha
                        cInstKar += oInv.movimiento + ", ";        //movimiento
                        cInstKar += oInv.nodocumento + ", ";       //no documento
                        cInstKar += "'" + oInv.serie + "', ";      //serie
                        cInstKar += cantidad + ", ";               //entrada
                        cInstKar += costo + ", ";                  //costo1
                        cInstKar += BuscarProducto(codigo) + ", "; //precio
                        cInstKar += correlativo + ", ";            //correlativo
                        cInstKar += "'" + oInv.concepto + "', ";   //obs
                        cInstKar += "'" + cUserConected + "');";   //hechopor

                        lError = insertar.ExecCommand(cInstKar, DB, ref cError);

                        correlativo++;

                        if (lError == true)
                        {
                            cMensaje = cError;
                            insertar.CerrarConexion();
                            str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                            return str;
                        }
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



        public ActionResult GenerarPDF()
        {
            //return new ActionAsPdf("EntradaKardex") { FileName = "Archivo.pdf" };
            return new Rotativa.ViewAsPdf("EntradaKardex");
        }



        [HttpPost]
        public string CargarSerie(Inventario oInv)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT serie1 FROM catkardex WHERE id_movi = " + oInv.movimiento;
                str = stringConexionMySql.Consulta(cInst, DB);
                
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string BuscarProducto(string id)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT precio1 FROM inventario WHERE id_codigo = " + id;
                str = stringConexionMySql.Consulta(cInst, DB);

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string BuscarCorrelativo(Inventario oInv)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT JSON_OBJECT('ID', IFNULL(MAX(a.docto)+1,1)) AS ID FROM kardexinven a WHERE serie = '" + oInv.serie + "';";

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