using EqCrm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class ConsultaOrdenServicioTallerController : Controller
    {
        // GET: OrdenTecnicaMod
        public ActionResult ConsultaOrdenServicioTaller()
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

            List<SelectListItem> lineas = new List<SelectListItem>();
            string setenciaSQLtlvehiculos = "SELECT id_linea, id_linea FROM catdisenosv";
            stringConexionMySql.LLenarDropDownList(setenciaSQLtlvehiculos, DB, lineas);
            ViewData["LineasVehiculos"] = lineas;

            //stringConexionMySql.CerrarConexion();

            DataTable dt = new DataTable();
            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO,telefono AS TELEFONO FROM clientes ";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlClienteOS(cInst, dt, DB);

            DataTable dtinv = new DataTable();

            string cInstInventario = "SELECT a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.producto AS PRODUCTO, a.precio1 AS PRECIO,ifnull( (SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= " + id_agencia.ToString() + "),0) AS EXISTENCIA, if(servicio='N', 'BIEN', 'SERVICIO') AS TIPO FROM inventario a ";
            ViewBag.TablaInventario = stringConexionMySql.LlenarDTTableHTmlInventarioPOS(cInstInventario, dtinv, DB);

            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }



        [HttpPost]
        public string GetDataOrden(OrdenServicio orden)
        {

            //StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            //StringConexionMySQL stringConexionMySql2 = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]);
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];

            string query = "SELECT JSON_OBJECT(" +
                "'IDORDEN', no_factura, 'SERIE', serie, 'FECHAI', fecha, 'FECHAE', fechae, 'IDCLIENTE', id_cliente, 'CLIENTE', cliente, 'NIT', nit, 'ASESORES', id_vendedor, " +
                "'DIRECCION', direccion, 'TOTAL', FORMAT(total,2), 'TIPOVENTA', cont_cred, 'DIASCRED', dias_cred, 'OBS', obs, 'HECHOPOR', hechopor, 'IDAGENCIA', id_agencia, 'TIPOVEHICULO', tipov, 'MARCAVEHICULO', marcav, " +
                "'PLACA', id_placa, 'COLOR', color, 'KILOMETRAJE', odometro1, 'MODELO', modelo, 'FECHAING', fechaps, 'MEDIDA', kmmi, 'LINEA', linea, 'CHASSIS', nunidad, 'ASEGURADORAS', codaseguradora, 'POLIZA', nopolizaseg, " +
                "'ASESOREMERGENCIA', atendio, 'RECLAMO', reclamo, 'CORREDORA', corredora, 'OPCIONES', seguradotercero, 'AJUSTADOR', ajustador, 'DEDUCIBLE', deducible, 'TIPOPLACA', tipoplaca" +
                ") AS JSON FROM ordenserv ";

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


        [Route("/{id}")]
        public ActionResult PDFOrdenServicio(string id)
        {

            string[] codURL = id.Split('|');

            string id_ = codURL[0].ToString();
            string serie_ = codURL[1].ToString();

            string str = "";
            string DB = (string)(Session["StringConexion"]);
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cEmail = (string)this.Session["cEmail"];
            string cDireccion = (string)this.Session["cDireccion"];


            string query = "SELECT JSON_OBJECT(" +
                "'ID_ORDEN', no_factura, 'BID_ORDEN', no_factura, 'SERIE', serie, 'FECHAINGRESO', fecha, 'FECHAENTREGA', fechae, 'IDCLIENTE', id_cliente, 'CLIENTE', cliente, 'NIT', nit, 'ASESORES', id_vendedor, 'TELEFONO', telefono, 'CORREO', email, " +
                "'DIRECCION', direccion, 'TOTAL', total, 'TIPOVENTA', cont_cred, 'DIASCRED', dias_cred, 'OBS', obs, 'HECHOPOR', hechopor, 'IDAGENCIA', id_agencia, 'TIPOVEHICULO', tipov, 'MARCA', marcav, " +
                "'PLACA', id_placa, 'COLOR', color, 'KILOMETRAJE', odometro1, 'MODELO', modelo, 'FECHAING', fechaps, 'MEDIDA', kmmi, 'LINEAVEHICULO', linea, 'CHASSIS', nunidad, 'ASEGURADORAS', codaseguradora, 'POLIZA', nopolizaseg, " +
                "'ASESOREMERGENCIA', atendio, 'RECLAMO', reclamo, 'CORREDORA', corredora, 'OPCIONES', seguradotercero, 'AJUSTADOR', ajustador, 'DEDUCIBLE', deducible, 'TIPOPLACA', tipoplaca" +
                ") AS JSON FROM ordenserv WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "' ";



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
            instruccionSQL = "SELECT d.cantidad AS CANTIDAD, d.obs AS PRODUCTO, FORMAT(d.subtotal, 2) AS SUBTOTAL, i.linea AS LINEA ";
            instruccionSQL += "FROM detordenserv d ";
            instruccionSQL += "INNER JOIN inventario i ON i.id_codigo = d.id_codigo ";
            instruccionSQL += "WHERE no_factura = " + id_ + " AND serie = '" + serie_ + "'";

            cResultado = lectura.LlenarItemsPDF(instruccionSQL, dt, Base_Datos).ToString();

            string[] tabla = cResultado.Split('|');

            ViewBag.Enderezado = tabla[0];
            ViewBag.Mecanica = tabla[1];
            ViewBag.RepExt = tabla[2];
            ViewBag.RepInt = tabla[3];
            ViewBag.Otros = tabla[4];

            string t5 = String.Format("{0:0.##}", Convert.ToDouble(tabla[5]));
            string t6 = String.Format("{0:0.##}", Convert.ToDouble(tabla[6]));
            string t7 = String.Format("{0:0.##}", Convert.ToDouble(tabla[7]));
            string t8 = String.Format("{0:0.##}", Convert.ToDouble(tabla[8]));
            string t9 = String.Format("{0:0.##}", Convert.ToDouble(tabla[9]));
            string t10 = String.Format("{0:0.##}", Convert.ToDouble(tabla[10]));

            ViewBag.TotalEnderezado = t5;
            ViewBag.TotalMecanica = t6;
            ViewBag.TotalRepExt = t7;
            ViewBag.TotalRepInt = t8;
            ViewBag.TotalOtros = t9;
            ViewBag.TotalCompleto = t10;

            ViewBag.Empresa = cComercial;
            ViewBag.Direccion = cDireccion;



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

            string query1 = "SELECT no_factura AS IDORDEN, serie AS SERIE, DATE_FORMAT(fecha, \"%d-%m-%Y\") AS FECHA, status AS STATUS, id_cliente AS IDCLIENTE, cliente AS CLIENTE, nit AS NIT, id_placa AS PLACA, marcav AS MARCAVEHICULO, linea AS LINEA, total AS TOTAL " +
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

            query1 += " ORDER BY no_factura DESC";


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

            string query = "SELECT id_codigo, cantidad, obs, precio, descto, subtotal FROM detordenserv WHERE no_factura = " + orden.id_orden + " AND serie = '" + orden.serie + "'";

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
                "id_agencia, tipov, marcav, id_placa, color, odometro1, modelo, fechaps, kmmi, linea, nunidad, telefono, email, codaseguradora, nopolizaseg, atendio, reclamo, corredora, seguradotercero, ajustador, deducible, tipoplaca) ";
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
                query += "0.00, ";                              //total
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
                query += "'" + oOS.tipoplaca + "'); ";          //tipoplaca

                lError = insertar.ExecCommand(query, DB, ref cError);

                //correlativo++;


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
                            string precio = row["precio"].ToString();
                            string subtotal = row["subtotal"].ToString();

                            if (codigo != "")
                            {
                                string queryDet = "INSERT INTO detordenserv (no_factura, serie, id_codigo, cantidad, precio, descto, tdescto, subtotal, obs, id_agencia, no_cor) ";
                                queryDet += "VALUES (";
                                queryDet += oOS.id_orden + ", ";        //no_factura              
                                queryDet += "'" + oOS.serie + "', ";    //serie  
                                queryDet += codigo + ", ";              //id_codigo
                                queryDet += cantidad + ", ";            //cantidad
                                queryDet += precio + ", ";              //precio
                                queryDet += "0.00" + ", ";              //descto
                                queryDet += "0.00" + ", ";              //tdescto
                                queryDet += subtotal + ", ";            //subtotal
                                queryDet += "'" + producto + "', ";     //obs
                                queryDet += sucursal + ", ";            //id_agencia
                                queryDet += no_cor + "); ";             //obs

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

                    str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

                    insertar.CerrarConexion();
                    return str;

                }

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

            insertar.CerrarConexion();
            return str;
        }
    }
}