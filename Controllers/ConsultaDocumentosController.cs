using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ConsultaDocumentosController : Controller
    {
        // GET: ConsultaDocumentos
        public ActionResult ConsultaDocumentos()
        {

            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            List<SelectListItem> lista1 = new List<SelectListItem>();
            stringConexionMySql.LLenarDropDownList("SELECT id_codigo,nombre FROM vendedores", DB, lista1);
            this.ViewData["asesores"] = (object)lista1;

            List<SelectListItem> lista2 = new List<SelectListItem>();
            stringConexionMySql.LLenarDropDownList("SELECT id_agencia,nombre FROM catagencias", DB, lista2);
            this.ViewData["sucursal"] = (object)lista2;

            return View();
        }

        [HttpPost]
        public object GetDocumentos(DoctosConsulta oFactu)
        {
            string cUserConected = (string)(Session["Usuario"]);
           
          

            if (string.IsNullOrEmpty(oFactu.fecha1))
            {
                oFactu.fecha1 = "";
            }

            if (string.IsNullOrEmpty(oFactu.fecha2))
            {
                oFactu.fecha2 = "";
            }

            if (string.IsNullOrEmpty(oFactu.id_agencia))
            {
                oFactu.id_agencia = "";
            }

            if (string.IsNullOrEmpty(oFactu.id_vendedor))
            {
                oFactu.id_vendedor = "";
            }

            if (string.IsNullOrEmpty(oFactu.nit))
            {
                oFactu.nit = "";
            }
            if (string.IsNullOrEmpty(oFactu.uuid))
            {
                oFactu.uuid= "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
               
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];


                string cInst = "SELECT a.no_factura AS IDINTERNO, a.serie AS SERIEINTERNA, a.firmaelectronica AS UUID, a.no_docto_fel AS NO_DOCTO, a.serie_docto_fel AS SERIE, a.fecha AS FECHA, a.status as STATUS,a.nit as NIT,a.id_cliente as IDCLIENTE, a.cliente AS CLIENTE ,b.nombre as ASESOR, a.total as TOTAL, a.id_agencia AS AGENCIA, a.obs AS OBS ";
                cInst += " FROM facturas a";
                cInst += " LEFT JOIN vendedores b ON (a.id_vendedor = b.id_codigo)";

                cInst += " WHERE a.status != 'B'";

                if (oFactu.fecha1 != "")
                {
                    
                    cInst += " AND a.fecha >= '" + oFactu.fecha1 + "'";
                }

                if (oFactu.fecha2 != "")
                {
                   
                    cInst += " AND a.fecha <= '" + oFactu.fecha2 + "'";
                }




                LlenarListaDocumentos.listaDocumentos = llenar.ListaDocumentos(cInst, DB, LlenarListaDocumentos.listaDocumentos);
               
            }

            return JsonConvert.SerializeObject(LlenarListaDocumentos.listaDocumentos);
        }

    }

}




//SELECT no_factura AS IDINTERNO, serie AS SERIEINTERNA, firmaelectronica AS UUID, no_docto_fel AS NO_DOCTO, serie_docto_fel AS SERIE, fecha AS FECHA, status as STATUS,nit as NIT,id_cliente as IDCLIENTE, cliente AS CLIENTE ,id_vendedor as ASESOR, total as TOTAL, id_agencia AS AGENCIA, obs AS OBS
//FROM facturas



//SELECT no_factura AS IDINTERNO, serie AS SERIEINTERNA, firmaelectronica AS UUID, no_docto_fel AS NO_FACTURA, serie_docto_fel AS SERIE, fecha AS FECHA, status as STATUS,nit as NIT,id_cliente as IDCLIENTE, cliente AS CLIENTE ,id_vendedor as VENDE, total as TOTAL, id_agencia AS AGENCIA, obs AS OBS
//FROM notadecredito ORDER BY fecha DESC


//SELECT no_factura AS IDINTERNO, serie AS SERIEINTERNA, '' AS UUID, '' AS NO_FACTURA, ''  AS SERIE, fecha AS FECHA, status as STATUS,nit as NIT,id_cliente as IDCLIENTE, cliente AS CLIENTE ,id_vendedor as VENDE, total as TOTAL, id_agencia AS AGENCIA, obs AS OBS
//FROM envios ORDER BY fecha DESC

//SELECT no_factura AS IDINTERNO, serie AS SERIEINTERNA, '' AS UUID, '' AS NO_FACTURA, ''  AS SERIE, fecha AS FECHA, status as STATUS,nit as NIT,id_cliente as IDCLIENTE, cliente AS CLIENTE ,id_vendedor as VENDE, total as TOTAL, id_agencia AS AGENCIA, obs AS OBS
//FROM pedidos ORDER BY fecha DESC

//SELECT no_factura AS IDINTERNO, serie AS SERIEINTERNA, '' AS UUID, '' AS NO_FACTURA, ''  AS SERIE, fecha AS FECHA, status as STATUS,nit as NIT,id_cliente as IDCLIENTE, cliente AS CLIENTE ,id_vendedor as VENDE, total as TOTAL, id_agencia AS AGENCIA, obs AS OBS
//FROM proformas ORDER BY fecha DESC