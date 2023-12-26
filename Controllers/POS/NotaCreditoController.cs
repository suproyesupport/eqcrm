using DevExpress.Xpo.Helpers;
using EqCrm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.POS
{
    public class NotaCreditoController : Controller
    {
        // GET: NotaCredito
        public ActionResult NotaCredito()
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


            string permisofactsinexist = (string)this.Session["Permiso"];


            if (permisofactsinexist == "S")
            {
                ViewBag.permiso = "true";
            }
            else if (permisofactsinexist == "N")
            {
                ViewBag.permiso = "false";
            }

            return View("~/Views/NotaCredito/NotaCredito.cshtml");
        }


        [HttpPost]
        public string BuscaDetalleFactura(string no_factura, string serie)
        {
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            DataTable dt = new DataTable();

            string query = "SELECT id_codigo, cantidad, obs, precio, descto, subtotal " +
                "FROM detfacturas " +
                "WHERE no_factura = " + no_factura + " AND serie = '" + serie + "'";

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

 

        public string CargarFacturas(FiltroGenerico oFiltro)
        {
            StringConexionMySQL mysql = new StringConexionMySQL();
            string Base_Datos = (string)(Session["StringConexion"]);
            string cTabla = "";

            string firma = oFiltro.firma;
            string fecha1 = "";
            string fecha2 = "";

            if (firma == null ) { 
                firma = ""; 
            }

            if (oFiltro.fecha1 != null) {

                string[] f1 = oFiltro.fecha1.Split('/');
                fecha1 = f1[2] + "-" + f1[0] + "-" + f1[1];
            }
           
            if (oFiltro.fecha2 != null ) {
                string[] f2 = oFiltro.fecha2.Split('/');
                fecha2 = f2[2] + "-" + f2[0] + "-" + f2[1];
            } 

            // Cada dia mas cabron y cierra conexion para no usar appi que no sea pro c#
            string cInst = "SELECT no_factura AS FACTURA, serie AS SERIE, fecha AS FECHA, status AS STATUS, id_cliente AS IDCLIENTE, " +
                "cliente AS CLIENTE, nit AS NIT, total AS TOTAL, direccion AS DIRECCION " +
                "FROM facturas " +
                "WHERE status = 'I' ";
                
            if (fecha1 != "" && fecha2 != "") {

                cInst += "AND fecha >= '" + fecha1 + "' AND fecha <= '" + fecha2 + "' ";    
            }

            if (firma != "")
            {
                cInst += "AND firmaelectronica = '" + firma + "' ";
            }
            
            cInst += "ORDER BY fecha DESC ";

            LlenarListaVentas.listaFacturas = mysql.ListFacturasNC(cInst, Base_Datos, LlenarListaVentas.listaFacturas);
            string cRespuesta = JsonConvert.SerializeObject(LlenarListaVentas.listaFacturas);
            cTabla = Funciones.LlenarTableHTmlFactNC(cRespuesta);

            return cTabla;
            //ViewBag.Tabla = cTabla;

            //return View();

            //return (ActionResult)this.View("~/Views/POS/Operaciones/CrearVentas.cshtml");
        }







    }
}