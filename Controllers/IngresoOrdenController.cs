using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Xml;
using System.IO;
using System.Data;

namespace EqCrm.Controllers
{
    public class IngresoOrdenController : Controller
    {
        // GET: IngresoOrden
        public ActionResult IngresoOrden()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
                        
            string Base_Datos = (string)(Session["StringConexion"]);
            StringConexionMySQL mysql = new StringConexionMySQL();


            DataTable dt = new DataTable();
            DataTable dtInventario = new DataTable();

            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO FROM clientes ";
            ViewBag.Tabla = mysql.LlenarDTTableHTmlClientePOS(cInst, dt, Base_Datos);

            cInst = "SELECT id_codigo AS CODIGO, producto AS PRODUCTO, precio1 AS PRECIO, existencia as EXISTENCIA FROM inventario ";
            ViewBag.TablaInventario = mysql.LlenarDTTableHTmlInventarioPOS(cInst, dtInventario, Base_Datos);

            
            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = "select id_depto,nombre from catdeptosconta";
            mysql.LLenarDropDownList(sentenciaSQL, Base_Datos, lineas);
            ViewData["Lineas"] = lineas;


            mysql.CerrarConexion();
            return (ActionResult)this.View();


            
        }

        [HttpPost]
        public string GetDataImages(Inventario oInven)
        {
            // StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";

            string path = Server.MapPath("~/images/Pedidos/" + "/" + oInven.id_codigo.Trim() + "/");

            string[] Carpeta;

            cResultado = "[";

            Carpeta = Funciones.leeCarpeta(path);

            foreach (string dir in Carpeta)
            {
                cResultado = cResultado + "\"/images/Pedidos/" + oInven.id_codigo.Trim() + "/" + dir.Replace(path, "").ToString() + "\",";
            }

            cResultado = cResultado + "];";
            cResultado = cResultado.Replace(",]", "]");


            return cResultado;
        }



        [HttpPost]
        public ActionResult SaveDropzoneJsUploadedFiles()
        {

            try
            {
                var id_codigo = Request.Params;
                var codigo = id_codigo[0].ToString();
                string path = Server.MapPath("~/Images/Pedidos/" + "/" + codigo + "/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }


                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase fileUpload = Request.Files[fileName];

                    //You can Save the file content here

                    fileUpload.SaveAs(path + Path.GetFileName(fileUpload.FileName));
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

            return Json(new { Message = "Archivo Guardado con Exito." });

        }

        [Route("/{id}")]
        public ActionResult VerOrden(string id)
        {
            string cDecodifica = Funciones.Base64Decode(id);            

            string[] strIdInterno = cDecodifica.Split('|');



            string no_orden = strIdInterno[0].ToString();
            string oDb = strIdInterno[1].ToString();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            ViewData["ORDEN"] = no_orden.ToString();
            ViewData["FIRMA"] = id.ToString();

            string cInst = "SELECT fecha,requirente,obs,(select nombre from catdeptosconta where id_depto = id_provee),direccion,status FROM ordcompras ";
                   cInst += "WHERE no_factura =" + no_orden.ToString();


            stringConexionMySql.EjecutarLectura(cInst, oDb);
            if (stringConexionMySql.consulta.HasRows)
            {
                while (stringConexionMySql.consulta.Read())
                {
                    if (stringConexionMySql.consulta.GetString(5) == "P")
                    {
                        ViewData["AUTORIZADA"] = "AUTORIZADA";
                    }
                    else
                    {
                        ViewData["AUTORIZADA"] = "";
                    }
                    ViewData["DEPTO"] = stringConexionMySql.consulta.GetString(3);
                    ViewData["FECHA"] = stringConexionMySql.consulta.GetString(0);
                    ViewData["REQUIRENTE"] = stringConexionMySql.consulta.GetString(1);
                    ViewData["USO"] = stringConexionMySql.consulta.GetString(2);
                    ViewData["CORREO"] = stringConexionMySql.consulta.GetString(4);

                }


            }

            DataTable dt = new DataTable();
            
            cInst = "SELECT a.no_cor as ITEM,a.id_codigo as CODIGO,(SELECT b.codigoe FROM inventario b WHERE b.id_codigo = a.id_codigo) AS CODIGOE, obs AS PRODUCTO, cantidad AS CANTIDAD FROM detordcompras a ";
            cInst += "WHERE no_factura =" + no_orden.ToString();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlN(cInst, dt, oDb);
           

            stringConexionMySql.CerrarConexion();


            

            return (ActionResult)this.View();

        }

        [HttpPost]
        public string AutorizarOrden(string id)
        {
            string cDecodifica = Funciones.Base64Decode(id);
            Funciones f = new Funciones();
            string[] strIdInterno = cDecodifica.Split('|');

            bool lError = false;
            string cMensaje = "";
            string cError = "";
            string str = "";
            string no_orden = strIdInterno[0].ToString();
            string oDb = strIdInterno[1].ToString();
            string cRequi = "";
            string cTo = "";

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            try
            {

                string cInst = "UPDATE ordcompras SET status ='P' ";
                cInst += "WHERE no_factura =" + no_orden.ToString();

                lError = stringConexionMySql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }


                

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"ORDEN AUTORIZADA EN PROCESO \", \"PRECIO\": 0}";

            cMensaje = "no_factura =" + no_orden.ToString();
            cRequi = f.ObtieneDatos("ordcompras", "requirente", cMensaje.ToString(), oDb);
            cTo = f.ObtieneDatos("parametros", "cTo", "", oDb);

              f.CrearBitacoraOS(no_orden.ToString(), "", "P", DateTime.Now.ToString("MM/dd/yyyy H:mm").ToString(), "ORDEN AUTORIZADA", "1", oDb);
              f.EnviarCorreoProceso("Autorizaciones", "AUTORIZACIONES",cTo, no_orden, oDb);

            stringConexionMySql.CerrarConexion();
            return str;

        }


        [HttpPost]
        public string AsignarProductoOrden(string id, string rcp)
        {
            
            Funciones f = new Funciones();

            
            bool lError = false;
            string cMensaje = "";
            string cError = "";
            string str = "";
            string no_orden = id.ToString();
            string oDb = (string)(Session["StringConexion"]);
            string cRequi = "";
            string cTo = "";

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            try
            {
                string cInst = " INSERT INTO kardexinven(obs,id_codigo, id_agencia, fecha, id_movi, docto, serie, entrada, costo1, precio, hechopor, correlativo)";
                       cInst +=" select 'Ingresado en Modulo de Ordenes Web',id_codigo, "+rcp.ToString()+",'"+ DateTime.Now.ToString("yyyy/MM/dd").ToString()+"',7,no_factura,serie,cantidad,costo,precio,'"+ (string)this.Session["Usuario"].ToString()+"',no_cor FROM detordcompras WHERE no_factura =" + no_orden.ToString();

                lError = stringConexionMySql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }


                cInst = " INSERT INTO kardexinven(obs,id_codigo, id_agencia, fecha, id_movi, docto, serie, salida, costo1, precio, hechopor, correlativo)";
                cInst += " select 'Ingresado en Modulo de Ordenes Web',id_codigo, 1,'" + DateTime.Now.ToString("yyyy/MM/dd").ToString() + "',51,no_factura,serie,cantidad,costo,precio,'" + (string)this.Session["Usuario"].ToString() + "',no_cor FROM detordcompras WHERE no_factura =" + no_orden.ToString();

                lError = stringConexionMySql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }

                cInst = "UPDATE ordcompras SET status ='E' ";
                cInst += "WHERE no_factura =" + no_orden.ToString();

                lError = stringConexionMySql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                                    
                }
                
                f.CrearBitacoraOS(no_orden.ToString(), "", "P", DateTime.Now.ToString("MM/dd/yyyy H:mm").ToString(), "ORDEN ENTREGADA", "1", oDb);

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"SOLICITUD ASIGNADA CON EXITO \", \"PRECIO\": 0}";

            
            stringConexionMySql.CerrarConexion();
            return str;

        }

        [HttpPost]
        public string InsertarDocumento(DatosProspecto datos)
        {
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string cInst = "";
            string cMensaje = "";
            
            string DB = (string)this.Session["StringConexion"];
            string _BaseDatos = (string)this.Session["oBase"];
            string cUser = (string)(Session["Usuario"]);
            string cError = "";
            int nDocumento = 0;
            bool lError = false;

            int nContador = 0;

            string codigo = "";
            
            string producto = "";
            string cantidad = "";

            string cDtalle = datos.cdetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cDtalle);


            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList requirente = xmlDoc.GetElementsByTagName("NOMBRESOLICITA");
            XmlNodeList uso = xmlDoc.GetElementsByTagName("USO");
            XmlNodeList depto = xmlDoc.GetElementsByTagName("AREA");
            XmlNodeList email = xmlDoc.GetElementsByTagName("EMAIL");
            

            cInst = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
            cInst += String.Format("FROM {0} ", "ordcompras");
            cInst += "where id_agencia = 1 and serie='S'";

            stringConexionMySql.EjecutarLectura(cInst, DB);
            if (stringConexionMySql.consulta.Read())
            {
                nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
            }
            // Cerramos la consulta
            stringConexionMySql.CerrarConexion();


            stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);

            cInst = "INSERT INTO ordcompras (no_factura, serie, fecha, status, id_provee, requirente,obs,id_agencia,direccion) ";
            cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') ",
                nDocumento.ToString(),
                "S",
                fechadocto[0].InnerText,
                "I",
                depto[0].InnerText,
                requirente[0].InnerText,
                uso[0].InnerText,                
                1,
                email[0].InnerText );

            lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

            if (lError == true)
            {
                cMensaje = cError;
                stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                stringConexionMySql.CerrarConexion();
                return cMensaje;

            }
                        

            XmlNodeList cXmlDetalle = xmlDoc.GetElementsByTagName("DETALLE");

            /// vamos a sumar el total


           
            /// 
            foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
            {
                codigo = node.ChildNodes[0].InnerText;                
                producto = node.ChildNodes[3].InnerText;
                cantidad = node.ChildNodes[2].InnerText;
                

                cInst = " INSERT INTO detordcompras (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), "S", codigo.ToString(), cantidad.ToString(), 0, 0, nContador.ToString(), 0, producto.ToString(), 1);

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {

                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cError;

                }



                nContador++;

            }

            stringConexionMySql.ExecAnotherCommand("COMMIT", DB);

            

            cMensaje = "DOCUMENTO CREADO CON EXITO";

            f.EnviarCorreoOS(cUser, requirente[0].InnerText.ToString(), email[0].InnerText.ToString(), nDocumento.ToString(),DB);
            f.CrearBitacoraOS(nDocumento.ToString(), "", "I", DateTime.Now.ToString("MM/dd/yyyy H:mm").ToString(), "ORDEN INGRESADA " + requirente[0].InnerText, "1", DB);
            return cMensaje;
        }

        [Route("/{id}")]
        public ActionResult AsignaOrden(string id)
        {
            

            string[] strIdInterno = id.Split('|');

            string oDb = (string)this.Session["StringConexion"];
            
            string cUser = (string)(Session["Usuario"]);

            string no_orden = strIdInterno[0].ToString();
            
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            List<SelectListItem> listadoasesores = new List<SelectListItem>();
            string cInst = "SELECT id_codigo,nombre FROM vendedores";
            stringConexionMySql.LLenarDropDownList(cInst, oDb, listadoasesores);
            ViewData["asesores"] = listadoasesores;

            ViewData["ORDEN"] = no_orden.ToString();
            ViewData["FIRMA"] = id.ToString();

            cInst = "SELECT fecha,requirente,obs,(select nombre from catdeptosconta where id_depto = id_provee),direccion,status FROM ordcompras ";
            cInst += "WHERE no_factura =" + no_orden.ToString();


            stringConexionMySql.EjecutarLectura(cInst, oDb);
            if (stringConexionMySql.consulta.HasRows)
            {
                while (stringConexionMySql.consulta.Read())
                {
                    if (stringConexionMySql.consulta.GetString(5) == "P")
                    {
                        ViewData["AUTORIZADA"] = "AUTORIZADA";
                    }
                    else
                    {
                        ViewData["AUTORIZADA"] = "";
                    }
                    ViewData["DEPTO"] = stringConexionMySql.consulta.GetString(3);
                    ViewData["FECHA"] = stringConexionMySql.consulta.GetString(0);
                    ViewData["REQUIRENTE"] = stringConexionMySql.consulta.GetString(1);
                    ViewData["USO"] = stringConexionMySql.consulta.GetString(2);
                    ViewData["CORREO"] = stringConexionMySql.consulta.GetString(4);

                }


            }

            DataTable dt = new DataTable();

            cInst = "SELECT a.no_cor as ITEM,a.id_codigo as CODIGO,(SELECT b.codigoe FROM inventario b WHERE b.id_codigo = a.id_codigo) AS CODIGOE, obs AS PRODUCTO, cantidad AS CANTIDAD FROM detordcompras a ";
            cInst += "WHERE no_factura =" + no_orden.ToString();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlN(cInst, dt, oDb);


            stringConexionMySql.CerrarConexion();




            return (ActionResult)this.View();

        }

        

    }
}