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
    public class CargaInvExcelController : Controller
    {
        // GET: CargaInvExcel
        public ActionResult CargaInvExcel()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return View();
        }




        [HttpPost]
        public string CargarInventario(string informacion)
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
                    string codigoe = row["codigoe"].ToString();
                    string ubicacion = row["ubicacion"].ToString();
                    string producto = row["producto"].ToString();
                    string marca = row["marca"].ToString();
                    string existencia = row["existencia"].ToString();
                    string ultimocosto = row["ultimocosto"].ToString();
                    string promediocosto = row["promediocosto"].ToString();
                    string precio1 = row["precio1"].ToString();
                    string precio2 = row["precio2"].ToString();
                    string stockminimo = row["stockminimo"].ToString();
                    string stockmaximo = row["stockmaximo"].ToString();
                    string bienservicio = row["bienservicio"].ToString();
                    string observaciones = row["observaciones"].ToString();

                    if (codigoe != "codigoe")
                    {

                        string cInstInv = "INSERT INTO inventario (codigoe, numero_departe, producto, linea, existencia, costo1, costo2, precio1, precio2, stockmin, stockmax, servicio, obs, id_marca) ";
                        cInstInv += "VALUES (";
                        cInstInv += "'" + codigoe + "', "; //codigo equivalente
                        cInstInv += "'" + ubicacion + "', "; //numero_departe = ubicacion
                        cInstInv += "'" + producto + "', "; //codigo equivalente
                        cInstInv += "'" + marca + "', "; //linea = marca
                        cInstInv += existencia + ", "; //codigo equivalente
                        cInstInv += ultimocosto + ", "; //codigo equivalente
                        cInstInv += promediocosto + ", "; //codigo equivalente
                        cInstInv += precio1 + ", "; //codigo equivalente
                        cInstInv += precio2 + ", "; //codigo equivalente
                        cInstInv += stockminimo + ", "; //codigo equivalente
                        cInstInv += stockmaximo + ", "; //codigo equivalente
                        cInstInv += "'" + bienservicio + "', "; //codigo equivalente
                        cInstInv += "'" + observaciones + "', "; //codigo equivalente
                        cInstInv += "'" + marca + "'); "; //linea = marca

                        lError = modificar.ExecCommand(cInstInv, DB, ref cError);

                        if (lError == true)
                        {
                            cMensaje = cError;
                            modificar.CerrarConexion();
                            str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                            return str;
                        }
                    }
                }


                string queryCatlineasi = "INSERT INTO catlineasi (id_linea, descripcion) SELECT linea, obs FROM inventario GROUP BY linea";
                lError = modificar.ExecCommand(queryCatlineasi, oDb, ref cError);

                string queryKardexInven = "INSERT INTO kardexinven (id_codigo,id_agencia,fecha,id_movi,docto,serie,entrada,costo1,costo2,correlativo,obs,hechopor) SELECT id_codigo,1,curdate(),1,'INVINI01','INV',existencia,costo1,costo2,id_codigo,'INGRESO DE INVENTARIO MASIVO','ADMIN' FROM inventario";
                lError = modificar.ExecCommand(queryKardexInven, oDb, ref cError);


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