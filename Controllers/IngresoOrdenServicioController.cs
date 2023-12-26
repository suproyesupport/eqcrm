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
using MySql.Data.MySqlClient;
using EqCrm.Models;
using System.Web.Script.Serialization;

namespace EqCrm.Controllers
{
    public class IngresoOrdenServicioController : Controller
    {
        // GET: IngresoOrdenServicio
        public ActionResult IngresoOrdenServicio()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            string id_agencia = (string)this.Session["Sucursal"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            ViewBag.Serie = CargarSerie();

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
        public string BuscarCorrelativo()
        {
            string DB = (string)this.Session["StringConexion"];
            string str = "";
            string cInst = "SELECT JSON_OBJECT('ID', IFNULL(MAX(a.no_factura),0)) AS ID FROM ordenserv a WHERE serie = '" + CargarSerie() + "'";

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
                            str = reader[0].ToString();
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
                str = stringConexionMySql.LlenarDDLLineaVehiculo(cInst, dt, DB);
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string CargarSerie()
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];
            string id_agencia = (string)this.Session["Sucursal"];

            try
            {
                string cInst = "SELECT serie FROM resolucionessat WHERE  tipo = 11 AND id_agencia = " + id_agencia.ToString();
                str = stringConexionMySql.Consulta(cInst, DB);

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            stringConexionMySql.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string GuardarOrdenServicio(string informacion)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //DESERIALIZAMOS EL JSON EN OBJETO
            OrdenServicio oOS = JsonConvert.DeserializeObject<OrdenServicio>(informacion);

            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            string sucursal = (string)this.Session["Sucursal"];


            try
            {
                string query = "INSERT INTO ordenserv (no_factura, serie, fecha, fechae, id_cliente, cliente, nit, id_vendedor, direccion, total, cont_cred, dias_cred, obs, hechopor, " +
                "id_agencia, tipov, marcav, id_placa, color, odometro1, modelo, fechaps, kmmi, linea, nunidad, telefono, email, codaseguradora, nopolizaseg, atendio, reclamo, corredora, seguradotercero, ajustador, deducible, tipoplaca, " +
                "radio, encendedor, documentos, alfombras, llanta, tricket, llave, herramienta, platos) ";
                query += "VALUES (";
                query += oOS.id_orden + ", ";                   //no_factura              
                query += "'" + CargarSerie() + "', ";           //serie  
                query += "'" + oOS.fechaingreso + "', ";        //fecha
                query += "'" + oOS.fechaentrega + "', ";        //fechae
                query += oOS.idcliente + ", ";                  //id_cliente
                query += "'" + oOS.cliente + "', ";             //cliente
                query += "'" + oOS.nit + "', ";                 //nit
                query += oOS.asesores + ", ";                   //id_vendedor
                query += "'" + "CIUDAD" + "', ";                //direccion
                //query += "'" + oOS.direccion + "', ";         //direccion
                query += "0.00, ";                              //total
                //query += oOS.total + ", ";                    //total
                query += "1, ";                                 //cont_cred       
                //query += oOS.tipoVenta + ", ";                //cont_cred       
                query += "0, ";                                 //dias_cred
                //query += oOS.diascredito + ", ";              //dias_cred
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
                query += "0, ";                                 //codaseguradora
                //query += oOS.aseguradoras + ", ";             //codaseguradora
                query += "'" + "" + "', ";                      //nopolizaseg
                //query += "'" + oOS.poliza + "', ";            //nopolizaseg
                query += "'" + "" + "', ";                      //atendio
                //query += "'" + oOS.asesoremergencia + "', ";  //atendio
                query += "'" + "" + "', ";                       //reclamo
                //query += "'" + oOS.reclamo + "', ";           //reclamo
                query += "'" + "" + "', ";                      //corredora
                //query += "'" + oOS.corredora + "', ";         //corredora
                query += "1, ";                                 //seguradotercero
                //query += oOS.opciones + ", ";                 //seguradotercero
                query += "'" + "" + "', ";                      //ajustador            
                //query += "'" + oOS.ajustador + "', ";         //ajustador            
                query += "0.00, ";                              //deducible
                //query += oOS.deducible + ", ";                //deducible
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
                str = "Orden registrada exitosamente";

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

            return str;
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
            if (str == "")
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }

            return str;
        }
    }
}