using Dapper;
using EqCrm.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.CargaFacturacionMasiva
{
    public class FacturacionMasivaController : Controller
    {
        // GET: FacturacionMasiva
        public ActionResult FacturacionMasiva()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public string CargarFacturas(string informacion)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //Convertimos el JSON en un DataTable
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(informacion, (typeof(DataTable)));

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL modificar = new StringConexionMySQL();
            dapperConnect eqdap = new dapperConnect();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            string cInst = "";

            string deleteQuery = "DELETE FROM clientesrecurrencia";

            //using (IDbConnection dbConnection = new MySqlConnection(DB))
            //{
            //    dbConnection.Open();

                
            //    string deleteQuery = "DELETE FROM clientesrecurrencia"; 
                
            //    int rowsAffected = dbConnection.Execute(deleteQuery);
                
            //    dbConnection.Close();
            //}

            lError = eqdap.EqExecute(deleteQuery, DB, ref cError);
            if (lError == true)
            {
                return cError;
            }

            
            try
            {
                //Recorremos el DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string no = row["no"].ToString();
                    string nit = row["nit"].ToString();
                    string cliente = row["cliente"].ToString();
                    string dtes = row["dtes"].ToString();
                    string total = row["total"].ToString();
                    string mesano = row["mesano"].ToString();
                    
                    if (no != "No.")
                    {

                        

                        cInst += " INSERT INTO clientesrecurrencia (cliente, nit,mesano,total,dtesemitidos,fecha) ";
                        cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}') ", cliente, nit, mesano, total, dtes, DateTime.Now.ToString("yyyy-MM-dd"));
                        cInst += ";";

                        
                    }

                   
                }

                cInst += "UPDATE clientesrecurrencia a, clientes b ";
                cInst += " set a.id_codigo = b.id_codigo,";
                cInst += " a.minimo = b.limcred,";
                cInst += " a.anualmensual = b.id_coordinador,";
                cInst += " a.preciodte = b.pcomi";
                cInst += " WHERE a.nit = b.nit ;";
               


                cInst += "UPDATE clientesrecurrencia set totalafacturar = preciodte*dtesemitidos, facturarminimo=1, descripafacturar =concat(\"SERVICIO DE DTE MENSUAL DEL MES \" , RIGHT(mesano,2), \" DEL \", LEFT(mesano,4)  ) where dtesemitidos >= minimo ;";



                cInst += "UPDATE clientesrecurrencia set totalafacturar = preciodte*minimo, facturarminimo=2, descripafacturar =concat(\"SERVICIO DE DTE MINIMO MENSUAL DEL MES \" , RIGHT(mesano,2), \" DEL \", LEFT(mesano,4)  ) where dtesemitidos < minimo ;";

                lError = eqdap.EqExecute(cInst, DB, ref cError);
                if (lError == true)
                {
                    return cError;
                }


                cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE,nit as NIT,fecha AS FECHA, minimo AS MINIMO, dtesemitidos AS EMITIDOS, preciodte AS PRECIO, totalafacturar AS SUBTOTAL, facturarminimo AS FACMIN, descripafacturar AS OBS,(SELECT direccion FROM clientes WHERE clientes.id_codigo = clientesrecurrencia.id_codigo) AS DIRECCION,(SELECT email FROM clientes WHERE clientes.id_codigo = clientesrecurrencia.id_codigo) AS EMAIL from clientesrecurrencia WHERE id_codigo > 0 ";



                var resultados = eqdap.ExecuteList<dynamic>(DB, cInst);

                var json = JsonConvert.SerializeObject(resultados);
                



                return json.ToString();


            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            
            return str;
        }

        [HttpPost]
        public string Facturar(string informacion)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            //Convertimos el JSON en un DataTable
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(informacion, (typeof(DataTable)));

            dapperConnect eqdap = new dapperConnect();
            
            StringConexionMySQL modificar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            
            string DB = (string)this.Session["StringConexion"];
            
            
            StringConexionMySQL mysql = new StringConexionMySQL();
            string cInst = "";
            
            int numeroMayor = 0;
            string id_vendedor = "";
            Funciones f = new Funciones();
            string cDireccion = "";
            string cEmail = "";
            

            string cNit = (string)this.Session["cNit"];
            string cEmisor = (string)this.Session["cNombreEmisor"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cAfiliacion = (string)this.Session["cAfiliacion"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string cDireccione = (string)this.Session["cDireccion"];
            string cFrases = (string)this.Session["cCampo1"];


            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();
            FelEstructura.Adenda adenda = new FelEstructura.Adenda();
            FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();
            FelEstructura.Items[] items = new FelEstructura.Items[1];
            FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();

            string cId_Interno = "";

            DateTime tiempo1 = DateTime.Now;
            DateTime tiempo2 = DateTime.Now;
            string difTiempo = "";


            numeroMayor = Funciones.NumeroMayor("facturas", "no_factura", "serie = 'FEL'", DB);
            adenda.leyendafacc = f.ObtieneDatos("resolucionessat", "leyendafacc", "serie = 'FEL'", DB);

            try
            {
                //Recorremos el DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string CODIGO = row["CODIGO"].ToString();
                    string CLIENTE = row["CLIENTE"].ToString();
                    string NIT = row["NIT"].ToString();
                    string FECHA = row["FECHA"].ToString();
                    string MINIMO = row["MINIMO"].ToString();
                    string EMITIDOS = row["EMITIDOS"].ToString();
                    string PRECIO = row["PRECIO"].ToString();
                    string SUBTOTAL = row["SUBTOTAL"].ToString();
                    string FACMIN = row["FACMIN"].ToString();
                    string OBS = row["OBS"].ToString();
                    string DIRECCION = row["DIRECCION"].ToString();
                    string EMAIL = row["EMAIL"].ToString();





                    if (CODIGO.Trim() != "")
                    {
                        /// ACA TIENEN QUE IR LOS QUERYS DE INSERCION 
                        /// 

                        id_vendedor = "0";//f.ObtieneDatos("clientes", "id_vendedor", "id_codigo=" + row.Cells[0].Text.ToString(), oDb);
                        cDireccion = DIRECCION;
                        cEmail = EMAIL;




                        cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,tdescto,email) ";
                        cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}') ",
                            numeroMayor.ToString(),
                            "FEL",
                            DateTime.Now.ToString("yyyy-MM-dd"),
                            "I",
                            CODIGO,
                            CLIENTE,
                            id_vendedor.ToString(),
                            cDireccion.ToString(),
                            SUBTOTAL,
                            "INGRESO DESDE EL MODULO AUTOMATICO DE FACTURACION",
                            1,
                            NIT,
                            Convert.ToDateTime(FECHA).ToString("yyyy-MM-dd"),
                            15, 0, cEmail);

                        lError = eqdap.EqExecute(cInst, DB, ref cError);
                        if (lError == true)
                        {
                            return cError;
                        }


                        

                        if (FACMIN.Trim() == "2")
                        {
                            cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                            cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                            numeroMayor.ToString(),
                            "FEL",
                            13,
                            MINIMO,
                            PRECIO,
                            SUBTOTAL,
                            1,
                            0,
                            OBS, 1);
                        }
                        else
                        {
                            cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                            cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                            numeroMayor.ToString(),
                            "FEL",
                            12,
                            EMITIDOS,
                            PRECIO,
                            SUBTOTAL,
                            1,
                            0,
                            OBS, 1);
                        }

                        lError = eqdap.EqExecute(cInst, DB, ref cError);
                        if (lError == true)
                        {
                            return cError;
                        }

                        


                        cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                        cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                                       CODIGO,
                                                       numeroMayor.ToString(),
                                                       "FEL",
                                                        1,
                                                        DateTime.Now.ToString("yyyy-MM-dd"),
                                                        DateTime.Now.ToString("yyyy-MM-dd"),
                                                        15, DateTime.Now.ToString("yyyy-MM-dd"),
                                                        SUBTOTAL,
                                                        SUBTOTAL,
                                                        "INGRESO DESDE EL MODULO AUTOMATICO DE FACTURACION",
                                                        "WEB", "FACTURA", 1);

                        lError = eqdap.EqExecute(cInst, DB, ref cError);
                        if (lError == true)
                        {
                            return cError;
                        }


                        //Aca ya inserto todo lo que tiene que ver con la base de datos, ahora haremos el envio de la factura electronica
                        datosEmisor.Tipo = "FCAM";//f.ObtieneDatos("resolucionessat", "tipoactivo", "serie ='FEL'", oDb);

                        if (datosEmisor.Tipo == "FCAM")
                        {
                            cfc.NumeroAbono = "1";
                            cfc.FechaVencimiento = DateTime.Now.ToString("yyyy-MM-dd");
                            cfc.MontoAbono = SUBTOTAL;

                        }
                        datosEmisor.FechaHoraEmision = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
                        datosEmisor.CodigoMoneda = "GTQ";


                        


                        // de esta forma es para pruebas
                        datosEmisor.cCorreos = cEmail;
                        datosEmisor.cAsunto = "FACTURA DE DTES FEL EMITIDA ";

                        datosEmisor.NitEmisor = cNit;
                        datosEmisor.NombreEmisor = cEmisor.Replace("&", "&amp;");
                        datosEmisor.NombreComercial = cComercial;
                        datosEmisor.AfiliacionIva = cAfiliacion;
                        datosEmisor.CorreoEmisor = "";
                        datosEmisor.Direccion = cDireccione;
                        datosEmisor.CodigoEstablecimiento = "1";
                        datosEmisor.CodigoPostal = "01011";
                        datosEmisor.Pais = "GT";
                        datosEmisor.CodigoPostal = "01011";
                        datosEmisor.Municipio = "GUATEMALA";  //f.ObtieneDatos("resolucionessat", "municipio", "serie = 'FEL'", oDb);
                        datosEmisor.Departamento = "GUATEMALA"; //f.ObtieneDatos("resolucionessat", "departamento", "serie = 'FEL'", oDb);
                        datosEmisor.Pais = "GT";



                        cId_Interno = "FEL" + "-" + numeroMayor.ToString();

                        datosReceptor.IdReceptor = NIT;
                        datosReceptor.NombreReceptor = CLIENTE.Replace("&", "&amp;"); ;
                        datosReceptor.CorreoReceptor = "";
                        datosReceptor.Direccion = cDireccion;
                        datosReceptor.CodigoPostal = "01011";
                        datosReceptor.Municipio = "GUATEMALA";
                        datosReceptor.Departamento = "GUATEMALA";
                        datosReceptor.Pais = "GT";


                        frases[0].TipoFrase = "1";
                        frases[0].CodigoEscenario = "2";


                        double nPrecioSD = 0.00;
                        double nCantidad = 0.00;
                        double nMontoGravable = 0.00;
                        double descto = 0.00;
                        double subtotal = Convert.ToDouble(SUBTOTAL);
                        double nMontoImpuesto = 0.00;
                        double nMontoTotalImpuesto = 0.00;

                        items[0].BienOServicio = "S";

                        items[0].NumeroLinea = "1";
                        if (FACMIN.Trim() == "2")
                        {
                            items[0].Cantidad = MINIMO;
                            nCantidad = Convert.ToDouble(MINIMO);
                        }
                        else
                        {
                            items[0].Cantidad = EMITIDOS;
                            nCantidad = Convert.ToDouble(EMITIDOS);
                        }

                        items[0].UnidadMedida = "SER";
                        items[0].Descripcion = OBS;
                        items[0].PrecioUnitario = f.FormatoDecimal(PRECIO, 6, false).Replace(",", "");
                        nPrecioSD = Convert.ToDouble(PRECIO) * Convert.ToDouble(nCantidad);
                        items[0].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                        items[0].Descuento = f.FormatoDecimal(descto.ToString(), 6, false).Replace(",", "");
                        items[0].NombreCorto = "IVA";
                        items[0].CodigoUnidadGravable = "1";
                        nMontoGravable = Convert.ToDouble(subtotal) / 1.12;
                        nMontoImpuesto = nMontoGravable * 0.12;
                        items[0].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                        items[0].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                        items[0].Total = f.FormatoDecimal(subtotal.ToString(), 6, false).Replace(",", "");

                        nMontoTotalImpuesto = Convert.ToDouble(subtotal.ToString()) / 1.12;
                        nMontoTotalImpuesto = nMontoTotalImpuesto * 0.12;
                        totales.NombreCorto = "IVA";
                        totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoTotalImpuesto.ToString(), 6, false).Replace(",", "");
                        totales.GranTotal = f.FormatoDecimal(subtotal.ToString(), 6, false).Replace(",", "");



                        //COMENTADO PARA UTILIZAR DESPUES EL WEB SERVICE DE FACTURA ELECTRONICA
                        FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();
                        FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();
                        var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, subtotal.ToString(), "FEL", adenda, cfc, "G4S", cnc, cfe, cex);

                        Dte = Dte.Replace("{CONT}", "");


                        var wsEnvio = wsConnector.wsEnvio("POST_DOCUMENT_SAT", Funciones.Base64Encode(Dte), cId_Interno, cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

                        string cUpdate = "";
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
                            cUpdate += " WHERE no_factura = " + numeroMayor.ToString();
                            cUpdate += " AND serie ='FEL'";
                            cUpdate += " AND id_agencia = 1";

                            lError = eqdap.EqExecute(cUpdate, DB, ref cError);
                            if (lError == true)
                            {
                                return cError;
                            }

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
                            cUpdate += " WHERE no_factura = " + numeroMayor.ToString();
                            cUpdate += " AND serie ='FEL'";
                            cUpdate += " AND id_agencia = 1";

                            lError = eqdap.EqExecute(cUpdate, DB, ref cError);
                            if (lError == true)
                            {
                                return cError;
                            }

                            }
                        else
                        {
                            tiempo2 = DateTime.Now;
                            difTiempo = (tiempo2 - tiempo1).ToString();
                            cUpdate = "UPDATE facturas SET no_docto_fel ='',";
                            cUpdate += " serie_docto_fel ='',";
                            cUpdate += "xmlFac='" + wsEnvio[2].ToString() + "',";
                            cUpdate += " error = '" + wsEnvio[2].ToString() + "',";
                            cUpdate += " fecha_certificacion ='',";
                            cUpdate += " impresa ='N',";
                            cUpdate += " tiempo_certificacion ='" + difTiempo.ToString() + "'";
                            cUpdate += " WHERE no_factura = " + numeroMayor.ToString();
                            cUpdate += " AND serie ='FEL'";
                            cUpdate += " AND id_agencia = 1";

                            lError = eqdap.EqExecute(cUpdate, DB, ref cError);
                            if (lError == true)
                            {
                                return cError;
                            }
                                                        
                        }

                        numeroMayor = numeroMayor + 1;

                    }



                }

                cInst = "SELECT no_factura as NOFACTURA,serie AS SERIE,fecha AS FECHA,id_cliente CODIGO, cliente AS CLIENTE, nit AS NIT, direccion AS DIRECCION, email AS MAIL, total AS TOTAL, firmaelectronica AS AUTORIZACION, no_docto_fel AS DOCTO, serie_docto_fel AS SERIEFEL, error AS ERROR FROM `facturas` WHERE fecha IN(curdate())";


                var resultados =eqdap.ExecuteList<dynamic>(DB, cInst);

                var json = JsonConvert.SerializeObject(resultados);

                return json.ToString();


            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";


            return str;
        }


        
    }
}