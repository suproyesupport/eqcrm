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
namespace EqCrm.Controllers
{

    [Route("api/[controller]")]
    public class AbcProductosController : Controller
    {
        public ActionResult AbcProductos(DataSourceLoadOptions loadOptions)
        {
           string cUserConected = (string)(Session["Usuario"]);
            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
            
        }


        [HttpPost]
        public object GetData(linea clinea)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
            string oDb = (string)(Session["StringConexion"]);
                StringConexionMySQL llenarGrid = new StringConexionMySQL();
                string cInst = "";
                //columnas = ["CODIGO", "CODIGOE", "PRODUCTO", "LINEA", "MARCA", "MEDIDA", "LINEAP", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"];
                if (clinea.lMonetario=="No")
                {
                    cInst = " SELECT d.id_codigo AS CODIGO , d.codigoe AS CODIGOE , d.producto AS PRODUCTO,d.linea AS LINEA,d.id_marca as MARCA, d.id_medida AS MEDIDA, d.lineac AS LINEAP,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 1, b.subtotal, 0)) AS ENERO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 2, b.subtotal, 0)) AS FEBRERO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 3, b.subtotal, 0)) AS MARZO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 4, b.subtotal, 0)) AS ABRIL,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 5, b.subtotal, 0)) AS MAYO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 6, b.subtotal, 0)) AS JUNIO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 7, b.subtotal, 0)) AS JULIO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 8, b.subtotal, 0)) AS AGOSTO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 9, b.subtotal, 0)) AS SEPTIEMBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 10, b.subtotal, 0)) AS OCTUBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 11, b.subtotal, 0)) AS NOVIEMBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 12, b.subtotal, 0)) AS DICIEMBRE";
                    cInst += " FROM facturas a";
                    cInst += " LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                    cInst += " LEFT JOIN vendedores  c ON(c.id_codigo = a.id_vendedor)";
                    cInst += " LEFT JOIN inventario  d ON(d.id_codigo = b.id_codigo)";
                    cInst += "WHERE a.status <> 'A' ";

                }
                else
                {
                    cInst = " SELECT d.id_codigo AS CODIGO , d.codigoe AS CODIGOE , d.producto AS PRODUCTO,d.linea AS LINEA,d.id_marca as MARCA, d.id_medida AS MEDIDA, d.lineac AS LINEAP,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 1, b.cantidad, 0)) AS ENERO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 2, b.cantidad, 0)) AS FEBRERO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 3, b.cantidad, 0)) AS MARZO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 4, b.cantidad, 0)) AS ABRIL,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 5, b.cantidad, 0)) AS MAYO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 6, b.cantidad, 0)) AS JUNIO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 7, b.cantidad, 0)) AS JULIO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 8, b.cantidad, 0)) AS AGOSTO,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 9, b.cantidad, 0)) AS SEPTIEMBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 10, b.cantidad, 0)) AS OCTUBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 11, b.cantidad, 0)) AS NOVIEMBRE,";
                    cInst += " SUM(IF(MONTH(a.fecha) = 12, b.cantidad, 0)) AS DICIEMBRE";
                    cInst += " FROM facturas a";
                    cInst += " LEFT JOIN detfacturas b ON(b.no_factura = a.no_factura and b.serie = a.serie and b.id_agencia = a.id_agencia)";
                    cInst += " LEFT JOIN vendedores  c ON(c.id_codigo = a.id_vendedor)";
                    cInst += " LEFT JOIN inventario  d ON(d.id_codigo = b.id_codigo)";
                    cInst += "WHERE a.status <> 'A' ";
                }

                if (System.String.IsNullOrEmpty(clinea.fecha1))
                {
                    //cInst += "AND a.fecha >= (SELECT CURDATE())";
                    cInst += "AND a.fecha >= '2019-01-01'";
                }
                else
                {
                    cInst += " AND a.fecha >= '"+clinea.fecha1+"'";
                }
                if (System.String.IsNullOrEmpty(clinea.fecha2))
                {
                    //cInst += " AND a.fecha <= (SELECT CURDATE())";
                    cInst += "AND a.fecha <= '2019-12-31'";
                }
                else
                {
                    cInst += " AND a.fecha <= '" + clinea.fecha2 + "'";
                }

                cInst += " GROUP BY d.id_codigo Order by d.producto";




            
                LlenarAbc.lista = llenarGrid.AbcProductos(cInst, oDb, LlenarAbc.lista);
                //llenarGrid.CerrarConexion();
            }
           

            //var json = JsonConvert.SerializeObject(LlenarAbc.lista);
            return JsonConvert.SerializeObject(LlenarAbc.lista);

           
        }

    }

    
}