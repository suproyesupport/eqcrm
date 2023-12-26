using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class InventarioTotalController : Controller
    {
        // GET: InventarioTotal
        public ActionResult PanelInventarioTotal()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

        [HttpPost]
        public object GetInventario()
        {
            string cUserConected = (string)(Session["Usuario"]);



            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string oDb = (string)(Session["StringConexion"]);
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];


                string cExistencia = " (select Round(sum( entrada-salida),2) FROM kardexinven b WHERE b.id_codigo = ";
                cExistencia += " a.id_codigo) as EXISTENCIA ";

                string cExis = " (select Round(sum( entrada-salida),2) FROM kardexinven b WHERE b.id_codigo = ";
                cExis += " a.id_codigo)  ";

                string cInst = "SELECT CONCAT(a.id_codigo,' ','') AS DATOS, a.id_codigo AS CODIGO,a.producto AS PRODUCTO,a.umedida AS UMEDIDA," + cExistencia + ",a.precio1 AS PRECIO1,a.costo2 AS COSTO,sum(a.costo2*" + cExis + ") AS COSTOT FROM inventario a  group by a.id_codigo ORDER BY a.id_codigo ";
                             

                LlenarListaInventario.lista = llenar.ListaIventario(cInst, DB, LlenarListaInventario.lista);

            }

            return JsonConvert.SerializeObject(LlenarListaInventario.lista);
        }
    }
}