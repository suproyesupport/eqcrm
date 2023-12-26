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

namespace EqCrm.Controllers
{
    public class IngresoAbonosController : Controller
    {
        // GET: IngresoAbonos
        public ActionResult IngresoAbonos()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> listcodigoabono = new List<SelectListItem>();
            string sentenciaSQLCodigoAbonos = "SELECT id_movi, nombre FROM catctac WHERE cargo_abono = 'A'";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLCodigoAbonos, DB, listcodigoabono);
            ViewData["CodigoAbono"] = listcodigoabono;

            List<SelectListItem> listcobradores = new List<SelectListItem>();
            string sentenciaSQLCobradores = "SELECT id_codigo, nombre FROM cobradores";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLCobradores, DB, listcobradores);
            ViewData["Cobradores"] = listcobradores;

            List<SelectListItem> listtipodepago = new List<SelectListItem>();
            string sentenciaSQLTipodePago = "SELECT id_tipopago, descripcion FROM cattipodepago";
            stringConexionMySql.LLenarDropDownList(sentenciaSQLTipodePago, DB, listtipodepago);
            ViewData["TiposdePago"] = listtipodepago;


            DataTable dt = new DataTable();

            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO,telefono AS TELEFONO FROM clientes ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteCXC(cInst, dt, DB);

            //ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlCliente(cInst, dt, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }


        [HttpPost]
        public string ConsultaCtcca(string idcliente)
        {
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);

            string instruccionSQL = "SELECT docto as DOCTO, serie as SERIE, tipodocto as TIPODOCTO, DATE_FORMAT(fechaa,' %m-%d-%Y') AS FECHAA, FORMAT(importe, 2) AS IMPORTEORIGINAL, FORMAT(saldo, 2) AS SALDOACTUAL, 0.00 AS ABONO ";
            instruccionSQL += "FROM ctacc ";
            instruccionSQL += "WHERE id_codigo = " + idcliente + " ";
            instruccionSQL += "AND saldo > 0;";

            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            DataTable dtc = new DataTable();

            return ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlCXC(instruccionSQL, dtc, DB);
        }


        [HttpPost]
        public string AbonarCuentasxCobrar(string informacion, Models.Cxcca oCxcca)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //Convertimos el JSON en un DataTable
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(informacion, (typeof(DataTable)));

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL modificar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                //Recorremos el DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string docto = row["docto"].ToString();
                    string serie = row["serie"].ToString();
                    string importe = row["importeoriginal"].ToString();
                    string saldoactual = row["saldoactual"].ToString();
                    string abono = row["abono"].ToString();

                    if (saldoactual != "" && abono != "")
                    {
                        double saldoact = 0;
                        double abo = 0;

                        double.TryParse(saldoactual, out saldoact);
                        double.TryParse(abono, out abo);
                        double resta = (saldoact - abo);

                        if (abo != 0 && saldoact >= abo)
                        {
                            //ultimo
                            string cInst = "UPDATE ctacc SET ";
                            cInst += "saldo = " + resta + " ";
                            cInst += "WHERE docto = " + docto + ";";
                            lError = modificar.ExecCommand(cInst, DB, ref cError);

                            string cInstCtaca = "INSERT INTO ctaca (id_codigo, docto, serie, id_movi, docto_r, serie_r, fechaa, fechai, importe, obs, hechopor, tipodocto, ";
                            cInstCtaca += "cobrador, id_agencia, forma_pago, monto_pago, id_caja, cortecaja, uuidcaja) ";
                            cInstCtaca += "VALUES (";
                            cInstCtaca += oCxcca.idcliente + ", "; //id_cliente
                            cInstCtaca += oCxcca.nodocumento + ", "; //docto
                            cInstCtaca += "'" + oCxcca.serie + "', "; //serie
                            cInstCtaca += oCxcca.codabono + ", "; //id_movi
                            cInstCtaca += docto + ", "; // docto_r
                            cInstCtaca += "'" + serie + "', "; //serie
                            cInstCtaca += "'" + oCxcca.fechaa + "', ";//fechaa
                            cInstCtaca += "'" + oCxcca.fechai + "', ";//fechai
                            cInstCtaca += "'" + importe + "', ";//importe
                            cInstCtaca += "'" + oCxcca.concepto + "', ";//obs
                            cInstCtaca += "'" + this.Session["Usuario"] + "', "; //hechopor
                            cInstCtaca += "'" + oCxcca.tipodocto + "', ";//tipodocto
                            cInstCtaca += "'" + oCxcca.cobradores + "', ";// cobrador
                            cInstCtaca += 1 + ", ";//idagencia
                            cInstCtaca += "'" + oCxcca.formapago + "', "; //forma_pago
                            cInstCtaca += abo + ", "; //monto_pago
                            cInstCtaca += 0 + ", ";//id_caja
                            cInstCtaca += 0 + ", ";//cortecaja
                            cInstCtaca += "'" + "0" + "'); ";//uuidcaja

                            lError = modificar.ExecCommand(cInstCtaca, DB, ref cError);

                            if (lError == true)
                            {
                                cMensaje = cError;
                                modificar.CerrarConexion();
                                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                                return str;
                            }
                        } else if (saldoact < abo)
                        {
                            
                        }
                    }
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



        [HttpPost]
        public string GetDataCliente(Models.Cxcca oCxcca)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'CLIENTE', cliente) " + "FROM clientes " + "WHERE id_codigo = " + oCxcca.idcliente;

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                }

                else
                {
                    sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'CLIENTE', cliente) " + "FROM clientes " + "WHERE id_codigo = " + oCxcca.idcliente;
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








    }
}

