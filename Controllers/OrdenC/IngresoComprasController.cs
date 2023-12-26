using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Xml;

namespace EqCrm.Controllers.OrdenC
{
    public class IngresoComprasController : Controller
    {
        // GET: IngresoCompras
        public ActionResult IngresoCompras()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string Base_Datos = (string)(Session["StringConexion"]);
            StringConexionMySQL mysql = new StringConexionMySQL();


            DataTable dt = new DataTable();
            DataTable dtInventario = new DataTable();

            string cInst = "SELECT id_codigo AS CODIGO, cliente AS PROVEEDOR, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO FROM proveedores ";
            ViewBag.Tabla = mysql.LlenarDTTableHTmlClientePOS(cInst, dt, Base_Datos);

            cInst = "SELECT id_codigo AS CODIGO, producto AS PRODUCTO, precio1 AS PRECIO, existencia as EXISTENCIA FROM inventario ";
            ViewBag.TablaInventario = mysql.LlenarDTTableHTmlInventarioPOS(cInst, dtInventario, Base_Datos);

            return (ActionResult)this.View();
        }
        [HttpPost]
        public string InsertarDocumento(DatosProspecto datos)
        {
            int formaPago = datos.formaPago;
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string codigo = "";
            string codigoe = "";
            string producto = "";
            string cantidad = "";
            string precio = "";
            string descto = "";
            string subtotal = "";
            string path = "";
            double nTotal = 0.00;
            double nDescto = 0.00;
            string cMensaje = "";
            string cInst = "";
            string DB = (string)this.Session["StringConexion"];
            string _BaseDatos = (string)this.Session["oBase"];

            
            bool lError = false;
            string cError = "Hubo un error en el ingreso, favor revisar...";
            int nContador = 1;
            
            
            

            string cDtalle = datos.cdetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cDtalle);


            XmlNodeList cXmlDetalle = xmlDoc.GetElementsByTagName("DETALLE");

            /// vamos a sumar el total
            /// 
            foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
            {
                try
                {
                    nDescto = nDescto + Convert.ToDouble(node.ChildNodes[5].InnerText);
                }
                catch
                {
                    nDescto = nDescto + 0;
                }
                nTotal = nTotal + Convert.ToDouble(node.ChildNodes[6].InnerText);

            }

            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList no_factura = xmlDoc.GetElementsByTagName("NODOCTO");
            XmlNodeList serie = xmlDoc.GetElementsByTagName("SERIE");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList tipoVenta = xmlDoc.GetElementsByTagName("TIPOVENTA");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");


            /// aca vamos hacer que ingrese una factura normal comun y corriente

            if (tipodocto[0].InnerText == "COMPRA")
            {


                stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);

                cInst = "INSERT INTO compras (no_factura, serie, fecha, status, id_provee, proveedor,direccion,total,obs,id_agencia,nit,fechap,dias_cred) ";

                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ",

                    no_factura[0].InnerText,
                    serie[0].InnerText,
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,                   
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    1,
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }




                

                // vamos a insertar el detalle de la factura y kardex y denas
                // 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                   

                    cInst = " INSERT INTO detcompras (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", no_factura[0].InnerText, serie[0].InnerText, codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1);

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }



                // vamos a insertar el detalle del kardex
                // 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;


                    
                    cInst = "INSERT INTO kardexinven ";
                    cInst += "( id_codigo, id_agencia, fecha, id_movi, docto, serie, obs, hechopor, entrada, costo1,precio, codigoemp,correlativo ) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "2", no_factura[0].InnerText, serie[0].InnerText, "Ingresado en modulo sistema Web", "Web", cantidad.ToString().Replace(",", ""), precio.ToString().Replace(",", ""), precio.ToString().Replace(",", ""), id_cliente[0].InnerText, nContador.ToString());

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }

                              

                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                stringConexionMySql.CerrarConexion();

                

                cMensaje = "DOCUMENTO CREADO CON EXITO";
            }

            return cMensaje;
        }
    }
}