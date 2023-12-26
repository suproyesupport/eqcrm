using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ReportDocumentsController : Controller
    {
        // GET: ReportDocuments
        public ActionResult ReportDocuments()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();          

            Encabezado_Documentos encabezado = new Encabezado_Documentos();

            List<Detalle_Documentos> detalle = new List<Detalle_Documentos>();

            string oDb = "server=localhost;Port=3306;user id=webadmin;password=Clave01*;persistsecurityinfo=True;database=proindesa";// (string)(Session["StringConexion"]);

            string cInst = "SELECT no_factura, fecha,id_cliente,cliente,id_vendedor, direccion,total FROM ordenserv";
            cInst += " WHERE no_factura = 1";

            lectura.EjecutarLectura(cInst, oDb);
            if (lectura.consulta.Read())
            {
                //encabezado.no_factura = lectura.consulta.GetString(0).ToString();
                encabezado.fecha = lectura.consulta.GetString(1).ToString();
                //encabezado.id_cliente = lectura.consulta.GetString(2).ToString();
                encabezado.cliente = lectura.consulta.GetString(3).ToString();
                encabezado.id_vendedor = lectura.consulta.GetString(4).ToString();
                encabezado.direccion = lectura.consulta.GetString(5).ToString();
                encabezado.total = lectura.consulta.GetString(6).ToString();
            }


            cInst = "SELECT id_codigo, obs, cantidad, precio, subtotal FROM detordenserv";
            cInst += " WHERE no_factura = 1";

            detalle = lectura.ObtieneDetalle(cInst, oDb, detalle);

            DocumentReport rpo = new DocumentReport();
         

            rpo.InitData(encabezado, detalle);
            rpo.CreateDocument();
            //rpo.ExportToHtml("c:\\PDf\\hola.html");
            rpo.ExportToPdf("c:\\PDf\\hola.pdf");
            //return View();
            return PartialView( rpo);
        }
    }
}