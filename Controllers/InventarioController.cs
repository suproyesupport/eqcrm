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

namespace EqCrm.Controllers
{
    public class InventarioController : Controller
    {
        public ActionResult Inventario()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            List<SelectListItem> lineas = new List<SelectListItem>();
            string sentenciaSQL = "select id_linea,descripcion from catlineasi";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL, DB, lineas);
            ViewData["Lineas"] = lineas;


            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }


        [HttpPost]
        public object GetInventarioo(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);


            if (string.IsNullOrEmpty(oInven.id_codigo))
            {
                oInven.id_codigo = "";
            }

            if (string.IsNullOrEmpty(oInven.codigoe))
            {
                oInven.codigoe = "";
            }

            if (string.IsNullOrEmpty(oInven.linea))
            {
                oInven.linea = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string oDb = (string)(Session["StringConexion"]);
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];


                string cInst = "SELECT  CONCAT(a.id_codigo,' ','') AS DATOS,a.id_codigo as CODIGO, a.codigoe AS CODIGOE, a.numero_departe AS ALIAS, a.producto AS PRODUCTO,a.linea AS LINEA,ifnull(sum(b.entrada-b.salida),0) AS EXISTENCIA,a.precio1 AS PRECIO1,a.precio2 AS PRECIO2,a.precio3 AS PRECIO3,a.precio4 AS PRECIO4, a.costo1 AS PRECIO1, a.costo2 AS COSTO2, a.OBS ";
                cInst += " FROM inventario a ";
                cInst += " LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) ";

                cInst += " WHERE status = 'A' ";

                if (oInven.id_codigo.ToString() != "")
                {
                    cInst += " AND a.id_codigo  = " + oInven.id_codigo.ToString();
                }

                if (oInven.codigoe.ToString() != "")
                {
                    cInst += " AND a.codigoe = '" + oInven.codigoe.ToString() + "'";
                }

                if (oInven.linea.ToString() != "")
                {
                    cInst += " AND a.linea  = '" + oInven.linea.ToString() + "'";
                }

                cInst += " GROUP BY a.id_codigo ";

                LlenarListaInventario.lista = llenar.ListaIventario(cInst, DB, LlenarListaInventario.lista);
               
            }

            return JsonConvert.SerializeObject(LlenarListaInventario.lista);
        }







        [HttpPost]
        public object GetListInventario(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            dapperConnect dapper = new dapperConnect();
            string DB = (string)this.Session["StringConexion"];

            string query = "SELECT CONCAT(a.id_codigo,' ','') AS DATOS, a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.numero_departe AS ALIAS, a.producto AS PRODUCTO, " +
                "a.linea AS LINEA, IFNULL(sum(b.entrada-b.salida),0) AS EXISTENCIA, a.precio1 AS PRECIO1,a.precio2 AS PRECIO2, a.precio3 AS PRECIO3, a.precio4 AS PRECIO4, " +
                "a.costo1 AS COSTO1, a.costo2 AS COSTO2, a.OBS " +
                "FROM inventario a " +
                "LEFT JOIN kardexinven b ON (a.id_codigo = b.id_codigo) " +
                "WHERE status = 'A' ";

            if (!string.IsNullOrEmpty(oInven.id_codigo))
            {
                query += "AND a.id_codigo  = " + oInven.id_codigo.ToString() + " ";
            }

            if (!string.IsNullOrEmpty(oInven.codigoe))
            {
                query += "AND a.codigoe = '" + oInven.codigoe.ToString() + "' ";
            }

            if (!string.IsNullOrEmpty(oInven.linea))
            {
                query += "AND a.linea  = '" + oInven.linea.ToString() + "' ";
            }

            query += "GROUP BY a.id_codigo;";

            var resultado = dapper.ExecuteList<dynamic>(DB, query);

            string json = JsonConvert.SerializeObject(resultado);

            return json;

        }





















        [HttpPost]
        public string GetDataExistenciaAgencias(Inventario oInven)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";



            instruccionSQL = "SELECT b.nombre as BODEGA, sum(a.entrada - a.salida) AS EXISTENCIA FROM kardexinven a INNER JOIN catagencias b on(a.id_agencia = b.id_agencia) WHERE a.id_codigo = " + oInven.id_codigo;
            instruccionSQL += " GROUP BY a.id_agencia";



            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;
        }



        [HttpPost]
        public string GetDataImages(Inventario oInven)
        {

            string Empresa = Session["oBase"].ToString();
                      


             DataTable dt = new DataTable();
            string cResultado = "";

            string path = Server.MapPath("~/Images/Inventario/" + Empresa.Trim() + "/" + oInven.id_codigo.Trim() + "/");
                
            string[] Carpeta;

            cResultado = "[";

            Carpeta = Funciones.leeCarpeta(path);

            foreach (string dir in Carpeta)
            {
                cResultado = cResultado + "\"/images/Inventario/" + Empresa.Trim() + "/"+ oInven.id_codigo.Trim() + "/" + dir.Replace(path, "").ToString() + "\",";
            }

            cResultado = cResultado + "];";
            cResultado = cResultado.Replace(",]", "]");


            return cResultado;
        }

        [Route("/{id}")]
        [HttpPost]
        public JsonResult GetDataFichaTecnica(string id)
        {
            try
            {
                string codigo_inventario = id;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string oDb = (string)(Session["oBase"]);
                string DB = conexionMySql.generarStringDB(oDb);

                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
                string obtenerURL = "select url from inventario where id_codigo = " + codigo_inventario;

                string url = stringConexionMySql.EjecutarCommandoFichaTecnicaInventario(obtenerURL, DB);

                if (url != null)
                {

                    return Json(url);
                }
                else
                {
                    return Json(url);
                }
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.ToString());
            }
        }

        [Route("/{id}")]
        [HttpPost]
        public JsonResult EliminaCodigo(string id)
        {
            try
            {
                string codigo_inventario = id;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string oDb = (string)(Session["oBase"]);
                string DB = conexionMySql.generarStringDB(oDb);

                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
                string obtenerURL = "DELETE FROM inventario WHERE id_codigo = " + codigo_inventario;

                string url = stringConexionMySql.EjecutarCommandoFichaTecnicaInventario(obtenerURL, DB);

                if (url != null)
                {

                    return Json(url);
                }
                else
                {
                    return Json(url);
                }
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.ToString());
            }
        }

        [HttpPost]
        public string GetDataInv(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            if (string.IsNullOrEmpty(oInven.id_codigo))
            {
                oInven.id_codigo = "";
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                return str;
            }

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {

                string cInst = "SELECT JSON_OBJECT('CODIGO',a.id_codigo," + "'CODIGOE',a.codigoe," + "'PRODUCTO',a.producto," + "'LINEA',c.descripcion," + "'LINEAP',d.descripcion," + "'EXISTENCIA',ifnull(sum(b.entrada-b.salida),0)," + "'PRECIO1',a.precio1," + "'PRECIO2',a.precio2," + "'PRECIO3',a.precio3," + "'PRECIO4',a.precio4," + "'COSTO1',a.costo1," + "'COSTO2',a.costo2," + "'COSTO3',a.costo3," + "'COSTO4',a.costo4,"+ "'OBS',a.obs) ";
                cInst += " FROM inventario a ";
                cInst += " LEFT JOIN  kardexinven b ON (a.id_codigo= b.id_codigo) ";
                cInst += " LEFT JOIN catlineasi c ON (a.linea= c.id_linea)";
                cInst += " LEFT JOIN catlineasp d ON (a.lineac= d.id_linea)";
                cInst += " WHERE status = 'A'";

                if (oInven.id_codigo.ToString() != "")
                {
                    cInst += " AND a.id_codigo  = " + oInven.id_codigo.ToString();

                }
                cInst += " GROUP BY a.id_codigo ";

                llenar.EjecutarLectura(cInst, DB);
                if (llenar.consulta.Read())
                {
                    str = llenar.consulta.GetString(0).ToString();
                }
                else
                {
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                }


            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            llenar.CerrarConexion();
            return str;
        }

        [HttpPost]
        public ActionResult SaveDropzoneJsUploadedFiles()
        {   
            string Empresa = Session["oBase"].ToString();

            try
            {
                var id_codigo = Request.Params;
                var codigo = id_codigo[0].ToString();
                string path = Server.MapPath("~/Images/Inventario/" +Empresa.Trim() + "/" + codigo + "/");

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

        //PARA SUBIR EL DOCUMENTO
        [Route("/{id}")]
        [HttpPost]
        public JsonResult uploadFile(string id)
        {
            // check if the user selected a file to upload
            try
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];

                string codigoArchivo = id;
                string Empresa = Session["oBase"].ToString();

                string path = Server.MapPath("~/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/FichaTecnica/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/FichaTecnica/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set url = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Ficha tecnica cargada con exito");

                    }
                }
                else
                {
                    Directory.CreateDirectory(path);
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/FichaTecnica/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set url = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Ficha tecnica cargada con exito");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

            return Json(new { Message = "Archivo Guardado con Exito." });
        }


        //PARA SUBIR EL DOCUMENTO
        [Route("/{id}")]
        [HttpPost]
        public JsonResult uploadFotoCotizacion(string id)
        {
            // check if the user selected a file to upload
            try
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];

                string codigoArchivo = id;
                string Empresa = Session["oBase"].ToString();

                string path = Server.MapPath("~/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/Cotizaciones/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here
                        if (extension=="")
                        {
                            extension = ".jpg";
                        }

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/Cotizaciones/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set foto = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Foto cotizacion cargada con exito");

                    }
                }
                else
                {
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here

                        if (extension == "")
                        {
                            extension = ".jpg";
                        }

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/Cotizaciones/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set foto = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Foto cotizacion cargada con exito");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

            return Json(new { Message = "Archivo Guardado con Exito." });
        }


        //PARA SUBIR EL DOCUMENTO
        [Route("/{id}")]
        [HttpPost]
        public JsonResult uploadFotoApp(string id)
        {
            // check if the user selected a file to upload
            try
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];

                string codigoArchivo = id;
                string Empresa = Session["oBase"].ToString();

                string path = Server.MapPath("~/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/App/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here
                        if (extension == "")
                        {
                            extension = ".jpg";
                        }

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/App/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set fotoapp = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Foto para app cargada con exito");

                    }
                }
                else
                {
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[fileName];
                        string extension = Path.GetExtension(fileName);

                        //You can Save the file content here

                        if (extension == "")
                        {
                            extension = ".jpg";
                        }

                        fileUpload.SaveAs(path + Path.GetFileName(file.FileName + extension));
                        string pathFinal = "/Images/Inventario/" + Empresa + "/" + codigoArchivo + "/App/" + Path.GetFileName(file.FileName + extension);

                        //PARA ACTUALIZAR LA URL DEL INVENTARIO MODIFICADO QUE SE CARGO LA FICHA TECNICA
                        ConexionMySQL conexionMySql = new ConexionMySQL();
                        string DB = (string)this.Session["StringConexion"];

                        string actualizarInventario = "update inventario set fotoapp = '" + pathFinal + "' where id_codigo =" + codigoArchivo;
                        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                        string cError = "";
                        bool lError = false;

                        lError = stringConexionMySql.ExecCommand(actualizarInventario, DB, ref cError);
                        if (lError == true)
                        {
                            return Json("Se ingreso el documento pero no se actualizo la base de datos...");
                        }

                        return Json("Foto para app cargada con exito");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

            return Json(new { Message = "Archivo Guardado con Exito." });
        }



        [HttpPost]
        public ActionResult SyncUpload()
        {

            // Specifies the target location for the uploaded files
            string targetLocation = Server.MapPath("~/Files/");

            // Specifies the maximum size allowed for the uploaded files (700 kb)
            int maxFileSize = 1024 * 700;

            // Checks whether the request contains any files
            if (Request.Files.Count == 0)
                return View("Index");

            HttpFileCollectionBase files = Request.Files;
            for (int index = 0; index < files.Count; index++)
            {
                HttpPostedFileBase file = files[index];

                // Checks that the file is not empty
                if (file.ContentLength <= 0)
                    continue;
                string fileName = file.FileName;

                // Checks that the file size does not exceed the allowed size
                if (file.ContentLength > maxFileSize)
                    continue;

                // Checks that the file is an image
                if (!file.ContentType.Contains("image"))
                    continue;

                try
                {
                    string path = System.IO.Path.Combine(targetLocation, file.FileName);
                    // Here, make sure that the file will be saved to the required directory.
                    // Also, ensure that the client has not uploaded files with malicious content.
                    // If all checks are passed, save the file.
                    file.SaveAs(path);
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return View("Index");
        }

        [HttpPost]
        public string BuscarID(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "SELECT JSON_OBJECT('CODIGO',max(a.id_codigo)) AS CODIGO FROM inventario a";
               
                llenar.EjecutarLectura(cInst, DB);
                if (llenar.consulta.Read())
                {
                    str = llenar.consulta.GetString(0).ToString();
                }
                else
                {
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }
            llenar.CerrarConexion();
            return str;
        }


        [HttpPost]
        public string BuscarCodigo(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";


            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL llenar = new StringConexionMySQL();

            string DB = (string)this.Session["StringConexion"];

            try
            {

                string cInst = "SELECT JSON_OBJECT('CODIGO',codigoe) AS CODIGO FROM inventario a WHERE codigoe ='"+oInven.id_codigo+"'";




                llenar.EjecutarLectura(cInst, DB);
                if (llenar.consulta.Read())
                {
                    str = llenar.consulta.GetString(0).ToString();
                }
                else
                {
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                }


            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            llenar.CerrarConexion();
            return str;
        }

        [HttpPost]
        public string InsertarProducto(Inventario oInven)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError="";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "INSERT INTO inventario ";
                cInst += "(id_codigo, codigoe, producto, linea, lineac, costo1, costo2, costo3, costo4, ";
                cInst += " servicio, stockmin, stockmax, precio1, precio2, precio3, precio4, obs, numero_departe) ";
                cInst += "VALUES (";
                cInst += oInven.idcodigoeq + ",";
                cInst += "'" + oInven.codigoeq + "',";
                cInst += "'" + oInven.producto + "',";
                cInst += "'" + oInven.id_linea + "',";
                cInst += "'" + oInven.lineaeq + "',";
                cInst += oInven.costoeq1 + ",";
                cInst += oInven.costoeq2 + ",";
                cInst += oInven.costoeq3 + ",";
                cInst += oInven.costoeq4 + ",";
                cInst += "'" + oInven.servicio + "',";
                cInst += oInven.stockmin + ",";
                cInst += oInven.stockmax + ",";
                cInst += oInven.precioeq1 + ",";
                cInst += oInven.precioeq2 + ",";
                cInst += oInven.precioeq3 + ",";
                cInst += oInven.precioeq4 + ",";
                cInst += "'" + oInven.obs + "',";
                cInst += "'" + oInven.alias + "')";

                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;                    
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;   
                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }

    }
}
