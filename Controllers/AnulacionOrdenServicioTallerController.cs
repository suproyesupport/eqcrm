using EqCrm.Models;
using MySql.Data.MySqlClient;
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
    public class AnulacionOrdenServicioTallerController : Controller
    {
        // GET: AnulacionOrdenServicioTaller
        public ActionResult AnulacionOrdenServicioTaller()
        {

            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            
            return View();
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
            string cInst = "SELECT direccion1, municipio, departamento FROM resolucionessat WHERE tipo = 11 AND id_agencia = " + id_agencia;

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
                            str = reader[0].ToString() + "|" + reader[1].ToString() + " " + reader[2].ToString();
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
            string[] deptomunidire = BuscarenResoluciones().Split('|');


            string idEmpresa = (string)this.Session["cIdEmpresa"];

            string id_ = codURL[0].ToString();
            string serie_ = codURL[1].ToString();

            string direccion = deptomunidire[0].ToString();
            string deptomuni = deptomunidire[1].ToString();

            string str = "";
            string DB = (string)(Session["StringConexion"]);
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cEmail = (string)this.Session["cEmail"];
            //string cDireccion = (string)this.Session["cDireccion"]; PENDIENTE***


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

            string query1 = "SELECT no_factura AS IDORDEN, serie AS SERIE, fecha AS FECHA, status AS STATUS, id_cliente AS IDCLIENTE, cliente AS CLIENTE, nit AS NIT, id_placa AS PLACA, marcav AS MARCAVEHICULO, linea AS LINEA, total AS TOTAL " +
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
        public string CargarCatLineasI(string id_linea)
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
        public string AnularOrdenServicio(string anulaid, string anulaserie)
        {
            string str = "";
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string DB = (string)this.Session["StringConexion"];
            //string sucursal = (string)this.Session["Sucursal"];

            try
            {
                

                //ELIMINAR DETALLE DE ORDEN
                string query = "UPDATE ordenserv SET status = 'A' WHERE no_factura = " + anulaid + " AND serie = '" + anulaserie + "';";
                lError = insertar.ExecCommand(query, DB, ref cError);
                insertar.CerrarConexion();
                str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"" + "TRUE" + "\", \"PRECIO\": 0}";


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