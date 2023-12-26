using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers {
    public class HomeController : Controller {

        public ActionResult Index()
        {
            string cUserConected = (string)(Session["Usuario"]);
            string cEmpresa = (string)(Session["cNombreEmisor"]);
            string cPanel1 = (string)(Session["cPanel1"]);
            string year = "";
            string oDb = (string)(Session["StringConexion"]);
            bool lError = false;
            string cError = "";
            string totalfactura = "";



            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string cInst = "";
                StringConexionMySQL consultar = new StringConexionMySQL();

                ViewBag.PaneWebl1 = cPanel1;

               
                cInst = "SELECT CURDATE()";
                year = consultar.CurdateYear(cInst, oDb);

                

                cInst = " SELECT FORMAT(SUM(a.total),2) AS TOTAL FROM facturas a";
                cInst += " WHERE a.fecha >= '" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2,'0') + "-01'";


                totalfactura = consultar.TotalFacturado(cInst, oDb);

                ViewData["VENTAS"] = totalfactura;

                //consultar.EjecutarLectura(cInst, oDb);
                //if (consultar.consulta.Read())
                //{
                //    ViewData["VENTAS"] = consultar.consulta[0].ToString();
                //}
                //else
                //{
                //    ViewData["VENTAS"] = "0.00";
                //}
                //consultar.CerrarConexion();

                //cInst = " SELECT a.id_vendedor AS ID_VENDEDOR,c.nombre AS VENDEDOR,FORMAT(SUM(b.subtotal),2) AS TOTAL";
                //cInst += " FROM facturas a LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                //cInst += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
                //cInst += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
                //cInst += " WHERE a.fecha >= '" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-01'";
                //cInst += " GROUP BY a.id_vendedor";


                cInst = " SELECT a.id_agencia AS ESTABLECIMIENTO,a.id_agencia AS NOMBRE_ESTABLECIMIENTO,FORMAT(SUM(b.subtotal),2) AS TOTAL";
                cInst += " FROM facturas a LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                cInst += " LEFT JOIN vendedores c ON(a.id_vendedor = c.id_codigo)";
                cInst += " LEFT JOIN inventario d ON(b.id_codigo = d.id_codigo)";
                cInst += " WHERE a.status<>'A' AND a.fecha >= '" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-01'";
                cInst += " GROUP BY a.id_agencia";

                

                 DataTable dt = new DataTable();
                



                string oGraph = consultar.GeneraGraficaBarras(cInst, dt, oDb).Replace(",]", "]").Replace("],", "]").Replace("']", "'],");
                ViewBag.Grafica = oGraph;


                cInst = " ALTER TABLE catkardex MODIFY COLUMN serie1 VARCHAR (20); ";
                lError = consultar.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {

                }


                cInst = " alter table inventario add column fotoapp varchar(500) DEFAULT '' ";
                lError = consultar.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {

                }
                                

                //consultar.CerrarConexion();

                ///consultar.KillAllMySQL(oDb);
                ///

               // ViewBag.Footer = "<span class=\"hidden-md-down fw-700\">"+year.Substring(0,4).ToString()+ " © EqSoftware by&nbsp;<a href = 'http://www.cgtecsa.com' class='text-primary fw-500' title='gotbootstrap.com' target='_blank'>https://www.cgtecsa.com</a> &nbsp;&nbsp;" + cEmpresa.ToUpper()+"</span>";
               // ViewBag.User = " USUARIO: " + cUserConected.ToUpper() ;
               // ViewData["User"] = "USUARIO CONECTADO:"+ cUserConected.ToString().ToUpper();



                ViewBag.Footer = "<span class=\"hidden-md-down fw-700\">" + year.Substring(0, 4).ToString() + " © EqSoftware by&nbsp;<a href = 'http://www.intec.com.gt/wp/' class='text-primary fw-500' title='gotbootstrap.com' target='_blank'>https://www.cgtecsa.com</a> &nbsp;&nbsp;" + cEmpresa.ToUpper() + "</span>";
                ViewBag.User = " USUARIO: " + cUserConected.ToUpper();
                ViewBag.Encabezado = cUserConected.ToUpper();
                ViewBag.Encabezado2 = cEmpresa.ToUpper();
                ViewData["User"] = "USUARIO CONECTADO:" + cUserConected.ToString().ToUpper();


                CargarPermiso();
                consultar.KillAllMySQL(oDb);
              

                return View();
            }
        }

        public ActionResult About()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewData["Message"] = "Quienes Somos";
                return View();
            }

        }


        [HttpPost]
        public void CargarPermiso()
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT lfsinexist FROM parametros";
                str = stringConexionMySql.Consulta(cInst, DB);
                this.Session["Permiso"] = (object)str;

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            
        }






    }
}