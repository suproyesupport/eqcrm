using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Xml;
using System.IO;
using EqCrm.Models.POS;
using EqCrm.Models;
using System.Globalization;
using Newtonsoft.Json;
using DevExpress.XtraLayout.Converter;
using DevExpress.Data.Linq.Design;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web.Configuration;
using RestSharp;
using ParameterType = RestSharp.ParameterType;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EqCrm.Controllers.POS
{
    public class OperacionesController : Controller
    {

        public ActionResult CrearVentas()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string IDCajaAsignada = Session["IDCajaASignada"].ToString();

            if (IDCajaAsignada != null)
            {
                ViewData["CajasAsignadas"] = Session["IDCajaAsignada"].ToString();
            }
            else
            {
                ViewData["CajasAsignadas"] = "0";
            }
            string cUserConected = (string)(Session["Usuario"]);
            string cEmpresa = (string)(Session["cNombreEmisor"]);

            string id_agencia = (string)this.Session["Sucursal"];
            string Base_Datos = (string)(Session["StringConexion"]);
            string factsinexist = (string)(Session["Permiso"]);
            string oBase = (string)(Session["oBase"]);
            string cTabla = "";

            string cNit = (string)this.Session["cNit"];


            string oDbOp = System.Configuration.ConfigurationManager.AppSettings["oTrosParametros"];


            string cDireccion = (string)this.Session["cDireccion"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string certificador = (string)this.Session["certificador"];


            if (certificador == "G4S")
            {
                var wsEnvio = wsConnector.wsEnvio("GET_ESTABLECIMIENTOS", "0", "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

                //var wsEnvio = wsConnector.wsEnvio("GET_ESTABLECIMIENTOS", "0", "", "ADMINISTRADOR", "https://fel.g4sdocumenta.com/webservicefront/factwsfront.asmx?wsdl", "7D7E1C0E-F77D-483D-B848-6863C8565BBD", "SYSTEM_REQUEST", "GT", "90843444", false, "");

                ViewBag.Establecimientos = Funciones.llenarEstablecimientos(wsEnvio[2], "idEstablecimientos");
            }
            else
            {
                ViewBag.Establecimientos = "";
            }

            StringConexionMySQL mysql = new StringConexionMySQL();
            Funciones funciones = new Funciones();

            DataTable dt = new DataTable();
            DataTable dtInventario = new DataTable();




            /// Cada dia mas cabron y cierra conexion para no usar appi que no sea pro c#
            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO, email AS CORREO, facturar AS FACTURAR FROM clientes ";
            LlenarListaVentas.listaClientes = mysql.ClientesFac(cInst, Base_Datos, LlenarListaVentas.listaClientes);
            string cRespuesta = JsonConvert.SerializeObject(LlenarListaVentas.listaClientes);
            cTabla = Funciones.LlenarTableHTmlClientePOS(cRespuesta);                       
            ViewBag.Tabla = cTabla;



            cInst = "SELECT codigoEscenario,escenario FROM  frases ";
            ListaOtrosParametros.listafrases = mysql.ListaFrases(cInst, oDbOp, ListaOtrosParametros.listafrases);
            string cFrases = JsonConvert.SerializeObject(ListaOtrosParametros.listafrases);
            //cTabla = Funciones.llenarSelectOnChange(cFrases, "cTipoFrase", "verificarFrase(this.value);");
            cTabla = Funciones.llenarFrases(cFrases, "cTipoFrase");
            ViewBag.Frases = cTabla;





            List<SelectListItem> listadotipodoctos = new List<SelectListItem>();
            //cInst = "SELECT serie AS IDSERIE, serie AS SERIE FROM resolucionessat WHERE tipo IN (1,5) AND id_agencia = '" + id_agencia.ToString()+ "'";
            cInst = "SELECT serie AS IDSERIE, serie AS SERIE FROM resolucionessat WHERE tipo IN (1,5)";
            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resolucionessat = new Resultado();
            resolucionessat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resolucionessat.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.llenarSelectOnChange(cRespuesta, "cTipoDocto", "verificarSerie(this.value);");
                
                ViewBag.TipoDocto = cTabla;
            }
           

            if (funciones.DamePermisos("1884", oBase) == true)
            {
                ViewBag.PermisoFacturarOrden = "true";
            }


            //if (funciones.DamePermisos("2305", Base_Datos) == true )
            //if (funciones.DamePermisos("2305", oBase) == true)
            //{
                cInst = "SELECT a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.numero_departe AS NOPARTE, a.id_marca AS MARCA, a.producto AS PRODUCTO,  a.precio1 AS PRECIO,ifnull( (SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= " + id_agencia.ToString() + "),0) AS EXISTENCIA, if(servicio='N', 'BIEN', 'SERVICIO') AS TIPO FROM inventario a ";
                //ViewBag.TablaInventario = mysql.LlenarDTTableHTmlInventarioPOS(cInst, dtInventario, Base_Datos);
                //mysql.CerrarConexion();
            //}
          //  else
          //  {
            //    cInst = "SELECT a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.producto AS PRODUCTO, a.precio1 AS PRECIO,ifnull( (SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= " + id_agencia.ToString() + "),0) AS EXISTENCIA, if(servicio='N', 'BIEN', 'SERVICIO') AS TIPO FROM inventario a ";
                //ViewBag.TablaInventario = mysql.LlenarDTTableHTmlInventarioPOS(cInst, dtInventario, Base_Datos);
                //mysql.CerrarConexion();
          //  }

            queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoTablaInventario = new Resultado();
            resultadoTablaInventario = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoTablaInventario.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.LlenarTableHTmlInventarioPOS(cRespuesta);
                ViewBag.TablaInventario = cTabla;
            }


            List<SelectListItem> listadoincoterm = new List<SelectListItem>();
            cInst = "SELECT id, concat(id,' ',nombre) FROM cattipoincoterm";


            queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoincoterm = new Resultado();
            resultadoincoterm = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoincoterm.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.llenarSelect(cRespuesta, "incoterm");
                //ViewData["incoterm"] = cTabla;
                ViewBag.incoterm = cTabla;
            }






           // mysql.LLenarDropDownListIncoterm(cInst, Base_Datos, listadoincoterm);
            //ViewData["incoterm"] = listadoincoterm;
           // mysql.CerrarConexion();



            string permisofactsinexist = (string)this.Session["Permiso"];


            if (permisofactsinexist == "S")
            {
                ViewBag.permiso = "true";
            }
            else if (permisofactsinexist == "N")
            {
                ViewBag.permiso = "false";
            }

            DateTime fechaActual = new DateTime();

            int year = fechaActual.Year;
            

            ViewBag.Footer = "<span class=\"hidden-md-down fw-700\">" + year + " © EqSoftware by&nbsp;<a href = 'http://www.intec.com.gt/wp/' class='text-primary fw-500' title='gotbootstrap.com' target='_blank'>https://www.cgtecsa.com</a> &nbsp;&nbsp;" + cEmpresa.ToUpper() + "</span>";
            ViewBag.User = " USUARIO: " + cUserConected.ToUpper();
            ViewBag.Encabezado = cUserConected.ToUpper();
            ViewBag.Encabezado2 = cEmpresa.ToUpper();
            ViewData["User"] = "USUARIO CONECTADO:" + cUserConected.ToString().ToUpper();


            return View("~/Views/POS/Operaciones/CrearVentas.cshtml");
        }





        [HttpPost]
        public string BuscaDetalleOrden(OrdenServicio orden)
        {
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            DataTable dt = new DataTable();

            string query = "";

            if (orden.resumen == "true")
            {
                query = "SELECT (1) AS id_codigo, d.cantidad, c.descripcion AS obs, SUM(d.precio) AS precio, d.descto, SUM(d.subtotal) AS subtotal, d.servicio, d.id_linea, c.descripcion " +
                "FROM detordenserv d " +
                "LEFT JOIN catlineasi c ON d.id_linea = c.id_linea " +
                "WHERE d.no_factura = " + orden.id_orden + " " +
                "AND d.serie = '" + orden.serie + "' " +
                "GROUP BY d.id_linea " +
                "ORDER BY d.id_linea";
            } else
            {
                query = "SELECT d.id_codigo, d.cantidad, CONCAT(c.descripcion, \" | \", d.obs) AS obs, d.precio, d.descto, d.subtotal, d.servicio, d.id_linea, c.descripcion " +
                "FROM detordenserv d " +
                "LEFT JOIN catlineasi c ON d.id_linea = c.id_linea " +
                "WHERE d.no_factura = " + orden.id_orden + " " +
                "AND d.serie = '" + orden.serie + "' " +
                "ORDER BY d.id_linea";
            }

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






        public string CargarOrdenes()
        {
            StringConexionMySQL mysql = new StringConexionMySQL();
            string Base_Datos = (string)(Session["StringConexion"]);
            string cTabla = "";

            // Cada dia mas cabron y cierra conexion para no usar appi que no sea pro c#
            string cInst = "SELECT no_factura AS ID_ORDEN, serie AS SERIE, fecha AS FECHA, id_cliente AS IDCLIENTE, cliente AS CLIENTE, nit AS NIT " +
                "FROM ordenserv " +
                " WHERE status != 'A' ORDER BY fecha DESC ";

            LlenarListaVentas.listaOrdenesServicio = mysql.ListOrdenServicioFacturar(cInst, Base_Datos, LlenarListaVentas.listaOrdenesServicio);
            string cRespuesta = JsonConvert.SerializeObject(LlenarListaVentas.listaOrdenesServicio);
            cTabla = Funciones.LlenarTableHTmlOrdenes(cRespuesta);

            return cTabla;
            //ViewBag.Tabla = cTabla;

            //return View();

            //return (ActionResult)this.View("~/Views/POS/Operaciones/CrearVentas.cshtml");
        }


        /// <summary>
        /// vamos a crear un pos diferente para lacol
        /// </summary>
        /// <param name="oCaja"></param>
        /// <returns></returns>

        public string  ObtieneVentas()
        {

            string id_agencia = (string)this.Session["Sucursal"];
            string Base_Datos = (string)(Session["StringConexion"]);
            string factsinexist = (string)(Session["Permiso"]);
            string oBase = (string)(Session["oBase"]);
            string cTabla = "";

            string cNit = (string)this.Session["cNit"];

           
            string cInst = "SELECT fecha as FECHA,firmaelectronica AS AUTORIZACION,no_docto_fel AS NO_FACTURA,serie_docto_fel AS SERIE, cliente AS CLIENTE, nit AS NIT, total AS TOTAL FROM facturas ORDER BY fecha DESC";
           

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            var jsonString = JsonConvert.SerializeObject(queryapi);

            var cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoTablaInventario = new Resultado();
            resultadoTablaInventario = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoTablaInventario.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.LlenarTableHTmlVentas(cRespuesta);
                
            }

            return cTabla;

        }


        public ActionResult PosC()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string IDCajaAsignada = Session["IDCajaASignada"].ToString();

            if (IDCajaAsignada != null)
            {
                ViewData["CajasAsignadas"] = Session["IDCajaAsignada"].ToString();
            }
            else
            {
                ViewData["CajasAsignadas"] = "0";
            }

            string id_agencia = (string)this.Session["Sucursal"];
            string Base_Datos = (string)(Session["StringConexion"]);
            string factsinexist = (string)(Session["Permiso"]);
            string oBase = (string)(Session["oBase"]);
            string cTabla = "";

            string cNit = (string)this.Session["cNit"];

            StringConexionMySQL mysql = new StringConexionMySQL();
            Funciones funciones = new Funciones();

            DataTable dt = new DataTable();
            DataTable dtInventario = new DataTable();


            string cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO, email AS CORREO, facturar AS FACTURAR FROM clientes LIMI 100";



            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            string cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultado = new Resultado();
            resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultado.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.LlenarTableHTmlClientePOS(cRespuesta);
                ViewBag.Tabla = cTabla;
            }


            


            List<SelectListItem> listadotipodoctos = new List<SelectListItem>();
            cInst = "SELECT serie AS IDSERIE, serie AS SERIE FROM resolucionessat WHERE tipo IN (1,5) AND id_agencia = '" + id_agencia.ToString() + "'";
            queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resolucionessat = new Resultado();
            resolucionessat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resolucionessat.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.llenarSelectOnChange(cRespuesta, "cTipoDocto", "verificarSerie(this.value);");

                ViewBag.TipoDocto = cTabla;
            }



            cInst = "SELECT a.id_codigo AS CODIGO, a.codigoe AS CODIGOE, a.numero_departe AS NOPARTE, a.id_marca AS MARCA, a.producto AS PRODUCTO,  a.precio1 AS PRECIO,ifnull( (SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia= " + id_agencia.ToString() + "),0) AS EXISTENCIA, if(servicio='N', 'BIEN', 'SERVICIO') AS TIPO FROM inventario a ";
            

            queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoTablaInventario = new Resultado();
            resultadoTablaInventario = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoTablaInventario.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.LlenarTableHTmlInventarioPOS(cRespuesta);
                ViewBag.TablaInventario = cTabla;
            }







            List<SelectListItem> listadoincoterm = new List<SelectListItem>();
            cInst = "SELECT id, concat(id,' ',nombre) FROM cattipoincoterm";


            queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBase

            };

            jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultadoincoterm = new Resultado();
            resultadoincoterm = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultadoincoterm.resultado.ToString() == "true")
            {

                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                cTabla = Funciones.llenarSelect(cRespuesta, "incoterm");
                //ViewData["incoterm"] = cTabla;
                ViewBag.incoterm = cTabla;
            }




            string permisofactsinexist = (string)this.Session["Permiso"];


            if (permisofactsinexist == "S")
            {
                ViewBag.permiso = "true";
            }
            else if (permisofactsinexist == "N")
            {
                ViewBag.permiso = "false";
            }

            return View("~/Views/Operaciones/PosC.cshtml");
        }




        public string getEmpresa()
        {
            return (string)(Session["oBase"]);

        }

        public string getCertificador()
        {
            
            return (string)this.Session["certificador"];
        }

        public string gettasa()
        {
            var nTasa = wsConnector.obtieneTasaBG();
            return nTasa.ToString();
        }



        [HttpPost]
        public string Cargar(Caja oCaja)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                DataTable dt = new DataTable();
                string cInst = "SELECT exportacion FROM resolucionessat WHERE serie = '" + oCaja.cTipoDocto + "'";
                str = stringConexionMySql.Consulta(cInst, DB);
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }






        // GET: Operaciones
        public ActionResult Corte()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            string Base_Datos = (string)(Session["StringConexion"]);
            StringConexionMySQL mysql = new StringConexionMySQL();
            //DROPDOWNLIST DE CAJAS REGISTRADAS AL USUARIO
            List<SelectListItem> listadoCajas = new List<SelectListItem>();
            string validarCajas = "select cu.id_caja, c.nombre from cajas_usuarios cu inner join cajas c on c.id_caja = cu.id_caja where estatus = 1 and cu.id_usuario = '" + Session["Usuario"] + "'";
            ConexionMySQL llenarCajas = new ConexionMySQL();
            llenarCajas.LLenarDropDownList(validarCajas, Base_Datos, listadoCajas);
            ViewData["CajasAsignadas"] = listadoCajas;
            llenarCajas.CerrarConexion();

            return View("~/Views/POS/Operaciones/Corte.cshtml");
        }



        //PARA OBTENER LISTADO SEGUN LA CAJA
        [HttpPost]
        public object FiltrarDatos(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            int id_caja = doctos.id_caja;

            if (string.IsNullOrEmpty(doctos.fecha1))
            {
                doctos.fecha1 = "";
            }

            if (string.IsNullOrEmpty(doctos.fecha2))
            {
                doctos.fecha2 = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();
                
                string DB = (string)this.Session["StringConexion"];
                string cInst = "CALL cortecaja(" + id_caja + ");";

                LLenarListaCorteCaja.listaCorteCaja = llenar.ListaCorteCaja(cInst, DB, LLenarListaCorteCaja.listaCorteCaja);
                llenar.CerrarConexion();
            }
            return JsonConvert.SerializeObject(LLenarListaCorteCaja.listaCorteCaja);
        }



        [HttpPost]
        public object FiltrarDatosFechas(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);
            int id_caja = doctos.id_caja;

            if (string.IsNullOrEmpty(doctos.fecha1))
            {
                doctos.fecha1 = "";
            }

            if (string.IsNullOrEmpty(doctos.fecha2))
            {
                doctos.fecha2 = "";
            }

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                StringConexionMySQL llenar = new StringConexionMySQL();

                string DB = (string)this.Session["StringConexion"];


                string cInst = "";
                cInst = "SELECT ";
                cInst += "uuidcaja AS ID,";
                cInst += "id_caja AS CAJA,";
                cInst += "fecha AS FECHACAJA,";
                cInst += "efectivocontado AS EFECTIVOCONTADO,";
                cInst += "tarjetacontado AS TARJETACONTADO,";
                cInst += "chequecontado AS CHEQUECONTADO,";
                cInst += "valecontado AS VALESCONTADO";
                cInst += " FROM";
                cInst += " cortecaja";


                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        cInst += " WHERE fecha >= '" + fecha + "'";
                    }

                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        cInst += " AND fecha <= '" + fecha + "'";
                    }
                }

                cInst += " AND id_caja = " + id_caja;

                LLenarListaCorteCaja.listaCorteCaja = llenar.ListaCorteCaja(cInst, DB, LLenarListaCorteCaja.listaCorteCaja);
            }
            return JsonConvert.SerializeObject(LLenarListaCorteCaja.listaCorteCaja);
        }



        [HttpPost]
        public object GetCorte(FiltroGenerico filtro)
        {
            try
            {
                string cUserConected = (string)(Session["Usuario"]);
                if (string.IsNullOrEmpty(cUserConected))
                {
                    return (ActionResult)this.RedirectToAction("Login", "Account");
                }
                else
                {
                    StringConexionMySQL llenar = new StringConexionMySQL();
                    int id_caja = filtro.id_caja;
                    string DB = (string)this.Session["StringConexion"];
                    string cInst = "SELECT total AS TOTAL,efectivo AS EFECTIVO,if(tarjeta='',0.00,tarjeta) AS TARJETA,if(cheque='',0.00,cheque) AS CHEQUE,if(transferencia='',0.00,transferencia) AS TRANSFERENCIA,if(vale='',0.00,vale) AS VALE FROM  rescortecaja WHERE caja = " + id_caja;

                    LLenarListaCorteCaja.listaCorteCaja = llenar.ListaCorteCaja(cInst, DB, LLenarListaCorteCaja.listaCorteCaja);

                    return JsonConvert.SerializeObject(LLenarListaCorteCaja.listaCorteCaja);
                }
                
            }
            catch
            {
                return "Hubo un error...";
            }
        }



        [HttpPost]
        public object guardarCorte(FiltroGenerico doctos)
        {
            try
            {
                string cUserConected = (string)(Session["Usuario"]);
                double efectivocontado = doctos.efectivocontado;
                double tarjetacontado = doctos.tarjetacontado;
                double chequecontado = doctos.chequecontado;
                double valecontado = doctos.valecontado;
                double efectivocalculado = doctos.efectivocalculado;
                double tarjetacalculado = doctos.tarjetacalculado;
                double chequecalculado = doctos.chequecalculado;
                double valecalculado = doctos.valecalculado;
                double efectivodiferencia = doctos.efectivodiferencia;
                double tarjetadiferencia = doctos.tarjetadiferencia;
                double chequediferencia = doctos.chequediferencia;
                double valediferencia = doctos.valediferencia;
                double efectivoretiro = doctos.efectivoretiro;
                double tarjetaretiro = doctos.tarjetaretiro;
                double chequeretiro = doctos.chequeretiro;
                double valeretiro = doctos.valeretiro;
                string uuidcaja = doctos.uuidcaja;
                int id_caja = doctos.id_caja;
                string cMensaje = "";
                string cError="";


                if (string.IsNullOrEmpty(cUserConected))
                {
                    return (ActionResult)this.RedirectToAction("Login", "Account");
                }
                else
                {
                    StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                    string DB = (string)this.Session["StringConexion"];


                    string fecha = DateTime.Now.ToString("yyyy/MM/dd");


                    string storedProcedureCorteCaja = "ventaPOS_insertar_cortecaja";
                    List<string> listaCorteCaja = new List<string>();
                    listaCorteCaja.Add(fecha.ToString());
                    listaCorteCaja.Add(efectivocontado.ToString());
                    listaCorteCaja.Add(tarjetacontado.ToString());
                    listaCorteCaja.Add(chequecontado.ToString());
                    listaCorteCaja.Add(valecontado.ToString());
                    listaCorteCaja.Add(efectivocalculado.ToString());
                    listaCorteCaja.Add(tarjetacalculado.ToString());
                    listaCorteCaja.Add(chequecalculado.ToString());
                    listaCorteCaja.Add(valecalculado.ToString());
                    listaCorteCaja.Add(efectivodiferencia.ToString());
                    listaCorteCaja.Add(tarjetadiferencia.ToString());
                    listaCorteCaja.Add(chequediferencia.ToString());
                    listaCorteCaja.Add(valediferencia.ToString());
                    listaCorteCaja.Add(efectivoretiro.ToString());
                    listaCorteCaja.Add(tarjetaretiro.ToString());
                    listaCorteCaja.Add(chequeretiro.ToString());
                    listaCorteCaja.Add(valeretiro.ToString());
                    listaCorteCaja.Add(uuidcaja.ToString().ToUpper());
                    listaCorteCaja.Add(id_caja.ToString());

                    bool errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureCorteCaja, DB, listaCorteCaja,ref cError);

                    if (errorSP == true)
                    {
                   
                        cMensaje = cError;
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cMensaje;

                    }

                    string cInst = "";
                    cInst = " UPDATE facturas SET prom_pago = '1' , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE prom_pago = '' AND id_caja = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cInst = " UPDATE notadecredito SET prom_pago = '1' , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE  prom_pago = '' AND id_caja = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cInst = " UPDATE envios SET prom_pago = '1' , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE  prom_pago = ''AND  id_caja = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cInst = " UPDATE envios SET cortecaja=1 , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE cortecaja = 0 AND status = 'A' and id_caja2 = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cInst = " UPDATE ctaca SET cortecaja = '1' , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE cortecaja = 0 AND id_caja = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cInst = " UPDATE retiros SET cortecaja = '1' , uuidcaja='" + uuidcaja.ToString().ToUpper() + "' WHERE cortecaja = 0 AND id_caja = " + id_caja.ToString();
                    stringConexionMySql.EjecutarCommando(cInst, DB);

                    cMensaje = "¡Corte realizado con exito!";
                    return cMensaje;
                }
            }
            catch
            {
                string cMensaje = "Ha ocurrido un error, por favor revise...";
                return cMensaje;
            }
            
        }



        public ActionResult SeleccionarCaja()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string Base_Datos = (string)(Session["StringConexion"]);
            StringConexionMySQL mysql = new StringConexionMySQL();
            //DROPDOWNLIST DE CAJAS REGISTRADAS AL USUARIO
            List<SelectListItem> listadoCajas = new List<SelectListItem>();
            //string validarCajas = "select cu.id_caja, c.nombre from cajas_usuarios cu ; //inner join cajas c on c.id_caja = cu.id_caja "; // where estatus = 1 and cu.id_usuario = '" + Session["Usuario"] + "'";
            string validarCajas = "select id_caja, id_usuario from cajas_usuarios"; //inner join cajas c on c.id_caja = cu.id_caja "; // where estatus = 1 and cu.id_usuario = '" + Session["Usuario"] + "'";
            ConexionMySQL llenarCajas = new ConexionMySQL();
            llenarCajas.LLenarDropDownList(validarCajas, Base_Datos, listadoCajas);
            ViewData["Cajas"] = listadoCajas;
            llenarCajas.CerrarConexion();

            return View("~/Views/POS/Operaciones/SeleccionarCaja.cshtml");
        }



        [HttpPost]
        public string Asignar(Caja caja)
        {
            try
            {
                var idCajaAsignada = caja.id_caja;
                var nombreCajaAsignada = caja.Nombre;
                Session["IDCajaAsignada"] = idCajaAsignada;
                return "caja asignada con exito...";
            }
            catch (Exception ex)
            {
                return "No se pudo asignar";
            }

        }


        
        [HttpPost]
        public string GetDataCliente(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            StringConexionMySQL stringConexionMySql2 = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]);            
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];

            string sentenciaSQL1 = "SELECT json_object('CLIENTE', cliente," + "'DIRECCION', direccion," + "'NIT', nit," + "'DIASCRED',diascred, " + "'CORREO', email, " + "'FACTURAR', facturar " + ") AS JSON FROM clientes " + " WHERE id_codigo = " + datos.id;
            string sentenciaSQL2 = "SELECT json_object('CLIENTE', cliente, 'DIRECCION', direccion, 'NIT', nit, 'DIASCRED',diascred, " + "'CORREO', email, " + "'FACTURAR', facturar " + ") AS JSON FROM clientes WHERE nit = '" + datos.id + "'";



            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = sentenciaSQL1,
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
            else 
            {
                queryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = sentenciaSQL2,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(queryapi);

                cRespuesta = Funciones.EqAppQuery(jsonString);

                Resultado resultado2 = new Resultado();
                resultado2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta);
                if (resultado2.resultado.ToString() == "true")
                {
                    str = resultado2.Data[0].JSON.ToString();
                }
                else
                {
                            Funciones f = new Funciones();
                            str = f.GetDataNit(datos.id);
                           
                }

            }
            if (str=="")
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }


            
            return str;
        }



        [HttpPost]
        public string GetDataInventario(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];
            string id_agencia = (string)this.Session["Sucursal"];
            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            string oBaseDatos = (string)this.Session["oBase"];
            string cNit = (string)this.Session["cNit"];

            
            try
            {
                string sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'PRODUCTO', producto," + "'PRECIO', precio1," + "'CODIGOE', codigoe," + "'NOPARTE', numero_departe, " + "'MARCA', id_marca, " + "'EXISTENCIA', ifnull((SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia=" + id_agencia.ToString() + "),0), " + "'SERVICIO', servicio, " + "'COSTO1', costo1) AS JSON" + " FROM inventario a " + " WHERE id_codigo = '" + datos.id+"'";

                EqAppQuery queryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = sentenciaSQL1,
                    BaseDatos = oBaseDatos

                };

                string jsonString = JsonConvert.SerializeObject(queryapi);

                string cRespuesta = Funciones.EqAppQuery(jsonString);
                                

                Resultado resultado = new Resultado();                
                resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString()) ;

                if (resultado.resultado.ToString()=="true")
                {
                    str = resultado.Data[0].JSON.ToString();
                }
                else    // si no encuentra con codigo noraml
                {
                    sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'PRODUCTO', producto," + "'PRECIO', precio1," + "'CODIGOE', codigoe," + "'NOPARTE', numero_departe, " + "'MARCA', id_marca, " + "'EXISTENCIA', ifnull((SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia=" + id_agencia.ToString() + "),0), " + "'SERVICIO', servicio, " + "'COSTO1', costo1) as JSON " + " FROM inventario a " + " WHERE codigoe = '" + datos.id.Trim() + "'";

                    queryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = sentenciaSQL1,
                        BaseDatos = oBaseDatos

                    };

                    jsonString = JsonConvert.SerializeObject(queryapi);

                    cRespuesta = Funciones.EqAppQuery(jsonString);

                    Resultado resultado2 = new Resultado();
                    resultado2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta);
                    if (resultado2.resultado.ToString() == "true")
                    {
                        str = resultado2.Data[0].JSON.ToString();
                    }
                    else   // si no encontro con codigo equivalente
                    {
                        sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'PRODUCTO', producto," + "'PRECIO', precio1," + "'CODIGOE', codigoe, " + "'NOPARTE', numero_departe, " + "'MARCA', id_marca, " + "'EXISTENCIA',  ifnull((SELECT sum(b.entrada-b.salida) FROM kardexinven b WHERE b.id_codigo= a.id_codigo AND b.id_agencia=" + id_agencia.ToString() + "),0), " + "'SERVICIO', servicio, " + "'COSTO1', costo1) as JSON " + " FROM inventario a " + " WHERE numero_departe = '" + datos.id.Trim() + "'";

                        queryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = sentenciaSQL1,
                            BaseDatos = oBaseDatos

                        };

                        jsonString = JsonConvert.SerializeObject(queryapi);

                        cRespuesta = Funciones.EqAppQuery(jsonString);


                        Resultado resultado3 = new Resultado();
                        resultado3 = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta);
                        if (resultado3.resultado.ToString() == "true")
                        {
                            str = resultado3.Data[0].JSON.ToString();
                        }
                    }
                }
                                
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }



            //stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string GetDataPrecioClie(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string sentenciaSQL1 = "SELECT json_object('PRECIO', precio  ) FROM preciosclientes WHERE id_codigo = " + datos.id +" AND id_cliente = "+datos.nit;

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                    stringConexionMySql.CerrarConexion();
                }
                else
                {
                        str = "{\"PRECIO\": \"ERROR\"}";
                    
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }



            stringConexionMySql.CerrarConexion();
            return str;
        }

        //agregado 25/02/2023  FUNCION FACTURAR 
        [HttpPost]
        public string InsertarDocumento(DatosProspecto datos, string id_orden, string serie_orden)
        {
            int formaPago = datos.formaPago;
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            //string id_orden = datos.id_orden;
            //string serie_orden = datos.serie_orden;

            string jeje1 = id_orden;
            string jeje2 = serie_orden;



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

            string cNit = (string)this.Session["cNit"];
            string cEmisor = (string)this.Session["cNombreEmisor"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cAfiliacion = (string)this.Session["cAfiliacion"];
            string cEmail = (string)this.Session["cEmail"];
            string cDireccion = (string)this.Session["cDireccion"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];

            string certificador = (string)this.Session["certificador"];
            string cTokenFel = (string)this.Session["tokenfel"];
            string jSonWebApp = (string)this.Session["jSonWebApp"];
            string cCampo6    = (string)this.Session["cCampo6"];
            string id_agencia = (string)this.Session["Sucursal"];

            string cPersoneria = (string)this.Session["cPersoneria"];

            string cUser = Session["Usuario"].ToString();

            string cFrases = (string)this.Session["cCampo1"];

            string cUpdate = "";

            bool lgetDigi = false;
            string pageurl = "";
            string Guid = "";

            string cExp = "";

            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            string oBaseDatos = (string)this.Session["oBase"];

            ResultCrud errorcrud = new ResultCrud();


            int nDocumento = 0;
            bool lError = false;
            string cError = "Hubo un error en el ingreso, favor revisar...";
            int nContador = 1;
            double nCosto = 0.00;
            
            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();
            FelEstructura.Adenda adenda = new FelEstructura.Adenda();
            FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();
            FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();
            FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();

            string cId_Interno = "";

            int nLinea = 0;
            string cServicio = "";
            double nPrecioSD = 0.00;
            double nMontoGravable = 0.00;
            double nMontoImpuesto = 0.00;
            double nMontoTotalImpuesto = 0.00;

            DateTime tiempo1 = DateTime.Now;
            DateTime tiempo2 = DateTime.Now;
            string difTiempo = "";

            DateTime hora = DateTime.Now;
            string solo_hora = hora.ToString("HH:mm:ss");

            string cDtalle = datos.cdetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/");
            XmlDocument xmlDoc = new XmlDocument();
            
            /// este es el xml del EQ guardarlo en una ruta x
            xmlDoc.LoadXml(cDtalle);


          //  System.IO.File.WriteAllText("C:\\API\\EqXml" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);


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
                nTotal = nTotal + Convert.ToDouble(node.ChildNodes[7].InnerText);

            }

            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList tipoVenta = xmlDoc.GetElementsByTagName("TIPOVENTA");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");
            XmlNodeList serie = xmlDoc.GetElementsByTagName("SERIE");
            XmlNodeList cCorreo = xmlDoc.GetElementsByTagName("CORREO");
            XmlNodeList cExtranjero = xmlDoc.GetElementsByTagName("EXTRANJERO");
            XmlNodeList cEstablecimiento = xmlDoc.GetElementsByTagName("ESTABLECIMIENTO");
            XmlNodeList cExportacion = xmlDoc.GetElementsByTagName("EXPORTACION");
            XmlNodeList cMoneda = xmlDoc.GetElementsByTagName("MONEDA");

            /// variables para exportacion

            XmlNodeList cNombreDestinatario    = xmlDoc.GetElementsByTagName("NOMBREDESTINATARIO");
            XmlNodeList cDireccionDestinatario = xmlDoc.GetElementsByTagName("DIRECCIONDESTINATARIO");
            XmlNodeList cIncoterm             = xmlDoc.GetElementsByTagName("INCOTERM");
            XmlNodeList cCodigoConsignatario   = xmlDoc.GetElementsByTagName("CODIGOCONSIGNATARIO");
            XmlNodeList cNombreComprador       = xmlDoc.GetElementsByTagName("NOMBRECOMPRADOR");
            XmlNodeList cDireccionComprador    = xmlDoc.GetElementsByTagName("DIRECCIONCOMPRADOR");
            XmlNodeList cCodigoComprador       = xmlDoc.GetElementsByTagName("CODIGOCOMPRADOR");
            XmlNodeList cNombreExportador      = xmlDoc.GetElementsByTagName("NOMBREEXPORTADOR");
            XmlNodeList cCodigoExportador      = xmlDoc.GetElementsByTagName("CODIGOEXPORTADOR");
            XmlNodeList cOtraReferencia        = xmlDoc.GetElementsByTagName("OTRAREFERENCIA");
            XmlNodeList escenario = xmlDoc.GetElementsByTagName("ESCENARIO");

            cex.NombreDistanatario = cNombreDestinatario[0].InnerText;
            cex.DireccionDestinatario = cDireccionDestinatario[0].InnerText;
            cex.Incoterm = cIncoterm[0].InnerText;
            cex.CodigoConsignatario = cCodigoConsignatario[0].InnerText;
            cex.NombreComprador = cNombreComprador[0].InnerText;
            cex.DireccionComprador = cDireccionComprador[0].InnerText;
            cex.CodigoComprador = cCodigoComprador[0].InnerText;
            cex.NombreExportador = cNombreExportador[0].InnerText;
            cex.CodigoExportador = cCodigoExportador[0].InnerText;
            cex.OtraReferencia = cOtraReferencia[0].InnerText;




            // Aca vamos a tomar el codigo de escenario, para armar la frase de exento
            if (Convert.ToInt32(escenario[0].InnerText) != 0)
            {
                cFrases = cFrases.Replace("</dte:Frases>", "<dte:Frase TipoFrase=\"4\" CodigoEscenario=\"" + escenario[0].InnerText.Trim() + "\"/></dte:Frases>");
            }





            /// aca vamos hacer que ingrese una factura normal comun y corriente

            if (tipodocto[0].InnerText == "FACTU")
            {
                cInst = String.Format("SELECT IFNULL(max({0}), 0) AS JSON ", "no_factura");
                cInst += String.Format("FROM {0} ", "facturas");
                //cInst += "where id_agencia = " + id_agencia.ToString() + " and serie='" + serie[0].InnerText + "'";
                cInst += "where serie='" + serie[0].InnerText + "'";

                EqAppQuery queryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                string jsonString = JsonConvert.SerializeObject(queryapi);

                // llamar al api 

                string cRespuesta = Funciones.EqAppQuery(jsonString);


                Resultado resultado = new Resultado();
                resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                if (resultado.resultado.ToString() == "true")
                {
                    double ndocu = Convert.ToDouble(resultado.Data[0].JSON);
                    nDocumento = Convert.ToInt32(ndocu) + 1;
                }




                System.IO.File.WriteAllText("C:\\API\\EqXml" +nit[0].InnerText+"idintero"+nDocumento.ToString()+""+ hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);


                stringConexionMySql.ExecAnotherCommand("START TRANSACTION", DB);


                

                double EFECTIVO = 0.00, TARJETA = 0.00, CHEQUE = 0.00, VALE = 0.00, TRANSFERENCIA = 0.00;

                switch (formaPago)
                {
                    case 1:
                        EFECTIVO = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 2:
                        TARJETA = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 3:
                        CHEQUE = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 4:
                        VALE = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 5:
                        TRANSFERENCIA = Convert.ToDouble(nTotal.ToString());
                        break;
                }


              

                if (certificador == "G4S")
                {
                    cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,cont_cred,id_caja,efectivo,tarjeta,vale,cheque,transferencia,hechopor,tdescto) ";
                    cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}') ",
                     nDocumento.ToString(),
                     serie[0].InnerText,
                     fechadocto[0].InnerText,
                     "I",
                     id_cliente[0].InnerText,
                     cliente[0].InnerText,
                     vendedor[0].InnerText,
                     direccion[0].InnerText,
                     nTotal.ToString(),
                     obs[0].InnerText,
                     cEstablecimiento[0].InnerText,
                     nit[0].InnerText,
                     fechadocto[0].InnerText,
                     dias[0].InnerText.ToString(),
                     tipoVenta[0].InnerText,
                     Session["IDCajaAsignada"].ToString(),
                     EFECTIVO.ToString(),
                     TARJETA.ToString(),
                     VALE.ToString(),
                     CHEQUE.ToString(),
                     TRANSFERENCIA.ToString(),
                     cUser,
                     nDescto.ToString());
                }
                else
                {
                    cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,cont_cred,id_caja,efectivo,tarjeta,vale,cheque,transferencia,hechopor,tdescto) ";
                    cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}') ",
                     nDocumento.ToString(),
                     serie[0].InnerText,
                     fechadocto[0].InnerText,
                     "I",
                     id_cliente[0].InnerText,
                     cliente[0].InnerText,
                     vendedor[0].InnerText,
                     direccion[0].InnerText,
                     nTotal.ToString(),
                     obs[0].InnerText,
                     id_agencia.ToString(),
                     nit[0].InnerText,
                     fechadocto[0].InnerText,
                     dias[0].InnerText.ToString(),
                     tipoVenta[0].InnerText,
                     Session["IDCajaAsignada"].ToString(),
                     EFECTIVO.ToString(),
                     TARJETA.ToString(),
                     VALE.ToString(),
                     CHEQUE.ToString(),
                     TRANSFERENCIA.ToString(),
                     cUser,
                     nDescto.ToString());
                }

                EqAppQuery insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                 jsonString = JsonConvert.SerializeObject(insertqueryapi);
                 cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadoinsert = new ResultCrud();
                 
                resultadoinsert = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadoinsert.resultado.ToString() == "false")
                {
                    cError = resultadoinsert.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    errorcrud = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                    return cError;
                }

                bool errorSP = false;



                //VAMOS A VALIDAR QUE SI LA VENTA ES AL CONTADO NO INGRESA DATOS A LA TABLA CTACC
                if (tipoVenta[0].InnerText == "2")
                {

                    cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                                   id_cliente[0].InnerText, nDocumento.ToString(), serie[0].InnerText, 1, fechadocto[0].InnerText, fechadocto[0].InnerText, dias[0].InnerText.ToString(), fechadocto[0].InnerText,
                                                    nTotal.ToString(), nTotal.ToString(), "Operado en Modulo Web", cUser, "FACTURA", 1);

                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };

                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                    ResultCrud resultadoctacc = new ResultCrud();

                    resultadoctacc = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                    if (resultadoctacc.resultado.ToString() == "false")
                    {
                        cError = resultadoctacc.data;

                        cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBaseDatos

                        };
                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                        errorcrud = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());
                        

                        return cError;
                    }
                   

                }

                // vamos a insertar el detalle de la factura y kardex y denas
                // 
                nContador = 0;
                cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) VALUES ";
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {
                    byte[] bytes = Convert.FromBase64String(node.ChildNodes[3].InnerText.ToString());
                    string txtDescrip = Encoding.UTF8.GetString(bytes);

                    producto = txtDescrip; //Funciones.Base64Decode(node.ChildNodes[3].InnerText).Replace("<pre>", "");

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;                    
                    
                    producto = producto.ToString().Replace("/", "|");
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[7].InnerText;

                    if (descto.ToString() == "")
                    {
                        descto = "0";
                    }


                    if (certificador == "G4S")
                    {
                        cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), serie[0].InnerText, codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString().Replace("'", ""), cEstablecimiento[0].ToString());
                    }
                    else
                    {
                        cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), serie[0].InnerText, codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString().Replace("'", ""), id_agencia.ToString());
                    }
                    
                    

                    nContador++;
                }

                cInst = cInst.Replace(") (", "),(");
                insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(insertqueryapi);
                cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadodetfactura = new ResultCrud();

                resultadodetfactura = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadodetfactura.resultado.ToString() == "false")
                {
                    cError = resultadodetfactura.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    return cError;
                }


                // vamos a insertar el detalle del kardex
                // 

                nContador = 0;
                cInst = "INSERT INTO kardexinven ( id_codigo, id_agencia, fecha, id_movi, docto, serie, obs, hechopor, salida, costo1,precio, codigoemp,correlativo ) VALUES";                
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[7].InnerText;
                   try
                    {
                       nCosto = Convert.ToDouble(f.ObtieneDatos("inventario", "costo2", "id_codigo = " + codigo.ToString(), _BaseDatos));
                    }
                    catch
                    {
                        nCosto = 0.00;
                    }

                    

                    //// esto si puede tronar por el constraint de las agencias(catagencias)
                    if (certificador == "G4S")
                    {
                        cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "51", nDocumento.ToString(), serie[0].InnerText, "Ingresado en modulo sistema Web", cUser, cantidad.ToString().Replace(",", ""), nCosto.ToString(), precio.ToString().Replace(",", ""), id_cliente[0].InnerText, nContador.ToString());
                    }
                    else
                    {
                        cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "51", nDocumento.ToString(), serie[0].InnerText, "Ingresado en modulo sistema Web", cUser, cantidad.ToString().Replace(",", ""), nCosto.ToString(), precio.ToString().Replace(",", ""), id_cliente[0].InnerText, nContador.ToString());
                    }
                   
                    nContador++;
                }
                cInst = cInst.Replace(") (", "),(");

                insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(insertqueryapi);
                cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadokardexinven = new ResultCrud();

                resultadokardexinven = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadokardexinven.resultado.ToString() == "false")
                {
                    cError = resultadokardexinven.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    return cError;
                }


                // vamos a buscr los establecimientos jponce

                if (certificador == "G4S")
                {
                    var wsEstablecimiento = wsConnector.wsEnvio("GET_ESTABLECIMIENTOS", cEstablecimiento[0].InnerText, "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

                    JArray jsonParseo = JArray.Parse(wsEstablecimiento[2].ToString());
                    foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                    {
                        
                        foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                        {
                            

                            datosEmisor.NombreComercial = jsonOperaciones["Nombre"].ToString();
                            //cExp = jsonOperaciones["exportacion"].ToString();
                            if (_BaseDatos.Trim() == "kenichi")
                            {
                                datosEmisor.Tipo = "";
                            }
                            else
                            { 
                                 datosEmisor.Tipo = serie[0].InnerText;
                            }

                            datosEmisor.Direccion = jsonOperaciones["Direccion"].ToString().ToUpper();
                            datosEmisor.CodigoEstablecimiento = jsonOperaciones["id_establecimiento"].ToString();
                            datosEmisor.Municipio = jsonOperaciones["Municipio"].ToString().ToUpper();
                            datosEmisor.Departamento = jsonOperaciones["Departamento"].ToString().ToUpper();
                            //adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();


                        }


                    }

                    cInst = "SELECT exportacion,tipoactivo,sucursal,direccion1,direccion2,codigoestablecimiento,leyendafacc,municipio,departamento FROM resolucionessat WHERE serie ='" + serie[0].InnerText + "'";

                    queryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };

                    jsonString = JsonConvert.SerializeObject(queryapi);

                    cRespuesta = Funciones.EqAppQuery(jsonString);


                    Resultado resolucionesat = new Resultado();
                    resolucionesat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                    //if (resultado.resultado.ToString() == "true")
                    if (resolucionesat.resultado.ToString() == "true")
                    {
                        cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                        jsonParseo = JArray.Parse(cRespuesta);

                        foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                        {

                            cExp = cExportacion[0].InnerText;//jsonOperaciones["exportacion"].ToString();
                            adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();
                            if (_BaseDatos.Trim() == "kenichi")
                            {
                                datosEmisor.Tipo = jsonOperaciones["tipoactivo"].ToString();
                            }

                        }

                    }


                }
                else
                {
                    //vamos a buscar todos los datos en resolucciones sat para construir el xml 
                    cInst = "SELECT exportacion,tipoactivo,sucursal,direccion1,direccion2,codigoestablecimiento,leyendafacc,municipio,departamento FROM resolucionessat WHERE serie ='" + serie[0].InnerText + "'";

                    queryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };

                    jsonString = JsonConvert.SerializeObject(queryapi);

                    cRespuesta = Funciones.EqAppQuery(jsonString);


                    Resultado resolucionesat = new Resultado();
                    resolucionesat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                    //if (resultado.resultado.ToString() == "true")
                    if (resolucionesat.resultado.ToString() == "true")
                    {
                        cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                        JArray jsonParseo = JArray.Parse(cRespuesta);

                        foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                        {
                            //string base_datos = jsonOperaciones["base_datos"].ToString();
                            //string nombre_empresa = 
                            datosEmisor.NombreComercial = jsonOperaciones["sucursal"].ToString();
                            //cExp = jsonOperaciones["exportacion"].ToString();
                            cExp = cExportacion[0].InnerText;
                            datosEmisor.Tipo = jsonOperaciones["tipoactivo"].ToString();
                            datosEmisor.Direccion = jsonOperaciones["direccion1"].ToString() + " " + jsonOperaciones["direccion2"].ToString(); ;
                            datosEmisor.CodigoEstablecimiento = jsonOperaciones["codigoestablecimiento"].ToString();
                            datosEmisor.Municipio = jsonOperaciones["municipio"].ToString();
                            datosEmisor.Departamento = jsonOperaciones["departamento"].ToString();
                            adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();

                        }

                    }

                }



                

                if (datosEmisor.Tipo == "FCAM")
                {
                    cfc.NumeroAbono = "1";
                    cfc.FechaVencimiento = fechadocto[0].InnerText;
                    cfc.MontoAbono = nTotal.ToString();

                }

                if (datosEmisor.Tipo == "RDON")
                {
                    datosEmisor.TipoPersoneria = cPersoneria;

                }

                double nretencion = 0;
                double nretencioniva = 0;
                double ntotalretenciones = 0;

                if (datosEmisor.Tipo == "FESP")
                {
                    cFrases = "";
                    nretencion = (nTotal / 1.12) * 0.05;
                    nretencioniva = (nTotal / 1.12) * 0.12;
                    ntotalretenciones = nTotal - nretencion - nretencioniva;

                    cfe.RetencionISR = f.FormatoDecimal(nretencion.ToString(), 6, false).Replace(",", "");
                    cfe.RetencionIVA = f.FormatoDecimal(nretencioniva.ToString(), 6, false).Replace(",", "");
                    cfe.TotalMenosRetenciones = f.FormatoDecimal(ntotalretenciones.ToString(), 6, false).Replace(",", "");


                }



                datosEmisor.FechaHoraEmision = fechadocto[0].InnerText + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
                //if (cExp == "2")
                //{
                //    datosEmisor.CodigoMoneda = "GTQ";

                //}
                //else if (cExp == "1")
                //{
                //    datosEmisor.cExp = "SI";
                //    datosEmisor.CodigoMoneda = "USD";

                //}
                //else
                //{
                //    datosEmisor.CodigoMoneda = "USD";
                //}


                if (cExp == "1")
                {
                    datosEmisor.cExp = "SI";                    

                }

                datosEmisor.CodigoMoneda = cMoneda[0].InnerText;




                datosEmisor.cCorreos = cCorreo[0].InnerText;// f.ObtieneDatos("clientes", "email", "id_codigo = " + id_cliente[0].InnerText, DB);
                datosEmisor.NitEmisor = cNit;
                datosEmisor.NombreEmisor = cEmisor;
                
                datosEmisor.AfiliacionIva = cAfiliacion;
                datosEmisor.CorreoEmisor = cEmail;
                
                
                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Pais = "GT";
                datosEmisor.CodigoPostal = "01011";
                
                datosEmisor.Pais = "GT";

                

                cId_Interno = serie[0].InnerText + "-" + nDocumento.ToString();

                datosReceptor.IdReceptor = nit[0].InnerText.ToString().Replace("-", "");
                datosReceptor.NombreReceptor = cliente[0].InnerText;
                if (certificador.ToString() == "DIGIFACT")
                {
                    datosReceptor.CorreoReceptor = datosEmisor.cCorreos;
                }
                else
                {
                    datosReceptor.CorreoReceptor = "";
                }
                datosReceptor.Direccion = direccion[0].InnerText;
                datosReceptor.CodigoPostal = "01011";
                //datosReceptor.Municipio = "GUATEMALA";
                //datosReceptor.Departamento = "GUATEMALA";

                datosReceptor.Municipio = "";
                datosReceptor.Departamento = "";

                datosReceptor.Pais = "GT";

                if (cExtranjero[0].InnerText=="S")
                {
                    datosReceptor.TipoEspecial = "EXT";

                }
                else
                {
                    datosReceptor.TipoEspecial = "";
                }

                // frases[0].TipoFrase = "1";
                // frases[0].CodigoEscenario = "1";

                FelEstructura.Items[] items = new FelEstructura.Items[cXmlDetalle.Count];
                nLinea = 0;
                cServicio = "";
                nPrecioSD = 0.00;
                nMontoGravable = 0.00;
                nMontoImpuesto = 0.00;
                nContador = 1;
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {
                    codigo = node.ChildNodes[0].InnerText;
                    //codigoe = node.ChildNodes[1].InnerText;
                    //producto = node.ChildNodes[3].InnerText;

                    // esto es para poder respetar tildes y enes
                    byte[] bytes = Convert.FromBase64String(node.ChildNodes[3].InnerText.ToString());
                    string txtDescrip = Encoding.UTF8.GetString(bytes);

                    // aca ya se lee en UTF8
                    //producto = Funciones.Base64Decode(txtDescrip).Replace("<pre>", "");

                    producto =txtDescrip.Replace("<pre>", "");

                    //producto = producto.ToString().Replace("/", "|");

                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[7].InnerText;

                    cServicio = f.ObtieneDatos("inventario", "servicio", "id_codigo=" + codigo, _BaseDatos);  //node.ChildNodes[1].InnerText;  //f.ObtieneDatos("inventario", "servicio", "id_codigo=" + codigo, DB);
                    if (cServicio.Trim() == "N")
                    {

                        items[nLinea].BienOServicio = "B";
                    }
                    else
                    {
                        items[nLinea].BienOServicio = "S";
                    }
                    if (datosEmisor.cExp == "SI")
                    {
                        items[nLinea].NumeroLinea = nContador.ToString();
                        items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                        items[nLinea].UnidadMedida = "UNI";
                        items[nLinea].Descripcion = producto.Replace("/", "|");
                        items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                        items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                        items[nLinea].NombreCorto = "IVA";
                        items[nLinea].CodigoUnidadGravable = "2";                        
                        items[nLinea].MontoGravable = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        items[nLinea].MontoImpuesto = "0.00";
                        items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        nLinea = nLinea + 1;
                        nContador = nContador + 1;
                    }
                    else
                    {
                        if (node.ChildNodes[6].InnerText == "12")
                        {
                            items[nLinea].NumeroLinea = nContador.ToString();
                            items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                            items[nLinea].UnidadMedida = "UNI";
                            items[nLinea].Descripcion = producto.Replace("/", "|");
                            items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                            nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                            items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                            items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                            items[nLinea].NombreCorto = "IVA";
                            items[nLinea].CodigoUnidadGravable = "1";
                            nMontoGravable = Convert.ToDouble(subtotal) / 1.12;
                            nMontoImpuesto = nMontoGravable * 0.12;

                            nMontoTotalImpuesto += nMontoGravable;

                            items[nLinea].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                            items[nLinea].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                            items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                            nLinea = nLinea + 1;
                            nContador = nContador + 1;
                        }
                        else
                        {
                            items[nLinea].NumeroLinea = nContador.ToString();
                            items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                            items[nLinea].UnidadMedida = "UNI";
                            items[nLinea].Descripcion = producto.Replace("/", "|");
                            items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                            nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                            items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                            items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                            items[nLinea].NombreCorto = "IVA";
                            items[nLinea].CodigoUnidadGravable = "2";
                            nMontoGravable = Convert.ToDouble(subtotal);
                            nMontoImpuesto = 0;
                            items[nLinea].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                            items[nLinea].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                            items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                            nLinea = nLinea + 1;
                            nContador = nContador + 1;

                        }
                    }
                }


                //nMontoTotalImpuesto = Convert.ToDouble(nTotal.ToString()) / 1.12;
                nMontoTotalImpuesto = nMontoTotalImpuesto * 0.12;

                if (datosEmisor.cExp == "SI")
                {
                    totales.NombreCorto = "IVA";
                    totales.TotalMontoImpuesto = "0.00";
                    totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");
                }
                else
                {
                    totales.NombreCorto = "IVA";
                    totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoTotalImpuesto.ToString(), 6, false).Replace(",", "");
                    totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");
                    
                }

                FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();

                //COMENTADO PARA UTILIZAR DESPUES EL WEB SERVICE DE FACTURA ELECTRONICA
                
                var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, nTotal.ToString(), serie[0].InnerText, adenda, cfc, certificador,cnc,cfe,cex);



                Dte = Dte.Replace("{CONT}", "");
                Dte = Dte.Replace("&", "&amp;");


                //Dte de la sat ya es el xml buscar guardarlo en alguna parte.

                System.IO.File.WriteAllText("C:\\XML\\Dte" + nit[0].InnerText + "idinterno" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", Dte);



                if (certificador.ToString() == "DIGIFACT")
                {
                    tiempo2 = DateTime.Now;

                    var wsEnvio = wsConnector.wsEnvioDigifact(cUrlFel, cTokenFel, Dte);

                    if (wsEnvio == "Error")
                    {
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        cError = "Ha ocurrido un error en la cerificacion";

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }

                    RespuestaFelDigifact respuesta = JsonConvert.DeserializeObject<RespuestaFelDigifact>(wsEnvio.ToString());

                    if (respuesta.Mensaje.Trim()== "La transformación XSLT falló.")
                    {
                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }


                    if (respuesta.Autorizacion.Trim() == "")
                    {
                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }

                    if (respuesta.Codigo == 3010)
                    {
                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }




                    if (respuesta.Codigo==9022)
                    {
                        //string cExtrae = Funciones.ExtraerInfo(respuesta.ResponseDATA1.ToString(), "UUID", ", a");

                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();

                        cCampo6 = cCampo6.Replace("{ID}", cId_Interno);
                        //cCampo6 = cCampo6.Replace("{ID}", "DIG-6");
                        cCampo6 = cCampo6.Replace("{FECHA}", fechadocto[0].InnerText);
                        wsEnvio = wsConnector.wsGetDigifact(cCampo6, cTokenFel);

                        
                        ResponseGet responseget = JsonConvert.DeserializeObject<ResponseGet>(wsEnvio.ToString());

                        if (Convert.ToDouble(responseget.RESPONSE[0].TOTAL) == Convert.ToDouble(nTotal))
                        {
                            if (responseget.RESPONSE[0].NIT_COMPRADOR.Trim() == nit[0].InnerText.ToString().Replace("-", "").Trim())
                            {
                                difTiempo = (tiempo2 - tiempo1).ToString();
                                cUpdate = "UPDATE facturas SET no_docto_fel ='" + responseget.RESPONSE[0].NUMERO + "',";
                                cUpdate += " serie_docto_fel ='" + responseget.RESPONSE[0].SERIE + "',";
                                cUpdate += " firmaelectronica = '" + responseget.RESPONSE[0].GUID + "',";
                                cUpdate += " fecha_certificacion ='" + responseget.RESPONSE[0].FECHA_DE_CERTIFICACION + "',";
                                cUpdate += " impresa ='N',";
                                cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                                cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                                cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                                cUpdate += " AND id_agencia = "+ id_agencia.ToString();

                                lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                                if (lError == true)
                                {

                                    //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                                    //stringConexionMySql.CerrarConexion();

                                    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                                    //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                                    //List<string> listaEliminarVenta = new List<string>();
                                    //listaEliminarVenta.Add(nDocumento.ToString());
                                    //listaEliminarVenta.Add(serie[0].InnerText);
                                    //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                                    insertqueryapi = new EqAppQuery()
                                    {
                                        Nit = cNit,
                                        Query = cInst,
                                        BaseDatos = oBaseDatos

                                    };
                                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                                    return cError;

                                }


                                Guid = responseget.RESPONSE[0].GUID.Trim();
                                lgetDigi = true;
                            }


                        }
                        else
                        {
                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            // stringConexionMySql.CerrarConexion();


                            //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                            //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                            //List<string> listaEliminarVenta = new List<string>();
                            //listaEliminarVenta.Add(nDocumento.ToString());
                            //listaEliminarVenta.Add(serie[0].InnerText);
                            //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);


                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }



                    }

                    if (lgetDigi == false)
                    {
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + respuesta.NUMERO + "',";
                        cUpdate += " serie_docto_fel ='" + respuesta.Serie + "',";
                        cUpdate += " firmaelectronica = '" + respuesta.Autorizacion + "',";
                        cUpdate += " fecha_certificacion ='" + respuesta.Fecha_de_certificacion + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        cUpdate += " AND id_agencia = "+ id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            // stringConexionMySql.CerrarConexion();

                            //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                            // string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                            // List<string> listaEliminarVenta = new List<string>();
                            // listaEliminarVenta.Add(nDocumento.ToString());
                            // listaEliminarVenta.Add(serie[0].InnerText);
                            // errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }
                    }


                    pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";
                    pageurl = pageurl.Replace("{NITEMISOR}", cNit);

                    if (lgetDigi == false)
                    {
                        pageurl = pageurl.Replace("{UUID}", respuesta.Autorizacion);
                    }
                    else
                    {
                        pageurl = pageurl.Replace("{UUID}", Guid);
                    }

                    Response.Write("<script> window.open('" + pageurl + "','_blank'); </script>");

                }


                if (certificador.ToString() == "G4S")
                {
                    var wsEnvio = wsConnector.wsEnvio("POST_DOCUMENT_SAT", Funciones.Base64Encode(Dte), cId_Interno, cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

                    
                    if (wsEnvio[1].ToString().ToUpper() == "TRUE")
                    {

                        tiempo2 = DateTime.Now;
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + wsEnvio[4].ToString() + "',";
                        cUpdate += " serie_docto_fel ='" + wsEnvio[3].ToString() + "',";
                        cUpdate += "xmlFac='" + wsEnvio[2].ToString() + "',";
                        cUpdate += " firmaelectronica = '" + wsEnvio[6].ToString() + "',";
                        cUpdate += " fecha_certificacion ='" + wsEnvio[7].ToString() + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        //cUpdate += " AND id_agencia = "+ id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                             stringConexionMySql.CerrarConexion();

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }

                        //stringConexionMySql.CerrarConexion();

                    }
                    else if (wsEnvio[2].ToString().Contains("PK_MASTERINDEX"))
                    {
                        wsEnvio = wsConnector.wsObtener(cId_Interno, "", "XML", cUserFe, cUrlFel, cToken, "LOOKUP_ISSUED_INTERNAL_ID", "GT", cNit, false, "");
                        wsEnvio = wsConnector.wsEnvio(wsEnvio[6], "", "XML", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, false, "");
                        tiempo2 = DateTime.Now;
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + wsEnvio[4].ToString() + "',";
                        cUpdate += " serie_docto_fel ='" + wsEnvio[3].ToString() + "',";
                        cUpdate += "xmlFac='" + wsEnvio[2].ToString() + "',";
                        cUpdate += " firmaelectronica = '" + wsEnvio[6].ToString() + "',";
                        cUpdate += " fecha_certificacion ='" + wsEnvio[7].ToString() + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        //cUpdate += " AND id_agencia = "+ id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            stringConexionMySql.CerrarConexion();


                            // asi no borra y pega la firma
                            //cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            //insertqueryapi = new EqAppQuery()
                            //{
                            //    Nit = cNit,
                            //    Query = cInst,
                            //    BaseDatos = oBaseDatos

                            //};
                            //jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            //cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }

                        //stringConexionMySql.CerrarConexion();

                    }
                    else
                    {
                        //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        cError = wsEnvio[2].ToString();


                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        //List<string> listaEliminarVenta = new List<string>();
                        //listaEliminarVenta.Add(nDocumento.ToString());
                        //listaEliminarVenta.Add(serie[0].InnerText);
                        //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta,ref cError);

                        cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBaseDatos

                        };
                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                        return cError;


                    }


                    // vamos hacer la prueba con el link
                    //string cNombreDocumento = wsEnvio[6] + ".pdf";

                    //cNombreDocumento = "c:\\PDF\\" + wsEnvio[6] + ".pdf";


                    //string cRutaPdf = cNombreDocumento;
                    //System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                    //oFileStream.Write(Base64String_ByteArray(wsEnvio[9]), 0, Base64String_ByteArray(wsEnvio[9]).Length);
                    //oFileStream.Close();
                    //pageurl = "/Rgp/" + wsEnvio[6].ToString() + ".pdf";



                    pageurl = wsEnvio[10].ToString();
                    Response.Write("<script>window.open('" + pageurl + "','_blank'); </script>");
                }


                //VAMOS A VALIDAR QUE SI LA VENTA ES UNA ORDEN DE SERVICIO COCA
                if (id_orden != "")
                {
                    if (serie_orden != "")
                    {
                        cInst = "UPDATE ordenserv SET status = 'T' WHERE no_factura = " + datos.id_orden + "AND serie = '" + datos.serie_orden + "'";

                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBaseDatos

                        };

                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                        ResultCrud resultadoctacc = new ResultCrud();

                        resultadoctacc = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                        if (resultadoctacc.resultado.ToString() == "false")
                        {
                            cError = resultadoctacc.data;

                            return cError;
                        }
                    }   
                }


                stringConexionMySql.KillAllMySQL(DB);
                // puse pipe por que ? ni ide de por que siempre poner el pipe
                cMensaje = "|DOCUMENTO CREADO CON EXITO|";
            }

            return cMensaje;
        }






        [HttpPost]
        public string InsertarDocumentoPosc(DatosProspecto datos)
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


            
            string cNit = (string)this.Session["cNit"];
            string cEmisor = (string)this.Session["cNombreEmisor"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cAfiliacion = (string)this.Session["cAfiliacion"];
            string cEmail = (string)this.Session["cEmail"];
            string cDireccion = (string)this.Session["cDireccion"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];

            string certificador = (string)this.Session["certificador"];
            string cTokenFel = (string)this.Session["tokenfel"];
            string jSonWebApp = (string)this.Session["jSonWebApp"];
            string cCampo6 = (string)this.Session["cCampo6"];
            string id_agencia = (string)this.Session["Sucursal"];

            string cUser = Session["Usuario"].ToString();

            string cFrases = (string)this.Session["cCampo1"];

            string cUpdate = "";

            bool lgetDigi = false;
            string pageurl = "";
            string Guid = "";

            string cExp = "";

            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            string oBaseDatos = (string)this.Session["oBase"];

            ResultCrud errorcrud = new ResultCrud();


            int nDocumento = 0;
            bool lError = false;
            string cError = "Hubo un error en el ingreso, favor revisar...";
            int nContador = 1;
            double nCosto = 0.00;
            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();
            FelEstructura.Adenda adenda = new FelEstructura.Adenda();
            FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();
            FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();

            string cId_Interno = "";

            int nLinea = 0;
            string cServicio = "";
            double nPrecioSD = 0.00;
            double nMontoGravable = 0.00;
            double nMontoImpuesto = 0.00;
            double nMontoTotalImpuesto = 0.00;

            DateTime tiempo1 = DateTime.Now;
            DateTime tiempo2 = DateTime.Now;
            string difTiempo = "";

            DateTime hora = DateTime.Now;
            string solo_hora = hora.ToString("HH:mm:ss");

            string cDetalle = Funciones.Base64Decode(datos.cdetalle);

            string cDtalle = cDetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/").Replace("<span id=\"mySpan\" style=\"color: red;\">","").Replace("<span id=\"mySpan\" style=\"color: black;\">", "").Replace("<span>", "");
            XmlDocument xmlDoc = new XmlDocument();

            /// este es el xml del EQ guardarlo en una ruta x
            xmlDoc.LoadXml(cDtalle);


            //  System.IO.File.WriteAllText("C:\\API\\EqXml" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);



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
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList tipoVenta = xmlDoc.GetElementsByTagName("TIPOVENTA");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");
            XmlNodeList serie = xmlDoc.GetElementsByTagName("SERIE");
            XmlNodeList cCorreo = xmlDoc.GetElementsByTagName("CORREO");



            /// aca vamos hacer que ingrese una factura normal comun y corriente

            if (tipodocto[0].InnerText == "FACTU")
            {
                cInst = String.Format("SELECT IFNULL(max({0}), 0) AS JSON ", "no_factura");
                cInst += String.Format("FROM {0} ", "facturas");
                cInst += "where id_agencia = " + id_agencia.ToString() + " and serie='" + serie[0].InnerText + "'";

                EqAppQuery queryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                string jsonString = JsonConvert.SerializeObject(queryapi);

                // llamar al api 

                string cRespuesta = Funciones.EqAppQuery(jsonString);


                Resultado resultado = new Resultado();
                resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                if (resultado.resultado.ToString() == "true")
                {
                    double ndocu = Convert.ToDouble(resultado.Data[0].JSON);
                    nDocumento = Convert.ToInt32(ndocu) + 1;
                }




                //stringConexionMySql.EjecutarLectura(cInst, DB);
                //if (stringConexionMySql.consulta.Read())
                //{
                //    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                //}
                //// Cerramos la consulta
                //stringConexionMySql.CerrarConexion();






                System.IO.File.WriteAllText("C:\\API\\EqXml" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);


                stringConexionMySql.ExecAnotherCommand("START TRANSACTION", DB);




                double EFECTIVO = 0.00, TARJETA = 0.00, CHEQUE = 0.00, VALE = 0.00, TRANSFERENCIA = 0.00;

                switch (formaPago)
                {
                    case 1:
                        EFECTIVO = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 2:
                        TARJETA = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 3:
                        CHEQUE = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 4:
                        VALE = Convert.ToDouble(nTotal.ToString());
                        break;
                    case 5:
                        TRANSFERENCIA = Convert.ToDouble(nTotal.ToString());
                        break;
                }



                cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,cont_cred,id_caja,efectivo,tarjeta,vale,cheque,transferencia,hechopor,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}') ",
                    nDocumento.ToString(),
                    serie[0].InnerText,
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText,
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    id_agencia.ToString(),
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString(),
                   tipoVenta[0].InnerText,
                   Session["IDCajaAsignada"].ToString(),
                   EFECTIVO.ToString(),
                   TARJETA.ToString(),
                   VALE.ToString(),
                   CHEQUE.ToString(),
                   TRANSFERENCIA.ToString(),
                   cUser,
                   nDescto.ToString()); ;

                EqAppQuery insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(insertqueryapi);
                cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadoinsert = new ResultCrud();

                resultadoinsert = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadoinsert.resultado.ToString() == "false")
                {
                    cError = resultadoinsert.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    errorcrud = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                    return cError;
                }

                bool errorSP = false;

                //** CAMBIAR AL API Y NO MANEJAR STORE PROCEDURE
                //string storedProcedureFactura = "ventaPOS_insertar_facturas";
                //List<string> listaEncabezadoFactura = new List<string>();
                //listaEncabezadoFactura.Add(nDocumento.ToString());
                //listaEncabezadoFactura.Add(serie[0].InnerText);
                //listaEncabezadoFactura.Add(fechadocto[0].InnerText);
                //listaEncabezadoFactura.Add("I");
                //listaEncabezadoFactura.Add(id_cliente[0].InnerText);
                //listaEncabezadoFactura.Add(cliente[0].InnerText);
                //listaEncabezadoFactura.Add(vendedor[0].InnerText);
                //listaEncabezadoFactura.Add(direccion[0].InnerText);
                //listaEncabezadoFactura.Add(nTotal.ToString());
                //listaEncabezadoFactura.Add(obs[0].InnerText);
                //listaEncabezadoFactura.Add(id_agencia.ToString());
                //listaEncabezadoFactura.Add(nit[0].InnerText);
                //listaEncabezadoFactura.Add(dias[0].InnerText.ToString());
                //listaEncabezadoFactura.Add(nDescto.ToString());
                //listaEncabezadoFactura.Add(tipoVenta[0].InnerText);
                //**AGREGAR LA FORMA DE PAGO 
                //listaEncabezadoFactura.Add(Session["IDCajaAsignada"].ToString());
                //listaEncabezadoFactura.Add(EFECTIVO.ToString());
                //listaEncabezadoFactura.Add(TARJETA.ToString());
                //listaEncabezadoFactura.Add(CHEQUE.ToString());
                //listaEncabezadoFactura.Add(VALE.ToString());
                //listaEncabezadoFactura.Add(TRANSFERENCIA.ToString());
                //bool errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureFactura, DB, listaEncabezadoFactura, ref cError);

                //if (errorSP == true)
                //{
                //    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                //    stringConexionMySql.CerrarConexion();


                //    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                //    string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                //    List<string> listaEliminarVenta = new List<string>();
                //    listaEliminarVenta.Add(nDocumento.ToString());
                //    listaEliminarVenta.Add(serie[0].InnerText);
                //    errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                //    return cError;
                //}
                //else
                //{

                //    stringConexionMySql.CerrarConexion();

                //}



                //VAMOS A VALIDAR QUE SI LA VENTA ES AL CONTADO NO INGRESA DATOS A LA TABLA CTACC
                if (tipoVenta[0].InnerText == "2")
                {

                    cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                                   id_cliente[0].InnerText, nDocumento.ToString(), serie[0].InnerText, 1, fechadocto[0].InnerText, fechadocto[0].InnerText, dias[0].InnerText.ToString(), fechadocto[0].InnerText,
                                                    nTotal.ToString(), nTotal.ToString(), "Operado en Modulo Web", cUser, "FACTURA", 1);

                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };

                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                    ResultCrud resultadoctacc = new ResultCrud();

                    resultadoctacc = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                    if (resultadoctacc.resultado.ToString() == "false")
                    {
                        cError = resultadoctacc.data;

                        cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBaseDatos

                        };
                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                        errorcrud = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());


                        return cError;
                    }



                    //**//INSERTAR CTACC Y ASUMIR QUE ES VENTA AL CREDITO
                    //string storedProcedureCTACC = "ventaPOS_insertar_ctacc";
                    //List<string> listaCuentaCorriente = new List<string>();
                    //listaCuentaCorriente.Add(id_cliente[0].InnerText);
                    //listaCuentaCorriente.Add(nDocumento.ToString());
                    //listaCuentaCorriente.Add(serie[0].InnerText);
                    //listaCuentaCorriente.Add(id_agencia.ToString());
                    //listaCuentaCorriente.Add(fechadocto[0].InnerText);
                    //listaCuentaCorriente.Add(nTotal.ToString());
                    //listaCuentaCorriente.Add("Operado en Modulo Web");
                    //listaCuentaCorriente.Add(cUser);
                    //listaCuentaCorriente.Add("FACTURA");
                    //listaCuentaCorriente.Add("1");
                    //listaCuentaCorriente.Add(dias[0].InnerText.ToString());
                    //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureCTACC, DB, listaCuentaCorriente, ref cError);

                    //if (errorSP == true)
                    //{
                    //    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    //    stringConexionMySql.CerrarConexion();


                    //    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                    //    string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                    //    List<string> listaEliminarVenta = new List<string>();
                    //    listaEliminarVenta.Add(nDocumento.ToString());
                    //    listaEliminarVenta.Add(serie[0].InnerText);
                    //    errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);
                    //    stringConexionMySql.CerrarConexion();
                    //    return cError;

                    //}
                    //else
                    //{
                    //    stringConexionMySql.CerrarConexion();
                    //}
                }

                // vamos a insertar el detalle de la factura y kardex y denas
                // 
                nContador = 0;
                cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) VALUES ";
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText.Replace("/", "|");
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                    if (descto.ToString() == "")
                    {
                        descto = "0";
                    }

                    cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), serie[0].InnerText, codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString().Replace("'", ""), 1);

                    //if (nContador == cXmlDetalle.Count)
                    //{
                    //    cInst += "";
                    //}
                    //else
                    //{
                    //    cInst += ",";
                    //}
                    //**//INSERTAR DETFACTURAS
                    //string storedProcedureDetFact = "ventaPOS_insertar_detfacturas";
                    //List<string> listaDetalleFactura = new List<string>();
                    //listaDetalleFactura.Add(nDocumento.ToString());
                    //listaDetalleFactura.Add(serie[0].InnerText);
                    //listaDetalleFactura.Add(codigo.ToString());
                    //listaDetalleFactura.Add(cantidad.ToString());
                    //listaDetalleFactura.Add(precio.ToString().Replace(",", ""));
                    //listaDetalleFactura.Add(subtotal.ToString().Replace(",", ""));
                    //listaDetalleFactura.Add(nContador.ToString());
                    //listaDetalleFactura.Add(descto.ToString().Replace(",", ""));
                    //listaDetalleFactura.Add(producto.ToString());
                    //listaDetalleFactura.Add(id_agencia.ToString());
                    //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureDetFact, DB, listaDetalleFactura, ref cError);


                    //if (errorSP == true)
                    //{

                    //    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    //    stringConexionMySql.CerrarConexion();


                    //    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                    //    string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                    //    List<string> listaEliminarVenta = new List<string>();
                    //    listaEliminarVenta.Add(nDocumento.ToString());
                    //    listaEliminarVenta.Add(serie[0].InnerText);
                    //    errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);
                    //    stringConexionMySql.CerrarConexion();
                    //    return cError;

                    //}
                    //else
                    //{
                    //    stringConexionMySql.CerrarConexion();
                    //}

                    nContador++;
                }

                cInst = cInst.Replace(") (", "),(");
                insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(insertqueryapi);
                cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadodetfactura = new ResultCrud();

                resultadodetfactura = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadodetfactura.resultado.ToString() == "false")
                {
                    cError = resultadodetfactura.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    return cError;
                }


                // vamos a insertar el detalle del kardex
                // 

                nContador = 0;
                cInst = "INSERT INTO kardexinven ( id_codigo, id_agencia, fecha, id_movi, docto, serie, obs, hechopor, salida, costo1,precio, codigoemp,correlativo ) VALUES";
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;
                    try
                    {
                        nCosto = Convert.ToDouble(f.ObtieneDatos("inventario", "costo2", "id_codigo = " + codigo.ToString(), _BaseDatos));
                    }
                    catch
                    {
                        nCosto = 0.00;
                    }


                    cInst += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "51", nDocumento.ToString(), serie[0].InnerText, "Ingresado en modulo sistema Web", cUser, cantidad.ToString().Replace(",", ""), nCosto.ToString(), precio.ToString().Replace(",", ""), id_cliente[0].InnerText, nContador.ToString());
                    //if (nContador == cXmlDetalle.Count)
                    //{
                    //    cInst += "";
                    //}
                    //else
                    //{
                    //    cInst += ",";
                    //}


                    //**//INSERTAR kardexinven
                    //string storedProcedureKardexInven = "ventaPOS_insertar_kardexinven";
                    //List<string> listaKardexInventario = new List<string>();
                    //listaKardexInventario.Add(codigo.ToString());
                    //listaKardexInventario.Add(id_agencia.ToString());
                    //listaKardexInventario.Add(fechadocto[0].InnerText);
                    //listaKardexInventario.Add("51");
                    //listaKardexInventario.Add(nDocumento.ToString());
                    //listaKardexInventario.Add(serie[0].InnerText);
                    //listaKardexInventario.Add("Ingresado en modulo sistema Web");
                    //listaKardexInventario.Add(cUser);
                    //listaKardexInventario.Add(cantidad.ToString().Replace(",", ""));
                    //listaKardexInventario.Add(nCosto.ToString());
                    //listaKardexInventario.Add(precio.ToString().Replace(",", ""));
                    //listaKardexInventario.Add(id_cliente[0].InnerText);
                    //listaKardexInventario.Add(nContador.ToString());
                    //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureKardexInven, DB, listaKardexInventario, ref cError);


                    //if (errorSP == true)
                    //{

                    //    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    //    stringConexionMySql.CerrarConexion();


                    //    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                    //    string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                    //    List<string> listaEliminarVenta = new List<string>();
                    //    listaEliminarVenta.Add(nDocumento.ToString());
                    //    listaEliminarVenta.Add(serie[0].InnerText);
                    //    errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);
                    //    stringConexionMySql.CerrarConexion();
                    //    return cError;

                    //}
                    //else
                    //{
                    //    stringConexionMySql.CerrarConexion();
                    //}
                    nContador++;
                }
                cInst = cInst.Replace(") (", "),(");

                insertqueryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(insertqueryapi);
                cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                ResultCrud resultadokardexinven = new ResultCrud();

                resultadokardexinven = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

                if (resultadokardexinven.resultado.ToString() == "false")
                {
                    cError = resultadokardexinven.data;

                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                    insertqueryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBaseDatos

                    };
                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                    return cError;
                }

                //julito ponce

                //vamos a buscar todos los datos en resolucciones sat para construir el xml 
                cInst = "SELECT exportacion,tipoactivo,sucursal,direccion1,direccion2,codigoestablecimiento,leyendafacc,municipio,departamento FROM resolucionessat WHERE serie ='" + serie[0].InnerText + "'";

                queryapi = new EqAppQuery()
                {
                    Nit = cNit,
                    Query = cInst,
                    BaseDatos = oBaseDatos

                };

                jsonString = JsonConvert.SerializeObject(queryapi);

                cRespuesta = Funciones.EqAppQuery(jsonString);


                Resultado resolucionesat = new Resultado();
                resolucionesat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                if (resultado.resultado.ToString() == "true")
                {
                    cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                    JArray jsonParseo = JArray.Parse(cRespuesta);

                    foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                    {
                        //string base_datos = jsonOperaciones["base_datos"].ToString();
                        //string nombre_empresa = 
                        datosEmisor.NombreComercial = jsonOperaciones["sucursal"].ToString();
                        cExp = jsonOperaciones["exportacion"].ToString();
                        datosEmisor.Tipo = jsonOperaciones["tipoactivo"].ToString();
                        datosEmisor.Direccion = jsonOperaciones["direccion1"].ToString() + " " + jsonOperaciones["direccion2"].ToString(); ;
                        datosEmisor.CodigoEstablecimiento = jsonOperaciones["codigoestablecimiento"].ToString();
                        datosEmisor.Municipio = jsonOperaciones["municipio"].ToString();
                        datosEmisor.Departamento = jsonOperaciones["departamento"].ToString();
                        adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();

                    }

                }








                //stringConexionMySql.EjecutarLectura(cInst, DB);
                //if (stringConexionMySql.consulta.Read())
                //{
                //    //datosEmisor.NombreComercial = f.ObtieneDatos("resolucionessat", "sucursal", "serie = '" + serie[0].InnerText + "'", DB);
                //    //cExp = f.ObtieneDatos("resolucionessat", "exportacion", "serie ='" + serie[0].InnerText + "'", DB);
                //    //datosEmisor.Tipo = f.ObtieneDatos("resolucionessat", "tipoactivo", "serie ='" + serie[0].InnerText + "'", DB);
                //    //datosEmisor.Direccion = f.ObtieneDatos("resolucionessat", "direccion1", "serie = '" + serie[0].InnerText + "'", DB) + " " + f.ObtieneDatos("resolucionessat", "direccion2", "serie = '" + serie[0].InnerText + "'", DB);
                //    //datosEmisor.CodigoEstablecimiento = f.ObtieneDatos("resolucionessat", "codigoestablecimiento", "serie = '" + serie[0].InnerText + "'", DB);
                //    //datosEmisor.Municipio = f.ObtieneDatos("resolucionessat", "municipio", "serie = '" + serie[0].InnerText + "'", DB);
                //    //datosEmisor.Departamento = f.ObtieneDatos("resolucionessat", "departamento", "serie = '" + serie[0].InnerText + "'", DB);
                //    //adenda.leyendafacc = f.ObtieneDatos("resolucionessat", "leyendafacc", "serie = '" + serie[0].InnerText + "'", DB);

                //    datosEmisor.NombreComercial = stringConexionMySql.consulta.GetString(2).ToString().Trim();
                //    cExp = stringConexionMySql.consulta.GetString(0).ToString().Trim();
                //    datosEmisor.Tipo = stringConexionMySql.consulta.GetString(1).ToString().Trim();
                //    datosEmisor.Direccion = stringConexionMySql.consulta.GetString(3).ToString().Trim() + " " + stringConexionMySql.consulta.GetString(4).ToString().Trim(); 
                //    datosEmisor.CodigoEstablecimiento = stringConexionMySql.consulta.GetString(5).ToString().Trim();
                //    datosEmisor.Municipio = stringConexionMySql.consulta.GetString(7).ToString().Trim();
                //    datosEmisor.Departamento = stringConexionMySql.consulta.GetString(8).ToString().Trim();
                //    adenda.leyendafacc = stringConexionMySql.consulta.GetString(6).ToString().Trim();
                //}
                //// Cerramos la consulta
                //stringConexionMySql.CerrarConexion();




                if (datosEmisor.Tipo == "FCAM")
                {
                    cfc.NumeroAbono = "1";
                    cfc.FechaVencimiento = fechadocto[0].InnerText;
                    cfc.MontoAbono = nTotal.ToString();

                }


                double  nretencion = 0;
                double nretencioniva = 0;
                double ntotalretenciones = 0;

                if (datosEmisor.Tipo == "FESP")
                {

                    cFrases = "";
                    nretencion = (nTotal / 1.12) * 0.05;
                    nretencioniva = (nTotal / 1.12) * 0.12;
                    ntotalretenciones = nTotal - nretencion - nretencioniva;

                     cfe.RetencionISR = f.FormatoDecimal(nretencion.ToString(), 6, false).Replace(",", "");
                     cfe.RetencionIVA = f.FormatoDecimal(nretencioniva.ToString(), 6, false).Replace(",", "");
                     cfe.TotalMenosRetenciones = f.FormatoDecimal(ntotalretenciones.ToString(), 6, false).Replace(",", "");


                }
                else
                {

                }


                datosEmisor.FechaHoraEmision = fechadocto[0].InnerText + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
                if (cExp == "2")
                {
                    datosEmisor.CodigoMoneda = "GTQ";

                }
                else if (cExp == "1")
                {
                    datosEmisor.cExp = "SI";
                    datosEmisor.CodigoMoneda = "USD";

                }
                else
                {
                    datosEmisor.CodigoMoneda = "USD";
                }


                datosEmisor.cCorreos = cCorreo[0].InnerText;
                datosEmisor.NitEmisor = cNit;
                datosEmisor.NombreEmisor = cEmisor;

                datosEmisor.AfiliacionIva = cAfiliacion;
                datosEmisor.CorreoEmisor = cEmail;


                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Pais = "GT";
                datosEmisor.CodigoPostal = "01011";

                datosEmisor.Pais = "GT";





                cId_Interno = serie[0].InnerText + "-" + nDocumento.ToString();

                datosReceptor.IdReceptor = nit[0].InnerText.ToString().Replace("-", "");
                datosReceptor.NombreReceptor = cliente[0].InnerText;
                if (certificador.ToString() == "DIGIFACT")
                {
                    datosReceptor.CorreoReceptor = datosEmisor.cCorreos;
                }
                else
                {
                    datosReceptor.CorreoReceptor = "";
                }
                datosReceptor.Direccion = direccion[0].InnerText;
                datosReceptor.CodigoPostal = "01011";
                datosReceptor.Municipio = "GUATEMALA";
                datosReceptor.Departamento = "GUATEMALA";
                datosReceptor.Pais = "GT";


                // frases[0].TipoFrase = "1";
                // frases[0].CodigoEscenario = "1";

                FelEstructura.Items[] items = new FelEstructura.Items[cXmlDetalle.Count];
                nLinea = 0;
                cServicio = "";
                nPrecioSD = 0.00;
                nMontoGravable = 0.00;
                nMontoImpuesto = 0.00;
                nContador = 1;
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {
                    codigo = node.ChildNodes[0].InnerText;
                    //codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                    cServicio = f.ObtieneDatos("inventario", "servicio", "id_codigo=" + codigo, DB);  //node.ChildNodes[1].InnerText;  //f.ObtieneDatos("inventario", "servicio", "id_codigo=" + codigo, DB);
                    if (cServicio.Trim() == "N")
                    {

                        items[nLinea].BienOServicio = "B";
                    }
                    else
                    {
                        items[nLinea].BienOServicio = "S";
                    }
                    if (datosEmisor.cExp == "SI")
                    {
                        items[nLinea].NumeroLinea = nContador.ToString();
                        items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                        items[nLinea].UnidadMedida = "UNI";
                        items[nLinea].Descripcion = producto.Replace("/", "|");
                        items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                        items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                        items[nLinea].NombreCorto = "IVA";
                        items[nLinea].CodigoUnidadGravable = "2";

                        items[nLinea].MontoGravable = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        items[nLinea].MontoImpuesto = "0.00";
                        items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        nLinea = nLinea + 1;
                        nContador = nContador + 1;
                    }
                    else
                    {
                        items[nLinea].NumeroLinea = nContador.ToString();
                        items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                        items[nLinea].UnidadMedida = "UNI";
                        items[nLinea].Descripcion = producto.Replace("/", "|");
                        items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                        items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                        items[nLinea].NombreCorto = "IVA";
                        items[nLinea].CodigoUnidadGravable = "1";
                        nMontoGravable = Convert.ToDouble(subtotal) / 1.12;
                        nMontoImpuesto = nMontoGravable * 0.12;
                        items[nLinea].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                        items[nLinea].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        nLinea = nLinea + 1;
                        nContador = nContador + 1;
                    }
                }


                nMontoTotalImpuesto = Convert.ToDouble(nTotal.ToString()) / 1.12;
                nMontoTotalImpuesto = nMontoTotalImpuesto * 0.12;

                if (datosEmisor.cExp == "SI")
                {
                    totales.NombreCorto = "IVA";
                    totales.TotalMontoImpuesto = "0.00";
                    totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");
                }
                else
                {
                    totales.NombreCorto = "IVA";
                    totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoTotalImpuesto.ToString(), 6, false).Replace(",", "");
                    totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");

                }

                FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();
                FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();
                //COMENTADO PARA UTILIZAR DESPUES EL WEB SERVICE DE FACTURA ELECTRONICA
                var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, nTotal.ToString(), serie[0].InnerText, adenda, cfc, certificador,cnc,cfe,cex);



                Dte = Dte.Replace("{CONT}", "");
                Dte = Dte.Replace("&", "&amp;");


                //Dte de la sat ya es el xml buscar guardarlo en alguna parte.

                System.IO.File.WriteAllText("C:\\API\\EqDte" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", Dte);



                if (certificador.ToString() == "DIGIFACT")
                {
                    tiempo2 = DateTime.Now;

                    var wsEnvio = wsConnector.wsEnvioDigifact(cUrlFel, cTokenFel, Dte);

                    if (wsEnvio == "Error")
                    {
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        cError = "Ha ocurrido un error en la cerificacion";

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }

                    RespuestaFelDigifact respuesta = JsonConvert.DeserializeObject<RespuestaFelDigifact>(wsEnvio.ToString());

                    if (respuesta.Mensaje.Trim() == "La transformación XSLT falló.")
                    {
                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }

                    if (respuesta.Codigo == 3010)
                    {
                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();
                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();

                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        List<string> listaEliminarVenta = new List<string>();
                        listaEliminarVenta.Add(nDocumento.ToString());
                        listaEliminarVenta.Add(serie[0].InnerText);
                        errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                        return cError;

                    }




                    if (respuesta.Codigo == 9022)
                    {
                        //string cExtrae = Funciones.ExtraerInfo(respuesta.ResponseDATA1.ToString(), "UUID", ", a");

                        cError = respuesta.Mensaje + respuesta.ResponseDATA1.ToString();

                        cCampo6 = cCampo6.Replace("{ID}", cId_Interno);
                        //cCampo6 = cCampo6.Replace("{ID}", "DIG-6");
                        cCampo6 = cCampo6.Replace("{FECHA}", fechadocto[0].InnerText);
                        wsEnvio = wsConnector.wsGetDigifact(cCampo6, cTokenFel);


                        ResponseGet responseget = JsonConvert.DeserializeObject<ResponseGet>(wsEnvio.ToString());

                        if (Convert.ToDouble(responseget.RESPONSE[0].TOTAL) == Convert.ToDouble(nTotal))
                        {
                            if (responseget.RESPONSE[0].NIT_COMPRADOR.Trim() == nit[0].InnerText.ToString().Replace("-", "").Trim())
                            {
                                difTiempo = (tiempo2 - tiempo1).ToString();
                                cUpdate = "UPDATE facturas SET no_docto_fel ='" + responseget.RESPONSE[0].NUMERO + "',";
                                cUpdate += " serie_docto_fel ='" + responseget.RESPONSE[0].SERIE + "',";
                                cUpdate += " firmaelectronica = '" + responseget.RESPONSE[0].GUID + "',";
                                cUpdate += " fecha_certificacion ='" + responseget.RESPONSE[0].FECHA_DE_CERTIFICACION + "',";
                                cUpdate += " impresa ='N',";
                                cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                                cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                                cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                                cUpdate += " AND id_agencia = " + id_agencia.ToString();

                                lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                                if (lError == true)
                                {

                                    //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                                    //stringConexionMySql.CerrarConexion();

                                    //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                                    //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                                    //List<string> listaEliminarVenta = new List<string>();
                                    //listaEliminarVenta.Add(nDocumento.ToString());
                                    //listaEliminarVenta.Add(serie[0].InnerText);
                                    //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                                    cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                                    insertqueryapi = new EqAppQuery()
                                    {
                                        Nit = cNit,
                                        Query = cInst,
                                        BaseDatos = oBaseDatos

                                    };
                                    jsonString = JsonConvert.SerializeObject(insertqueryapi);
                                    cRespuesta = Funciones.EqAppInsertQuery(jsonString);


                                    return cError;

                                }


                                Guid = responseget.RESPONSE[0].GUID.Trim();
                                lgetDigi = true;
                            }


                        }
                        else
                        {
                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            // stringConexionMySql.CerrarConexion();


                            //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                            //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                            //List<string> listaEliminarVenta = new List<string>();
                            //listaEliminarVenta.Add(nDocumento.ToString());
                            //listaEliminarVenta.Add(serie[0].InnerText);
                            //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);


                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }



                    }

                    if (lgetDigi == false)
                    {
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + respuesta.NUMERO + "',";
                        cUpdate += " serie_docto_fel ='" + respuesta.Serie + "',";
                        cUpdate += " firmaelectronica = '" + respuesta.Autorizacion + "',";
                        cUpdate += " fecha_certificacion ='" + respuesta.Fecha_de_certificacion + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        cUpdate += " AND id_agencia = " + id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            // stringConexionMySql.CerrarConexion();

                            //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                            // string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                            // List<string> listaEliminarVenta = new List<string>();
                            // listaEliminarVenta.Add(nDocumento.ToString());
                            // listaEliminarVenta.Add(serie[0].InnerText);
                            // errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta, ref cError);

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }
                    }


                    pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";
                    pageurl = pageurl.Replace("{NITEMISOR}", cNit);

                    if (lgetDigi == false)
                    {
                        pageurl = pageurl.Replace("{UUID}", respuesta.Autorizacion);
                    }
                    else
                    {
                        pageurl = pageurl.Replace("{UUID}", Guid);
                    }

                    Response.Write("<script> window.open('" + pageurl + "','_blank'); </script>");

                }


                if (certificador.ToString() == "G4S")
                {
                    var wsEnvio = wsConnector.wsEnvio("POST_DOCUMENT_SAT_PDF", Funciones.Base64Encode(Dte), cId_Interno, cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");


                    if (wsEnvio[1].ToString().ToUpper() == "TRUE")
                    {

                        tiempo2 = DateTime.Now;
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + wsEnvio[4].ToString() + "',";
                        cUpdate += " serie_docto_fel ='" + wsEnvio[3].ToString() + "',";
                        cUpdate += "xmlFac='" + wsEnvio[2].ToString() + "',";
                        cUpdate += " firmaelectronica = '" + wsEnvio[6].ToString() + "',";
                        cUpdate += " fecha_certificacion ='" + wsEnvio[7].ToString() + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        cUpdate += " AND id_agencia = " + id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            // stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            stringConexionMySql.CerrarConexion();

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }

                        //stringConexionMySql.CerrarConexion();

                    }
                    else if (wsEnvio[2].ToString().Contains("PK_MASTERINDEX"))
                    {
                        wsEnvio = wsConnector.wsObtener(cId_Interno, "", "XML", cUserFe, cUrlFel, cToken, "LOOKUP_ISSUED_INTERNAL_ID", "GT", cNit, false, "");
                        wsEnvio = wsConnector.wsEnvio(wsEnvio[6], "", "XML", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, false, "");
                        tiempo2 = DateTime.Now;
                        difTiempo = (tiempo2 - tiempo1).ToString();
                        cUpdate = "UPDATE facturas SET no_docto_fel ='" + wsEnvio[4].ToString() + "',";
                        cUpdate += " serie_docto_fel ='" + wsEnvio[3].ToString() + "',";
                        cUpdate += "xmlFac='" + wsEnvio[2].ToString() + "',";
                        cUpdate += " firmaelectronica = '" + wsEnvio[6].ToString() + "',";
                        cUpdate += " fecha_certificacion ='" + wsEnvio[7].ToString() + "',";
                        cUpdate += " impresa ='N',";
                        cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                        cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                        cUpdate += " AND serie ='" + serie[0].InnerText + "'";
                        cUpdate += " AND id_agencia = " + id_agencia.ToString();

                        lError = stringConexionMySql.ExecCommand(cUpdate, DB, ref cError);
                        if (lError == true)
                        {

                            //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                            stringConexionMySql.CerrarConexion();

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBaseDatos

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                            return cError;

                        }

                        //stringConexionMySql.CerrarConexion();

                    }
                    else
                    {
                        //stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        cError = wsEnvio[2].ToString();


                        //SI DA ERROR EN HACER LA FACTURACION ELIMINA LA INFORMACION QUE REGISTRO ANTERIORMENTE.
                        //string storedProcedureEliminarVenta = "ELIMINAR_VENTA_POS_FELWEB";
                        //List<string> listaEliminarVenta = new List<string>();
                        //listaEliminarVenta.Add(nDocumento.ToString());
                        //listaEliminarVenta.Add(serie[0].InnerText);
                        //errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedureEliminarVenta, DB, listaEliminarVenta,ref cError);

                        cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + nDocumento.ToString() + ",'" + serie[0].InnerText + "')";
                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBaseDatos

                        };
                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cRespuesta = Funciones.EqAppInsertQuery(jsonString);

                        return cError;


                    }

                    string cNombreDocumento = wsEnvio[6] + ".pdf";

                    cNombreDocumento = "c:\\PDF\\" + wsEnvio[6] + ".pdf";

                    //cRutaPdf = cRutaPdf + cNombreDocumento;
                    string cRutaPdf = cNombreDocumento;
                    System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                    oFileStream.Write(Base64String_ByteArray(wsEnvio[9]), 0, Base64String_ByteArray(wsEnvio[9]).Length);
                    oFileStream.Close();


                    //var wsGet = wsConnector.wsGet(wsEnvio[6].ToString(), "ticket.xslt", "XML PDF", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, true, @".\Temp\");
                    pageurl = "/Rgp/" + wsEnvio[6].ToString() + ".pdf";
                    Response.Write("<script> window.open('" + pageurl + "','_blank'); </script>");
                }

                //  stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                // stringConexionMySql.CerrarConexion();

                //stringConexionMySql.CerrarConexion();

                stringConexionMySql.KillAllMySQL(DB);
                // puse pipe por que ? ni ide de por que siempre poner el pipe
                cMensaje = "|DOCUMENTO CREADO CON EXITO";
            }

            return cMensaje;
        }



        [HttpPost]
        public string PrevisualizarDocumento(DatosProspecto datos)
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

            string cNit = (string)this.Session["cNit"];
            string cEmisor = (string)this.Session["cNombreEmisor"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cAfiliacion = (string)this.Session["cAfiliacion"];
            string cEmail = (string)this.Session["cEmail"];
            string cDireccion = (string)this.Session["cDireccion"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string oBaseDatos = (string)this.Session["oBase"];
            string certificador = (string)this.Session["certificador"];
            string cTokenFel = (string)this.Session["tokenfel"];
            string jSonWebApp = (string)this.Session["jSonWebApp"];
            string cCampo6 = (string)this.Session["cCampo6"];
            string id_agencia = (string)this.Session["Sucursal"];

            string cFrases = (string)this.Session["cCampo1"];

            string cUpdate = "";

            bool lgetDigi = false;
            string pageurl = "";
            string Guid = "";

            string cExp = "";

            int nDocumento = 0;
            bool lError = false;
            string cError = "Hubo un error en el ingreso, favor revisar...";
            int nContador = 1;
            double nCosto = 0.00;

            

            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();
            FelEstructura.Adenda adenda = new FelEstructura.Adenda();
            FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();
            FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();




            string cId_Interno = "";

            int nLinea = 0;
            string cServicio = "";
            double nPrecioSD = 0.00;
            double nMontoGravable = 0.00;
            double nMontoImpuesto = 0.00;
            double nMontoTotalImpuesto = 0.00;

            DateTime tiempo1 = DateTime.Now;
            DateTime tiempo2 = DateTime.Now;
            string difTiempo = "";

            DateTime hora = DateTime.Now;
            string solo_hora = hora.ToString("HH:mm:ss");


          //  datos.cdetalle = Funciones.Base64Decode(datos.cdetalle);

            string cDtalle = datos.cdetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/");
            XmlDocument xmlDoc = new XmlDocument();

            /// este es el xml del EQ guardarlo en una ruta x
            xmlDoc.LoadXml(cDtalle);


            //  System.IO.File.WriteAllText("C:\\API\\EqXml" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);



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
                nTotal = nTotal + Convert.ToDouble(node.ChildNodes[7].InnerText);

            }

            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList tipoVenta = xmlDoc.GetElementsByTagName("TIPOVENTA");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");
            XmlNodeList serie = xmlDoc.GetElementsByTagName("SERIE");
            XmlNodeList escenario = xmlDoc.GetElementsByTagName("ESCENARIO");


            if (Convert.ToInt32(escenario[0].InnerText) != 0)
            {
                cFrases = cFrases.Replace("</dte:Frases>", "<dte:Frase TipoFrase=\"4\" CodigoEscenario=\"" + escenario[0].InnerText + "\"/></dte:Frases>");
            }


            cInst = "SELECT exportacion,tipoactivo,sucursal,direccion1,direccion2,codigoestablecimiento,leyendafacc,municipio,departamento FROM resolucionessat WHERE serie ='" + serie[0].InnerText + "'";

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oBaseDatos

            };

            var jsonString = JsonConvert.SerializeObject(queryapi);

            var cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resolucionesat = new Resultado();
            resolucionesat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resolucionesat.resultado.ToString() == "true")
            {
                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                JArray jsonParseo = JArray.Parse(cRespuesta);

                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {
                    
                    datosEmisor.NombreComercial = jsonOperaciones["sucursal"].ToString();
                    cExp = jsonOperaciones["exportacion"].ToString();
                    datosEmisor.Tipo = jsonOperaciones["tipoactivo"].ToString();
                    datosEmisor.Direccion = jsonOperaciones["direccion1"].ToString() + " " + jsonOperaciones["direccion2"].ToString(); ;
                    datosEmisor.CodigoEstablecimiento = jsonOperaciones["codigoestablecimiento"].ToString();
                    datosEmisor.Municipio = jsonOperaciones["municipio"].ToString();
                    datosEmisor.Departamento = jsonOperaciones["departamento"].ToString();
                    adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();

                }

            }


            /// aca vamos hacer que ingrese una factura normal comun y corriente

            if (tipodocto[0].InnerText == "FACTU")
            {
                //cInst = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
                //cInst += String.Format("FROM {0} ", "facturas");
                //cInst += "where id_agencia = " + id_agencia.ToString() + " and serie='" + serie[0].InnerText + "'";


                //stringConexionMySql.EjecutarLectura(cInst, DB);
                //if (stringConexionMySql.consulta.Read())
                //{
                //    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                //}
                //// Cerramos la consulta
                //stringConexionMySql.CerrarConexion();

                nDocumento = 123456;


                System.IO.File.WriteAllText("C:\\XML\\EqXmlPV" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt", cDtalle);




                //cExp = "2"; // f.ObtieneDatos("resolucionessat", "exportacion", "serie ='" + serie[0].InnerText + "'", DB);



                //datosEmisor.Tipo = "FCAM";// f.ObtieneDatos("resolucionessat", "tipoactivo", "serie ='" + serie[0].InnerText + "'", DB);

                if (datosEmisor.Tipo == "FCAM")
                {
                    cfc.NumeroAbono = "1";
                    cfc.FechaVencimiento = fechadocto[0].InnerText;
                    cfc.MontoAbono = nTotal.ToString();

                }

                double nretencion = 0;
                double nretencioniva = 0;
                double ntotalretenciones = 0;

                FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();

                if (datosEmisor.Tipo == "FESP")
                {

                    nretencion = (nTotal / 1.12) * 0.05;
                    nretencioniva = (nTotal / 1.12) * 0.12;
                    ntotalretenciones = nTotal - nretencion - nretencioniva;

                    cfe.RetencionISR = f.FormatoDecimal(nretencion.ToString(), 6, false).Replace(",", "");
                    cfe.RetencionIVA = f.FormatoDecimal(nretencioniva.ToString(), 6, false).Replace(",", "");
                    cfe.TotalMenosRetenciones = f.FormatoDecimal(ntotalretenciones.ToString(), 6, false).Replace(",", "");


                }

                cExp = "2";
                datosEmisor.FechaHoraEmision = fechadocto[0].InnerText + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
                if (cExp == "2")
                {
                    datosEmisor.CodigoMoneda = "GTQ";

                }
                else if (cExp == "1")
                {
                    datosEmisor.cExp = "SI";
                    datosEmisor.CodigoMoneda = "USD";

                }
                else
                {
                    datosEmisor.CodigoMoneda = "USD";
                }


                datosEmisor.cCorreos = "";// f.ObtieneDatos("clientes", "email", "id_codigo = " + id_cliente[0].InnerText, DB);
                datosEmisor.NitEmisor = cNit;
                datosEmisor.NombreEmisor = cEmisor;
                datosEmisor.NombreComercial = "VISTA PREVIA ";//f.ObtieneDatos("resolucionessat", "sucursal", "serie = '" + serie[0].InnerText + "'", DB);
                datosEmisor.AfiliacionIva = cAfiliacion;
                datosEmisor.CorreoEmisor = cEmail;
                datosEmisor.Direccion = "CIUDAD GUATEMALA";//;f.ObtieneDatos("resolucionessat", "direccion1", "serie = '" + serie[0].InnerText + "'", DB) + " " + f.ObtieneDatos("resolucionessat", "direccion2", "serie = '" + serie[0].InnerText + "'", DB);
                datosEmisor.CodigoEstablecimiento = "1";// f.ObtieneDatos("resolucionessat", "codigoestablecimiento", "serie = '" + serie[0].InnerText + "'", DB);
                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Pais = "GT";
                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Municipio = "GUATEMALA"; //f.ObtieneDatos("resolucionessat", "municipio", "serie = '" + serie[0].InnerText + "'", DB);
                datosEmisor.Departamento = "GUATEMALA";// f.ObtieneDatos("resolucionessat", "departamento", "serie = '" + serie[0].InnerText + "'", DB);
                datosEmisor.Pais = "GT";

                adenda.leyendafacc = "";// f.ObtieneDatos("resolucionessat", "leyendafacc", "serie = '" + serie[0].InnerText + "'", DB);

                cId_Interno = serie[0].InnerText + "-" + nDocumento.ToString();

                datosReceptor.IdReceptor = nit[0].InnerText.ToString().Replace("-", "");
                datosReceptor.NombreReceptor = cliente[0].InnerText;
                if (certificador.ToString() == "DIGIFACT")
                {
                    datosReceptor.CorreoReceptor = datosEmisor.cCorreos;
                }
                else
                {
                    datosReceptor.CorreoReceptor = "";
                }
                datosReceptor.Direccion = direccion[0].InnerText;
                datosReceptor.CodigoPostal = "01011";
                datosReceptor.Municipio = "GUATEMALA";
                datosReceptor.Departamento = "GUATEMALA";
                datosReceptor.Pais = "GT";


                // frases[0].TipoFrase = "1";
                // frases[0].CodigoEscenario = "1";

                FelEstructura.Items[] items = new FelEstructura.Items[cXmlDetalle.Count];
                nLinea = 0;
                cServicio = "";
                nPrecioSD = 0.00;
                nMontoGravable = 0.00;
                nMontoImpuesto = 0.00;
                nContador = 1;
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    if (node.ChildNodes[6].InnerText == "12")
                    {
                        codigo = node.ChildNodes[0].InnerText;
                        codigoe = node.ChildNodes[1].InnerText;
                        producto = Funciones.Base64Decode(node.ChildNodes[3].InnerText).Replace("<pre>", "");
                        cantidad = node.ChildNodes[2].InnerText;
                        precio = node.ChildNodes[4].InnerText;
                        descto = node.ChildNodes[5].InnerText;
                        subtotal = node.ChildNodes[7].InnerText;


                        items[nLinea].BienOServicio = "B";


                        items[nLinea].NumeroLinea = nContador.ToString();
                        items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                        items[nLinea].UnidadMedida = "UNI";
                        items[nLinea].Descripcion = producto.Replace("/", "|");
                        items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                        items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                        items[nLinea].NombreCorto = "IVA";
                        items[nLinea].CodigoUnidadGravable = "1";
                        nMontoGravable = Convert.ToDouble(subtotal) / 1.12;
                        nMontoImpuesto = nMontoGravable * 0.12;

                        nMontoTotalImpuesto += nMontoGravable;
                        items[nLinea].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                        items[nLinea].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        nLinea = nLinea + 1;
                        nContador = nContador + 1;
                    }
                    else
                    {
                        codigo = node.ChildNodes[0].InnerText;
                        codigoe = node.ChildNodes[1].InnerText;
                        producto = Funciones.Base64Decode(node.ChildNodes[3].InnerText).Replace("<pre>", "");
                        cantidad = node.ChildNodes[2].InnerText;
                        precio = node.ChildNodes[4].InnerText;
                        descto = node.ChildNodes[5].InnerText;
                        subtotal = node.ChildNodes[7].InnerText;


                        items[nLinea].BienOServicio = "B";


                        items[nLinea].NumeroLinea = nContador.ToString();
                        items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                        items[nLinea].UnidadMedida = "UNI";
                        items[nLinea].Descripcion = producto.Replace("/", "|");
                        items[nLinea].PrecioUnitario = f.FormatoDecimal(precio, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(precio) * Convert.ToDouble(cantidad);
                        items[nLinea].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Descuento = f.FormatoDecimal(descto, 6, false).Replace(",", "");
                        items[nLinea].NombreCorto = "IVA";
                        items[nLinea].CodigoUnidadGravable = "2";
                        nMontoGravable = Convert.ToDouble(subtotal);
                        nMontoImpuesto = 0;
                        items[nLinea].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                        items[nLinea].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                        items[nLinea].Total = f.FormatoDecimal(subtotal, 6, false).Replace(",", "");
                        nLinea = nLinea + 1;
                        nContador = nContador + 1;
                    }
                }


                //nMontoTotalImpuesto = Convert.ToDouble(nTotal.ToString()) / 1.12;
                nMontoTotalImpuesto = nMontoTotalImpuesto * 0.12;
                totales.NombreCorto = "IVA";
                totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoTotalImpuesto.ToString(), 6, false).Replace(",", "");
                totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");


                FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();


                //COMENTADO PARA UTILIZAR DESPUES EL WEB SERVICE DE FACTURA ELECTRONICA
               

                var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, nTotal.ToString(), serie[0].InnerText, adenda, cfc, certificador,cnc,cfe,cex);



                Dte = Dte.Replace("{CONT}", "");
                Dte = Dte.Replace("&", "&amp;");


                //Dte de la sat ya es el xml buscar guardarlo en alguna parte.

                System.IO.File.WriteAllText("C:\\XML\\EqDteVPV" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".xml", Dte);

                string cNombreDocumento = "";
                string cRutaXslt = "";

                if (_BaseDatos.Trim() == "kenichi")
                {
                    cNombreDocumento = "C:\\XML\\EqDteVPV" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".xml";
                    cRutaXslt = "C:\\Rpo\\dtegt_kenichifel.xslt";
                }
                else
                {
                    cNombreDocumento = "C:\\XML\\EqDteVPV" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + ".xml";
                    cRutaXslt = "C:\\Rpo\\formato.xslt";

                }


                XPathDocument myXPathDoc = new XPathDocument(cNombreDocumento);
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(cRutaXslt);
                string cRutaPDF = "c:\\PDF\\EqDteV" + nit[0].InnerText + "idintero" + nDocumento.ToString() + "" + hora.ToString("yyyy-MM-ddTHH-mm-ss") + "." + "html";
                
                XmlTextWriter myWriter = new XmlTextWriter(cRutaPDF, null);
                myXslTrans.Transform(myXPathDoc, null, myWriter);

                
                //var wsGet = wsConnector.wsGet(wsEnvio[6].ToString(), "ticket.xslt", "XML PDF", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, true, @".\Temp\");
                pageurl = "/Rgp/" + cRutaPDF.Replace("c:\\PDF\\", "");
                //Response.Write("<script> window.open('" + pageurl + "','_blank'); </script>");


                cMensaje = pageurl;


                
            }
            return cMensaje;
        }




        #region Base64
        public static string ByteArray_Base64String(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        public static byte[] String_ByteArray(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        public static string String_Base64String(string s)
        {
            return ByteArray_Base64String(String_ByteArray(s));
        }//String_Base64String

        public static string Base64String_String(string b64)
        {
            try
            {
                return ByteArray_String(Base64String_ByteArray(b64));
            }
            catch
            {
                return b64;
            }

        }//Base64String_String

        public static byte[] Base64String_ByteArray(string s)
        {
            return Convert.FromBase64String(s);
        }//Base64String_ByteArray



        public static string ByteArray_String(byte[] b)
        {
            return new string(System.Text.Encoding.UTF8.GetChars(b));
        }//ByteArray_String
        #endregion


        public bool permisoDescuento()
        {
            string rol = (string)this.Session["Rol"];
            string Base_Datos = (string)(Session["StringConexion"]);

            //ConexionMySQL conexionMySql = new ConexionMySQL();

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string query = "SELECT estado FROM dlempresa.usuario_menus WHERE usuario = '" + rol + "' AND id_menu = 80;";
            
            string str = stringConexionMySql.Consulta(query, Base_Datos);

                if (str == "True")
                {
                    return true;
                }
            
            return false;
        }

    }
}