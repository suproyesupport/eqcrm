using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace EqCrm.Controllers
{
    public class CxcEnvioEstcueController : Controller
    {
        // GET: CxcEnvioEstcue
        public ActionResult CxcEnvioEstcue()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            DataTable dt = new DataTable();

            string cSql = "SELECT a.id_codigo AS CODIGO,a.cliente AS CLIENTE,a.nit AS NIT ,a.direccion AS DIRECCION ,a.email AS EMAIL, FORMAT((select sum(z.importe) from ctacc z where a.id_codigo = z.id_codigo)-(select sum(y.importe) from ctaca y where a.id_codigo = y.id_codigo),2) as SALDO from clientes a having SALDO >= 0.01";

            cSql = "SELECT a.id_codigo AS CODIGO,a.cliente AS CLIENTE,a.nit AS NIT ,a.direccion AS DIRECCION ,'soporte@suproye.com,jponce@suproye.com,operaciones@cgtecsa.com' AS EMAIL, FORMAT((select sum(z.importe) from ctacc z where a.id_codigo = z.id_codigo)-(select sum(y.importe) from ctaca y where a.id_codigo = y.id_codigo),2) as SALDO from clientes a having SALDO >= 0.01";

            string cxcClie = stringConexionMySql.LlenarDTTableHTmlcxc(cSql, dt, DB);
            ViewBag.cxcClientes = cxcClie;



            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }

        [HttpPost]
        public string CargarEnvio(string informacion)
        {
            
            string str = "";
            string cInstcargos = "";

            //Convertimos el JSON en un DataTable
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(informacion, (typeof(DataTable)));

            string oDb = (string)(Session["StringConexion"]);
            ///StringConexionMySQL modificar = new StringConexionMySQL();
           
            

            try
            {
                //Recorremos el DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string codigo = row["codigo"].ToString();

                    cInstcargos = "SELECT serie AS SERIE, docto AS DOCTO,fechaa AS FECHAA,fechap AS FECHAP,importe AS IMPORTE,0 AS ABONO,saldo AS SALDO FROM ctacc WHERE  saldo >= 0.01 AND id_codigo = " + codigo.ToString();

                    if (codigo != "")
                    {
                        using (IDbConnection dbConnection = new MySqlConnection(oDb))
                        {

                            IEnumerable<ctacc> cargos = dbConnection.Query<ctacc>(cInstcargos);

                            // Iterar a través de los resultados
                            foreach (var cxc in cargos)
                            {

                            }
                        }
                    }

                }


                

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            
            return str;
        }

    }
}