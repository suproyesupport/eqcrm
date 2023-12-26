using EqCrm.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class OrdenTecnicaModController : Controller
    {
        // GET: OrdenTecnicaMod
        public ActionResult OrdenTecnicaMod()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            string id_agencia = (string)this.Session["Sucursal"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            List<SelectListItem> asesores = new List<SelectListItem>();
            string setenciaSQLasesores = "SELECT id_codigo, nombre FROM vendedores";
            stringConexionMySql.LLenarDropDownList(setenciaSQLasesores, DB, asesores);
            ViewData["Asesores"] = asesores;

            List<SelectListItem> aseguradoras = new List<SelectListItem>();
            string setenciaSQLaseguradoras = "SELECT id_codigo, cliente FROM clientes WHERE tipocliente = 'ASEG'";
            stringConexionMySql.LLenarDropDownList(setenciaSQLaseguradoras, DB, aseguradoras);
            ViewData["Aseguradoras"] = aseguradoras;

            List<SelectListItem> tiposvehiculo = new List<SelectListItem>();
            string setenciaSQLtvechiculos = "SELECT id_linea, id_linea FROM cattiposv";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtvechiculos, DB, tiposvehiculo);
            ViewData["TiposVehiculos"] = tiposvehiculo;

            List<SelectListItem> marcas = new List<SelectListItem>();
            string setenciaSQLtmvehiculos = "SELECT id_linea, id_linea FROM catmarcasv";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtmvehiculos, DB, marcas);
            ViewData["MarcasVehiculos"] = marcas;

            //List<SelectListItem> lineas = new List<SelectListItem>();
            //string setenciaSQLtlvehiculos = "SELECT id_linea, id_linea FROM catdisenosv";
            //stringConexionMySql.LLenarDropDownList(setenciaSQLtlvehiculos, DB, lineas);
            //ViewData["LineasVehiculos"] = lineas;

            //stringConexionMySql.CerrarConexion();

            DataTable dt = new DataTable();
            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred AS DIASCREDITO, telefono AS TELEFONO FROM clientes ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteOS(cInst, dt, DB);

            DataTable dtinv = new DataTable();

            string cInstInventario = "SELECT a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.producto AS PRODUCTO, a.precio1 AS PRECIO,ifnull( (SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= " + id_agencia.ToString() + "),0) AS EXISTENCIA, if(servicio='N', 'BIEN', 'SERVICIO') AS TIPO, c.descripcion AS DESCRIPCION " +
                "FROM inventario a " +
                "INNER JOIN catlineasi c ON a.linea = c.id_linea ";
            ViewBag.TablaInventario = stringConexionMySql.LlenarDTTableHTmlInventarioPOS(cInstInventario, dtinv, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }




        [HttpPost]
        public ActionResult SaveDropzoneJsUploadedFiles()
        {
            string Empresa = Session["oBase"].ToString();

            try
            {
                var id_codigo = Request.Params;
                var codigo = id_codigo[0].ToString();
                var serie = id_codigo[1].ToString();
                string path = Server.MapPath("~/Images/OrdenServicio/" + Empresa.Trim() + "/" + codigo + "/" + serie + "/");

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




        [HttpPost]
        public string GetDataOrden(OrdenServicio orden)
        {
            string str = "";
            string DB = (string)(Session["StringConexion"]);
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];

            string query = "SELECT JSON_OBJECT(" +
                "'IDORDEN', no_factura, 'SERIE', serie, 'FECHAI', fecha, 'FECHAE', fechae, 'IDCLIENTE', id_cliente, 'CLIENTE', cliente, 'NIT', nit, 'ASESORES', id_vendedor, " +
                "'DIRECCION', direccion, 'TOTAL', total, 'TIPOVENTA', cont_cred, 'DIASCRED', dias_cred, 'OBS', obs, 'HECHOPOR', hechopor, 'IDAGENCIA', id_agencia, 'TIPOVEHICULO', tipov, 'MARCAVEHICULO', marcav, " +
                "'PLACA', id_placa, 'COLOR', color, 'KILOMETRAJE', odometro1, 'MODELO', modelo, 'FECHAING', fechaps, 'MEDIDA', kmmi, 'LINEA', linea, 'CHASSIS', nunidad, 'ASEGURADORAS', codaseguradora, 'POLIZA', nopolizaseg, " +
                "'ASESOREMERGENCIA', atendio, 'RECLAMO', reclamo, 'CORREDORA', corredora, 'OPCIONES', seguradotercero, 'AJUSTADOR', ajustador, 'DEDUCIBLE', deducible, 'TIPOPLACA', tipoplaca, " +
                "'TELEFONO', telefono, 'CORREO', email, 'RADIO', radio, 'ENCENDEDOR', encendedor, 'DOCUMENTOS', documentos, 'ALFOMBRAS', alfombras, 'LLANTA', llanta, 'TRICKET', tricket, 'LLAVE', llave, 'HERRAMIENTA', herramienta, " +
                "'PLATOS', platos) AS JSON FROM ordenserv ";

            string queryserie = " AND serie = '" + orden.serie + "' ";

            if (!string.IsNullOrEmpty(orden.bid_orden))
            {
                query += "WHERE no_factura = " + orden.bid_orden + queryserie;
            }

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = query,
                BaseDatos = oBaseDatos

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            string cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultado = new Resultado();
            resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultado.resultado.ToString() == "true")
            {
                str = resultado.Data[0].JSON.ToString();
            }

            if (str == "")
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }

            return str;
        }




        [HttpPost]
        public string BuscarenResoluciones()
        {
            string DB = (string)this.Session["StringConexion"];
            string id_agencia = (string)this.Session["Sucursal"];
            string str = "";
            string cInst = "SELECT direccion1, municipio, departamento, sucursal, adicional3 FROM resolucionessat WHERE tipo = 11 AND id_agencia = " + id_agencia;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(DB))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(cInst, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            str = reader[0].ToString() + "|" + reader[1].ToString() + " " + reader[2].ToString() + "|" + reader[3].ToString() + "|" + reader[4].ToString();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                string cError = "";
                cError = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + " " + ex.StackTrace.ToString() + "\", \"PRECIO\": 0}";
                return cError;
            }

            return str;
        }








        [Route("/{id}")]
        public ActionResult PDFOrdenServicio(string id)
        {

            string[] codURL = id.Split('|');
            string[] resoluciones = BuscarenResoluciones().Split('|');


            string idEmpresa = (string)this.Session["cIdEmpresa"];

            string id_ = codURL[0].ToString();
            string serie_ = codURL[1].ToString();

            string direccion = resoluciones[0].ToString();
            string deptomuni = resoluciones[1].ToString();
            string sucursal = resoluciones[2].ToString();

            //string adi = resoluciones[4].ToString();
            //string[] adicional = adi.Split('|');
            string pbx = resoluciones[3].ToString();
            string paginaweb = resoluciones[4].ToString();

            string str = "";
            string DB = (string)(Session["StringConexion"]);
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cEmail = (string)this.Session["cEmail"];
            //string cDireccion = (string)this.Session["cDireccion"]; PENDIENTE***


                string query = "SELECT JSON_OBJECT(" +
                "'ID_ORDEN', o.no_factura, 'BID_ORDEN', o.no_factura, 'SERIE', o.serie, 'FECHAINGRESO', o.fecha, 'FECHAENTREGA', o.fechae, 'IDCLIENTE', o.id_cliente, 'CLIENTE', o.cliente, 'NIT', o.nit, 'ASESORES', o.id_vendedor, 'TELEFONO', o.telefono, 'CORREO', o.email, " +
                "'DIRECCION', o.direccion, 'TOTAL', o.total, 'TIPOVENTA', o.cont_cred, 'DIASCRED', o.dias_cred, 'OBS', o.obs, 'HECHOPOR', o.hechopor, 'IDAGENCIA', o.id_agencia, 'TIPOVEHICULO', o.tipov, 'MARCA', o.marcav, " +
                "'PLACA', o.id_placa, 'COLOR', o.color, 'KILOMETRAJE', o.odometro1, 'MODELO', o.modelo, 'FECHAING', o.fechaps, 'MEDIDA', o.kmmi, 'LINEAVEHICULO', o.linea, 'CHASSIS', o.nunidad, 'ASEGURADORAS', IF(c.id_codigo = 0, '', c.cliente), 'POLIZA', o.nopolizaseg, " +
                "'ASESOREMERGENCIA', o.atendio, 'RECLAMO', o.reclamo, 'CORREDORA', o.corredora, 'OPCIONES', o.seguradotercero, 'AJUSTADOR', o.ajustador, 'DEDUCIBLE', o.deducible, 'TIPOPLACA', o.tipoplaca" +
                ") AS JSON " +
                "FROM ordenserv o " +
                "INNER JOIN clientes c ON o.codaseguradora = c.id_codigo " +
                "WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "' ";

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = query,
                BaseDatos = oBaseDatos
            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            string cRespuesta = Funciones.EqAppQuery(jsonString);

            Resultado resultado = new Resultado();
            resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            OrdenServicio orden = new OrdenServicio();

            if (resultado.resultado.ToString() == "true")
            {
                str = resultado.Data[0].JSON.ToString();

                //DESERIALIZAMOS EL JSON EN OBJETO  
                orden = JsonConvert.DeserializeObject<OrdenServicio>(str);
            }
            if (str == "")
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }

            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";

            //instruccionSQL = "SELECT d.id_codigo AS CODIGO, d.cantidad AS CANTIDAD, d.obs AS PRODUCTO, d.subtotal AS SUBTOTAL, i.linea AS LINEA ";
            instruccionSQL = "SELECT cantidad AS CANTIDAD, obs AS PRODUCTO, FORMAT(subtotal, 2) AS SUBTOTAL, id_linea AS LINEA " +
                "FROM detordenserv " +
                "WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "'";

            //instruccionSQL = "SELECT d.cantidad AS CANTIDAD, d.obs AS PRODUCTO, FORMAT(d.subtotal, 2) AS SUBTOTAL, i.linea AS LINEA ";
            //instruccionSQL += "FROM detordenserv d ";
            //instruccionSQL += "INNER JOIN inventario i ON i.id_codigo = d.id_codigo ";
            //instruccionSQL += "WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "'";

            cResultado = lectura.LlenarItemsPDF(instruccionSQL, dt, Base_Datos).ToString();

            string[] tabla = cResultado.Split('|');

            ViewBag.Enderezado = tabla[0];
            ViewBag.Mecanica = tabla[1];
            ViewBag.RepExt = tabla[2];
            ViewBag.RepInt = tabla[3];
            ViewBag.Otros = tabla[4];

            string t5 = String.Format("{0:0.00}", Convert.ToDouble(tabla[5]));
            string t6 = String.Format("{0:0.00}", Convert.ToDouble(tabla[6]));
            string t7 = String.Format("{0:0.00}", Convert.ToDouble(tabla[7]));
            string t8 = String.Format("{0:0.00}", Convert.ToDouble(tabla[8]));
            string t9 = String.Format("{0:0.00}", Convert.ToDouble(tabla[9]));
            string t10 = String.Format("{0:0.00}", Convert.ToDouble(tabla[10]));

            ViewBag.TotalEnderezado = t5;
            ViewBag.TotalMecanica = t6;
            ViewBag.TotalRepExt = t7;
            ViewBag.TotalRepInt = t8;
            ViewBag.TotalOtros = t9;
            ViewBag.TotalCompleto = t10;

            ViewBag.Empresa = cComercial;
            ViewBag.DeptoMuni = deptomuni;
            ViewBag.Direccion = direccion;
            ViewBag.Pbx = pbx;
            ViewBag.PaginaWeb = paginaweb;


            lectura.CerrarConexion();

            return (ActionResult)this.View(orden);

        }





        [HttpPost]
        public string ConsultaKardexCodigo(string id_codigo)
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";
            string id_ = "";
            string serie_ = "";

            instruccionSQL = "SELECT d.id_codigo AS CODIGO, d.cantidad AS CANTIDAD, d.obs AS PRODUCTO, d.subtotal AS SUBTOTAL, i.linea AS LINEA ";
            instruccionSQL += "FROM detordenserv d ";
            instruccionSQL += "INNER JOIN inventario i ON i.id_codigo = d.id_codigo ";
            instruccionSQL += "WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "'";

            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;
        }




        //OBTENER LISTA CON FILTRO
        [HttpPost]
        public object GetListaOrden(OrdenServicio orden)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string id_agencia = (string)this.Session["Sucursal"];

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            StringConexionMySQL llenar = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            string pattern = "MM/dd/yyyy";
            DateTime parsedFecha1, parsedFecha2;

            string query1 = "SELECT no_factura AS IDORDEN, serie AS SERIE, DATE_FORMAT(fecha, \"%d-%m-%Y\") AS FECHA, status AS STATUS, id_cliente AS IDCLIENTE, cliente AS CLIENTE, nit AS NIT, id_placa AS PLACA, marcav AS MARCAVEHICULO, " +
                "linea AS LINEA, FORMAT(total, 2) AS TOTAL " +
                "FROM ordenserv WHERE id_agencia = " + id_agencia + " ";

            if (!string.IsNullOrEmpty(orden.bid_orden))
            {
                query1 += "AND no_factura = " + orden.bid_orden;
            }

            if (!string.IsNullOrEmpty(orden.bidcliente))
            {
                query1 += "AND id_cliente = " + orden.bidcliente;
            }

            if (!string.IsNullOrEmpty(orden.b_placa))
            {
                query1 += "AND id_placa LIKE '%" + orden.b_placa + "%' ";
            }

            if (!string.IsNullOrEmpty(orden.fecha1) && !string.IsNullOrEmpty(orden.fecha2))
            {
                if (DateTime.TryParseExact(orden.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                {
                    string fecha = parsedFecha1.Year + "-" +
                        (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                        "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                    query1 += " AND fecha >= '" + fecha + "'";
                }
            }

            if (!string.IsNullOrEmpty(orden.fecha2) && !string.IsNullOrEmpty(orden.fecha1))
            {
                if (DateTime.TryParseExact(orden.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                {
                    string fecha = parsedFecha2.Year + "-" +
                        (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                        "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                    query1 += " AND fecha <= '" + fecha + "'";
                }
            }

            query1 += " AND status = 'I' ORDER BY fecha DESC ";

            LlenarListaDocumentos.listaOrdenTecnica = llenar.ListaOrdenTecnica(query1, DB, LlenarListaDocumentos.listaOrdenTecnica);

            return JsonConvert.SerializeObject(LlenarListaDocumentos.listaOrdenTecnica);
        }



        [HttpPost]
        public string BuscaDetalleOrden(OrdenServicio orden)
        {
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            DataTable dt = new DataTable();

            string query = "SELECT d.id_codigo, d.cantidad, d.obs, d.precio, d.descto, d.subtotal, d.servicio, d.id_linea, c.descripcion " +
                "FROM detordenserv d " +
                "LEFT JOIN catlineasi c ON d.id_linea = c.id_linea " +
                "WHERE d.no_factura = " + orden.id_orden + " " +
                "AND d.serie = '" + orden.serie + "'";

            stringConexionMySql.LlenarTabla(query, dt, DB);


            var lst = dt.AsEnumerable()
                .Select(r => r.Table.Columns.Cast<DataColumn>()
                        .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                       ).ToDictionary(z => z.Key, z => z.Value)
                ).ToList();
            //now serialize it
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            //var jponce = serializer.Serialize(lst);
            return serializer.Serialize(lst);
            // return queryVerCotizacion;
        }



        //FUNCION PARA CARGAR LAS LINEAS DE LOS VEHICULOS
        [HttpPost]
        public string Cargar(OrdenServicio oOS)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                DataTable dt = new DataTable();
                string cInst = "SELECT  id_linea AS ID, id_linea AS LINEA FROM catdisenosv WHERE id_marca = '" + oOS.marca + "'";
                str = stringConexionMySql.LlenarDDLLineaVehiculoMod(cInst, dt, DB);
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string CargarCatLineasI(string  id_linea)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                DataTable dt = new DataTable();
                string cInst = "SELECT  id_linea AS ID, descripcion AS DESCRIPCION FROM catlineasi ";
                str = stringConexionMySql.LlenarDDLCatLineasi(cInst, dt, DB, id_linea);
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }




        [HttpPost]
        public string GuardarOrdenServicio(string informacion, string jsonTablaDetalle, int sizeTablaDetalle)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //DESERIALIZAMOS EL JSON EN OBJETO  
            OrdenServicio oOS = JsonConvert.DeserializeObject<OrdenServicio>(informacion);

            //CONVERTIMOS EL JSON EN UN DATATABLE
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonTablaDetalle, (typeof(DataTable)));

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            string sucursal = (string)this.Session["Sucursal"];
            //int correlativo = 1;

            try
            {
                if (oOS.deducible == "")
                {
                    oOS.deducible = "0.00";
                }

                if (oOS.aseguradoras == "")
                {
                    oOS.aseguradoras = "0";
                }

                //ELIMINAR DETALLE DE ORDEN
                string query = "DELETE FROM detordenserv WHERE no_factura = " + oOS.id_orden + " AND serie = '" + oOS.serie + "'";
                lError = insertar.ExecCommand(query, DB, ref cError);
                insertar.CerrarConexion();

                //ELIMINAR ORDEN
                query = "DELETE FROM ordenserv WHERE no_factura = " + oOS.id_orden + " AND serie = '" + oOS.serie + "'";
                lError = insertar.ExecCommand(query, DB, ref cError);
                insertar.CerrarConexion();

                //INSERTAR ORDEN
                query = "INSERT INTO ordenserv (no_factura, serie, fecha, fechae, id_cliente, cliente, nit, id_vendedor, direccion, total, cont_cred, dias_cred, obs, hechopor, " +
                "id_agencia, tipov, marcav, id_placa, color, odometro1, modelo, fechaps, kmmi, linea, nunidad, telefono, email, codaseguradora, nopolizaseg, atendio, reclamo, corredora, seguradotercero, ajustador, deducible, tipoplaca, " +
                "radio, encendedor, documentos, alfombras, llanta, tricket, llave, herramienta, platos) ";
                query += "VALUES (";
                query += oOS.id_orden + ", ";                   //no_factura              
                query += "'" + oOS.serie + "', ";               //serie  
                query += "'" + oOS.fechaingreso + "', ";        //fecha
                query += "'" + oOS.fechaentrega + "', ";        //fechae
                query += oOS.idcliente + ", ";                  //id_cliente
                query += "'" + oOS.cliente + "', ";             //cliente
                query += "'" + oOS.nit + "', ";                 //nit
                query += oOS.asesores + ", ";                   //id_vendedor
                query += "'" + oOS.direccion + "', ";           //direccion
                query += oOS.total + ", ";                      //total
                query += oOS.tipoVenta + ", ";                  //cont_cred       
                query += oOS.diascredito + ", ";                //dias_cred
                query += "'" + oOS.obs + "', ";                 //obs
                query += "'" + cUserConected + "', ";           //hechopor
                query += sucursal + ", ";                       //id_agencia
                query += "'" + oOS.tiposvehiculos + "', ";      //tipov
                query += "'" + oOS.marca + "', ";               //marcav
                query += "'" + oOS.placa + "', ";               //id_placa
                query += "'" + oOS.color + "', ";               //color
                query += oOS.kilometraje + ", ";                //odometro1
                query += oOS.modelo + ", ";                     //modelo
                query += "'" + oOS.fechaingreso + "', ";        //fechaps
                query += oOS.medida + ", ";                     //kmmi
                query += "'" + oOS.lineavehiculo + "', ";       //linea
                query += "'" + oOS.chassis + "', ";             //nunidad
                query += "'" + oOS.telefono + "', ";            //telefono
                query += "'" + oOS.correo + "', ";              //email
                query += oOS.aseguradoras + ", ";               //codaseguradora
                query += "'" + oOS.poliza + "', ";              //nopolizaseg
                query += "'" + oOS.asesoremergencia + "', ";    //atendio
                query += "'" + oOS.reclamo + "', ";             //reclamo
                query += "'" + oOS.corredora + "', ";           //corredora
                query += oOS.opciones + ", ";                   //seguradotercero
                query += "'" + oOS.ajustador + "', ";           //ajustador            
                query += oOS.deducible + ", ";                  //deducible
                query += "'" + oOS.tipoplaca + "', ";           //tipoplaca
                query += oOS.radio + ", ";                      //radio
                query += oOS.encendedor + ", ";                 //encendedor
                query += oOS.documentos + ", ";                 //documentos
                query += oOS.alfombras + ", ";                  //alfombras
                query += oOS.llanta + ", ";                     //llanta
                query += oOS.tricket + ", ";                    //tricket
                query += oOS.llave + ", ";                      //llave
                query += oOS.herramienta + ", ";                //herramienta
                query += oOS.platos + "); ";                    //platos 

                lError = insertar.ExecCommand(query, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }


                if (sizeTablaDetalle > 1)
                {
                    int no_cor = 1;

                    try
                    {
                        //Recorremos el DataTable
                        foreach (DataRow row in dt.Rows)
                        {
                            string codigo = row["codigo"].ToString();
                            string cantidad = row["cantidad"].ToString();
                            string producto = row["producto"].ToString();
                            string servicio = row["servicio"].ToString();
                            string precio = row["precio"].ToString();
                            string descto = row["descto"].ToString();
                            string subtotal = row["subtotal"].ToString();
                            string tipo = row["tipo"].ToString().Trim();

                            if (codigo != "")
                            {
                                string queryDet = "INSERT INTO detordenserv (no_factura, serie, id_codigo, cantidad, precio, descto, tdescto, subtotal, obs, servicio, id_agencia, no_cor, id_linea) ";
                                queryDet += "VALUES (";
                                queryDet += oOS.id_orden + ", ";        //no_factura              
                                queryDet += "'" + oOS.serie + "', ";    //serie  
                                queryDet += codigo + ", ";              //id_codigo
                                queryDet += cantidad + ", ";            //cantidad
                                queryDet += precio + ", ";              //precio
                                queryDet += descto + ", ";              //descto
                                queryDet += "0.00" + ", ";              //tdescto
                                queryDet += subtotal + ", ";            //subtotal
                                queryDet += "'" + producto + "', ";     //obs
                                queryDet += "'" + servicio + "', ";     //obs
                                queryDet += sucursal + ", ";            //id_agencia
                                queryDet += no_cor + ", ";              //obs
                                queryDet += "'" + tipo + "');";         //tipo


                                lError = insertar.ExecCommand(queryDet, DB, ref cError);
                                no_cor++;

                                if (lError == true)
                                {
                                    cMensaje = cError;
                                    insertar.CerrarConexion();
                                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                                    return str;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
                    }

                    str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"ORDEN MODIFICADA EXITOSAMENTE \"}";

                    insertar.CerrarConexion();
                    return str;

                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"ORDEN MODIFICADA EXITOSAMENTE \"}";
            insertar.CerrarConexion();

            return str;
        }

        

    }
}
