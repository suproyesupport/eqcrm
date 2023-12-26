using DevExpress.PivotGrid.ServerMode;
using EqCrm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;

namespace EqCrm.Controllers
{
    public class GetDataFelController : Controller
    {
        // GET: GetDataFel
        [HttpPost]
        public string GetDataFel(string cUUid)
        {
            string cNit = (string)this.Session["cNit"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string certificador = (string)this.Session["certificador"];
            string cRespuesta = "";
            string pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";


            if (certificador.ToString() == "DIGIFACT")
            {
                pageurl = pageurl.Replace("{NITEMISOR}", cNit);
                pageurl = pageurl.Replace("{UUID}", cUUid.Trim());
                cRespuesta = pageurl;
            }
            else
            {

                var wsEnvio = wsConnector.wsGet(cUUid, "ticket.xslt", "XML PDF", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, true, @".\Temp\");
                cRespuesta = "/Rgp/" + wsEnvio[6].ToString() + ".PDF";
            }


            return cRespuesta.ToString();
        }

        [HttpPost]
        public string GetAnulaFel(string cUUid)
        {
            string[] cParse = cUUid.Split('|');
            string uuid = cParse[0];
            string cMotivo = cParse[1];
            string cNit = (string)this.Session["cNit"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string certificador = (string)this.Session["certificador"];
            string cXmlAnula = (string)this.Session["cCampo4"];
            string cRespuesta = "";
            string pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";
            string cDte = "";
            dapperConnect eqdapper = new dapperConnect();
            string cInst = "";
            string oDb = (string)(Session["StringConexion"]); 

            string cUser = (string)this.Session["Usuario"];
            Funciones f= new Funciones();

            if (cMotivo == "")
            {
                cMotivo = "ANULACION DE FACTURA";
            }

            if (certificador.ToString() == "DIGIFACT")
            {
                
                cNit = cNit.PadLeft(12, '0');
                var wsEnvio = wsConnector.wsEnvio(uuid,"", cMotivo, cUserFe, "https://felgtaws.digifact.com.gt/gt.com.fel.wsfrontalter/felwsfront.asmx?wsdl", cToken, "CANCEL_FEL", "GT", cNit, true, @".\Temp\");

                pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";
                pageurl = pageurl.Replace("{NITEMISOR}", cNit);
                pageurl = pageurl.Replace("{UUID}", uuid.Trim());

                cInst = "SELECT no_factura,serie,id_cliente,total,status FROM facturas WHERE firmaelectronica='"+uuid+"'";

                              


                cRespuesta = pageurl;
            }
            else
            {

                var wsEnvio = wsConnector.wsEnvio("GET_INFODTE", uuid, "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, true, @".\Temp\");

                JArray jsonParseo = JArray.Parse(wsEnvio[5]);
                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {

                    cDte = CrearXml.CreaDteAnulaFact(uuid, jsonOperaciones["Fecha_de_emision"].ToString(), jsonOperaciones["NIT_receptor"].ToString(), cNit, cXmlAnula, cMotivo);



                }
                var wsAnula = wsConnector.wsEnvio("VOID_DOCUMENT", Funciones.Base64Encode(cDte), "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

                var wsEnvio2 = wsConnector.wsGet(uuid, "ticket.xslt", "XML PDF", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, true, @".\Temp\");
                cRespuesta = "/Rgp/" + wsEnvio2[6].ToString() + ".PDF";
            }

            cInst = "SELECT no_factura,serie,id_cliente,total,status FROM facturas WHERE firmaelectronica='" + uuid + "'";

            var listfactura = eqdapper.ExecuteList<dynamic>(oDb, cInst);

            foreach (var factura in listfactura)
            {
                // Acceder a las propiedades dinámicamente
                var noFactura = factura.no_factura;
                var serie = factura.serie;
                var idCliente = factura.id_cliente;
                var total = factura.total;
                var status = factura.status;


                var lProcesa = Funciones.ProcesarAnulacion(noFactura.ToString(), serie.ToString(), idCliente.ToString(), "facturas", oDb.ToString(), cUser);
            }


            return cRespuesta.ToString();
        }

        [HttpPost]
        public string GetNcDescFel(string cUUid)
        {
            string[] cParse = cUUid.Split('|');
            string uuid = cParse[0];
            string cMotivo = cParse[1];
            string cNoDocto = cParse[2];
            string cSerie = cParse[3];
            string cFecha = cParse[4];
            string cAbono = cParse[5];
            int nDocumento = 0;
            string cNit = (string)this.Session["cNit"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string certificador = (string)this.Session["certificador"];
            string cXmlAnula = (string)this.Session["cCampo4"];
            string cRespuesta = "";
            string pageurl = "https://felgtaws.digifact.com.gt/guest/api/FEL?DATA={NITEMISOR}|{UUID}|GUESTUSERQR";
            string cDte = "";


            double nPrecioSD = 0.00;

            string cEmisor = (string)this.Session["cNombreEmisor"];
            string cComercial = (string)this.Session["cNombreComercial"];
            string cAfiliacion = (string)this.Session["cAfiliacion"];
            string cEmail = (string)this.Session["cEmail"];
            string cDireccion = (string)this.Session["cDireccion"];
            double ndocu = 0;
            string oDb = (string)this.Session["oBase"];

            string cFrases = (string)this.Session["cCampo1"];

            Funciones f = new Funciones();
            StringConexionMySQL lectura = new StringConexionMySQL();

            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();
            FelEstructura.Adenda adenda = new FelEstructura.Adenda();
            FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();
            FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();
            FelEstructura.Items[] items = new FelEstructura.Items[1];

            string cInst = "SELECT exportacion,tipoactivo,sucursal,direccion1,direccion2,codigoestablecimiento,leyendafacc,municipio,departamento FROM resolucionessat WHERE serie ='NCRE'";

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oDb

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resolucionesat = new Resultado();
            resolucionesat = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resolucionesat.resultado.ToString() == "true")
            {
                cRespuesta = Funciones.ExtraerInfoEqJson(cRespuesta.ToString());
                JArray jsonParseo = JArray.Parse(cRespuesta);

                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {
                    //string base_datos = jsonOperaciones["base_datos"].ToString();
                    //string nombre_empresa = 
                    datosEmisor.NombreComercial = jsonOperaciones["sucursal"].ToString();

                    datosEmisor.Tipo = jsonOperaciones["tipoactivo"].ToString();
                    datosEmisor.Direccion = jsonOperaciones["direccion1"].ToString() + " " + jsonOperaciones["direccion2"].ToString(); ;
                    datosEmisor.CodigoEstablecimiento = jsonOperaciones["codigoestablecimiento"].ToString();
                    datosEmisor.Municipio = jsonOperaciones["municipio"].ToString();
                    datosEmisor.Departamento = jsonOperaciones["departamento"].ToString();
                    adenda.leyendafacc = jsonOperaciones["leyendafacc"].ToString();

                }

            }


            //datosEmisor.FechaHoraEmision = lectura.CurdateYear("SELECT CURDATE()", oDb).ToString() + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
            datosEmisor.FechaHoraEmision = cFecha + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
            datosEmisor.CodigoMoneda = "GTQ";

            datosEmisor.cCorreos = "";
            datosEmisor.NitEmisor = cNit;
            datosEmisor.NombreEmisor = cEmisor;

            datosEmisor.AfiliacionIva = cAfiliacion;
            datosEmisor.CorreoEmisor = "";

            datosEmisor.CodigoPostal = "01011";
            datosEmisor.Pais = "GT";
            datosEmisor.CodigoPostal = "01011";
            datosEmisor.Pais = "GT";




            if (cMotivo == "")
            {
                cMotivo = "ANULACION DE FACTURA";
            }

            if (certificador.ToString() == "DIGIFACT")
            {
                pageurl = pageurl.Replace("{NITEMISOR}", cNit);
                pageurl = pageurl.Replace("{UUID}", cUUid.Trim());
                cRespuesta = pageurl;
            }
            else
            {

                var wsEnvio = wsConnector.wsEnvio("GET_INFODTE", uuid, "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, true, @".\Temp\");
                

                JArray jsonParseo = JArray.Parse(wsEnvio[5]);
                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {

                    datosReceptor.IdReceptor = jsonOperaciones["NIT_receptor"].ToString();
                    datosReceptor.NombreReceptor = jsonOperaciones["Nombre_comprador"].ToString();
                    if (certificador.ToString() == "DIGIFACT")
                    {
                        datosReceptor.CorreoReceptor = datosEmisor.cCorreos;
                    }
                    else
                    {
                        datosReceptor.CorreoReceptor = "";
                    }
                    datosReceptor.Direccion = "CIUDAD";
                    datosReceptor.CodigoPostal = "01011";
                    datosReceptor.Municipio = "GUATEMALA";
                    datosReceptor.Departamento = "GUATEMALA";
                    datosReceptor.Pais = "GT";

                    cnc.NumeroAutorizacionDocumentoOrigen = uuid;
                    cnc.NumeroDocumentoOrigen = cNoDocto;
                    cnc.FechaEmisionDocumentoOrigen = Convert.ToDateTime(jsonOperaciones["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd");///jsonOperaciones["Fecha_de_emision"].ToString();
                    cnc.SerieDocumentoOrigen = cSerie;
                    cnc.MotivoAjuste = cMotivo;
                    //cDte = CrearXml.CreaDteAnulaFact(uuid, jsonOperaciones["Fecha_de_emision"].ToString(), jsonOperaciones["NIT_receptor"].ToString(), cNit, cXmlAnula, cMotivo);



                }


                items[0].BienOServicio = "B";
                items[0].NumeroLinea = "1";
                items[0].Cantidad = "1";
                items[0].UnidadMedida = "UNI";
                items[0].Descripcion = cMotivo;
                items[0].PrecioUnitario = cAbono;
                nPrecioSD = Convert.ToDouble(cAbono);
                items[0].Precio = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");
                items[0].Descuento = "0";
                items[0].NombreCorto = "IVA";
                items[0].CodigoUnidadGravable = "1";
                double subtotal = nPrecioSD;
                double nMontoGravable = Convert.ToDouble(subtotal) / 1.12;
                double nMontoImpuesto = nMontoGravable * 0.12;
                items[0].MontoGravable = f.FormatoDecimal(nMontoGravable.ToString(), 6, false).Replace(",", "");
                items[0].MontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                items[0].Total = nPrecioSD.ToString();


                totales.NombreCorto = "IVA";
                totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoImpuesto.ToString(), 6, false).Replace(",", "");
                totales.GranTotal = f.FormatoDecimal(nPrecioSD.ToString(), 6, false).Replace(",", "");

            }


            cInst = String.Format("SELECT IFNULL(max({0}), 0) AS JSON ", "no_factura");
            cInst += String.Format("FROM {0} ", "notadecredito");
            cInst += "where  serie='NCRE'";

            EqAppQuery queryapi2 = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oDb

            };

            jsonString = JsonConvert.SerializeObject(queryapi2);

            // llamar al api 

             string cRespuesta2 = Funciones.EqAppQuery(jsonString);


            Resultado resultado2 = new Resultado();
            resultado2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta2.ToString());

            if (resultado2.resultado.ToString() == "true")
            {
                 ndocu = Convert.ToDouble(resultado2.Data[0].JSON);
                nDocumento = Convert.ToInt32(ndocu) + 1;
            }



            string cId_Interno = "NCRE-"+ nDocumento.ToString();

            FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();
            FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();
                      //COMENTADO PARA UTILIZAR DESPUES EL WEB SERVICE DE FACTURA ELECTRONICA
            var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales,false, cId_Interno,nPrecioSD.ToString(), datosEmisor.Tipo.ToString(), adenda, cfc, certificador, cnc,cfe,cex);


            var wsEnvioG4s = wsConnector.wsEnvio("POST_DOCUMENT_SAT_PDF", Funciones.Base64Encode(Dte), cId_Interno, cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");



            //                                       '{0}', '{1}','{2}','{3}',       '{4}',     '{5}',    '{6}',   '{7}',   '{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}'
            cInst = "INSERT INTO notadecredito (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,cont_cred,hechopor,firmaelectronica,serie_docto_fel,no_docto_fel,tdescto) ";
            cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}','{17}','{18}','{19}') ",
                nDocumento.ToString(),
                "NCRE",
                cFecha,
                "I",
                1,
                datosReceptor.NombreReceptor,
                1,
                "CIUDAD",
                nPrecioSD.ToString(),
                cMotivo,
                1,
                datosReceptor.IdReceptor,
                cFecha,
               0,
               1,
               "",
               wsEnvioG4s[6].ToString(),
               wsEnvioG4s[3].ToString(),
               wsEnvioG4s[4].ToString(),
               0); 

            EqAppQuery insertqueryapi = new EqAppQuery()
            {
                Nit = cNit,
                Query = cInst,
                BaseDatos = oDb

            };

            jsonString = JsonConvert.SerializeObject(insertqueryapi);
            cRespuesta = Funciones.EqAppInsertQuery(jsonString);


            ResultCrud resultadoinsert = new ResultCrud();

            resultadoinsert = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cRespuesta.ToString());

            if (resultadoinsert.resultado.ToString() == "false")
            {
                string cError = resultadoinsert.data;

                return cError;
            }


            string cNombreDocumento = wsEnvioG4s[6] + ".pdf";

            cNombreDocumento = "c:\\PDF\\" + wsEnvioG4s[6] + ".pdf";

            //cRutaPdf = cRutaPdf + cNombreDocumento;
            string cRutaPdf = cNombreDocumento;
            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
            oFileStream.Write(Base64String_ByteArray(wsEnvioG4s[9]), 0, Base64String_ByteArray(wsEnvioG4s[9]).Length);
            oFileStream.Close();


            //var wsGet = wsConnector.wsGet(wsEnvio[6].ToString(), "ticket.xslt", "XML PDF", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, true, @".\Temp\");
            pageurl = "/Rgp/" + wsEnvioG4s[6].ToString() + ".pdf";
            Response.Write("<script> window.open('" + pageurl + "','_blank');</script>");


            //string cMensaje = "|DOCUMENTO CREADO CON EXITO";

            return ""  ;//cMensaje;
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


        [HttpPost]
        public string GetDataNit(string nit)
        {

            string str = "";
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            
            string DB = (string)(Session["StringConexion"]); 
            

            NitsInfile.ingfaceWsServicesClient ingface = new NitsInfile.ingfaceWsServicesClient();

            string sentenciaSQL1 = "SELECT cliente FROM clientes WHERE nit='"+nit.ToString()+"'";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"ERROR ESTE CLIENTE YA APARECE REGISTRADO\"}";

                str = str.Replace("CNOMBRE",stringConexionMySql.consulta.GetString(0).ToString());

                return str;
            }



            sentenciaSQL1 = "SELECT cliente FROM clientes WHERE nit='" + nit.Replace("-","").ToString() + "'";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"ERROR ESTE CLIENTE YA APARECE REGISTRADO\"}";

                str = str.Replace("CNOMBRE", stringConexionMySql.consulta.GetString(0).ToString());
                
                return str;
            }



            stringConexionMySql.CerrarConexion();

                        
            var cRespuesta =  ingface.nitContribuyentes("CONSUMO_NIT", "58B45D8740C791420C53A49FFC924A1B58B45D8740C791420C53A49FFC924A1B", nit.Replace("-",""));


            str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"CDIRECCION\"}";

            str = str.Replace("CNOMBRE", cRespuesta.nombre);
            str = str.Replace("CDIRECCION", cRespuesta.direccion_completa);

            return str;
        }

        [HttpPost]
        public string GetDataNitProv(string nit)
        {

            string str = "";
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string DB = (string)(Session["StringConexion"]);

            NitsInfile.ingfaceWsServicesClient ingface = new NitsInfile.ingfaceWsServicesClient();

            string sentenciaSQL1 = "SELECT cliente FROM proveedores WHERE nit='" + nit.ToString() + "'";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"ERROR ESTE PROVEEDOR YA APARECE REGISTRADO\"}";

                str = str.Replace("CNOMBRE", stringConexionMySql.consulta.GetString(0).ToString());

                return str;
            }



            sentenciaSQL1 = "SELECT cliente FROM proveedores WHERE nit='" + nit.Replace("-", "").ToString() + "'";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"ERROR ESTE PROVEEDOR YA APARECE REGISTRADO\"}";

                str = str.Replace("CNOMBRE", stringConexionMySql.consulta.GetString(0).ToString());

                return str;
            }



            stringConexionMySql.CerrarConexion();



            var cRespuesta = ingface.nitContribuyentes("CONSUMO_NIT", "58B45D8740C791420C53A49FFC924A1B58B45D8740C791420C53A49FFC924A1B", nit.Replace("-", ""));


            str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"CDIRECCION\"}";

            str = str.Replace("CNOMBRE", cRespuesta.nombre);
            str = str.Replace("CDIRECCION", cRespuesta.direccion_completa);

            return str;
        }



        [HttpPost]
        public string GetDataNitEmpresa(string nit)
        {

            string str = "";
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string DB = (string)(Session["StringConexion"]);


            NitsInfile.ingfaceWsServicesClient ingface = new NitsInfile.ingfaceWsServicesClient();


            var cRespuesta = ingface.nitContribuyentes("CONSUMO_NIT", "58B45D8740C791420C53A49FFC924A1B58B45D8740C791420C53A49FFC924A1B", nit.Replace("-", ""));


            str = "{\"NOMBRE\": \"CNOMBRE\", \"DIRECCION\": \"CDIRECCION\"}";

            str = str.Replace("CNOMBRE", cRespuesta.nombre);
            str = str.Replace("CDIRECCION", cRespuesta.direccion_completa);

            return str;
        }



        [HttpPost]
        public string BusquedaDtes(string cFechas)
        {
            string[] cParse = cFechas.Split('|');
            string cFecha1 = Convert.ToDateTime(cParse[0]).ToString("yyyy-MM-dd");
            string cfecha2 = Convert.ToDateTime(cParse[1]).ToString("yyyy-MM-dd");
            string cNit = (string)this.Session["cNit"];
            string cUrlFel = (string)this.Session["cUrlFel"];
            string cToken = (string)this.Session["cToken"];
            string cUserFe = (string)this.Session["cUserFe"];
            string certificador = (string)this.Session["certificador"];
            string cUUid = "";
            string XMLToSend = "";
            string cId_CodigoClie = "";
            string cInst = "";
            string oBase = (string)(Session["oBase"]);
            EqAppQuery queryapi = new EqAppQuery();
            string cResult = "";
            string[] cDocto;
            string jsonString="";

            EqAppQuery insertqueryapi = new EqAppQuery();
            string cQuery="";

            string cError = "";

            XMLToSend = " {";
            XMLToSend += "    \"consulta_documentos\": {";
            XMLToSend += "        \"fecha_documento_del\": '" + cFecha1 + "',";
            XMLToSend += "                    \"fecha_documento_al\": '" + cfecha2 + "',";
            XMLToSend += "                    \"tipo_docto\": \"\",";
            XMLToSend += "                    \"nit_comprador\": \"\",";
            XMLToSend += "                    \"uuid_documento\": \"\",";
            XMLToSend += "                    \"referencia_interna\": \"\",";
            XMLToSend += "                    \"estado_documento\": \"\"";
            XMLToSend += "     }";
            XMLToSend += " }";

                       
            var cRespuesta = wsConnector.wsEnvio("CONSULTA_DOCUMENTOS", Funciones.Base64Encode(XMLToSend.ToString()), "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");


            JArray jsonParseo = JArray.Parse(cRespuesta[5]);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                cUUid = jsonOperaciones["uuid_documento"].ToString();
                cRespuesta = wsConnector.wsEnvio(cUUid, "", "XML", cUserFe, cUrlFel, cToken, "GET_DOCUMENT", "GT", cNit, false, "");
                var cGetInfo = wsConnector.wsEnvio("GET_INFODTE", cUUid, "", cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, true, @".\Temp\");

                JArray encabezado = JArray.Parse(cGetInfo[5]);
                foreach (JObject jsonEncabezado in encabezado.Children<JObject>())
                {
                    cInst = "SELECT id_codigo as JSON FROM clientes WHERE nit='" + jsonEncabezado["NIT_receptor"].ToString() + "'";

                    queryapi = new EqAppQuery()
                    {
                        Nit = cNit,
                        Query = cInst,
                        BaseDatos = oBase

                    };

                    jsonString = JsonConvert.SerializeObject(queryapi);

                    cResult = Funciones.EqAppQuery(jsonString);


                    Resultado resultado = new Resultado();
                    resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cResult.ToString());

                    if (resultado.resultado.ToString() == "true")
                    {
                        cId_CodigoClie = resultado.Data[0].JSON.ToString();
                        cDocto = jsonEncabezado["Numero_interno"].ToString().Split('-');

                        cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,cont_cred,id_caja,efectivo,tarjeta,vale,serie_docto_fel,no_docto_fel,hechopor,firmaelectronica) ";
                        cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}') ",
                            cDocto[1],
                            cDocto[0],
                            Convert.ToDateTime(jsonEncabezado["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd"),
                            "I",
                            cId_CodigoClie,
                            jsonEncabezado["Nombre_comprador"].ToString(),
                            1,
                            "CIUDAD",
                            jsonEncabezado["Monto_total"].ToString(),
                            "OPERADO EN MODULO DE RECUPERACION DE FIRMAS DTE",
                            1,
                            jsonEncabezado["NIT_receptor"].ToString(),
                            Convert.ToDateTime(jsonEncabezado["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd"),
                            1,
                           2,
                           1,
                           0.00,
                           0.00,
                           0.00,
                           cRespuesta[3],
                           cRespuesta[4],
                           "root",
                           cUUid);


                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBase

                        };

                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cQuery = Funciones.EqAppInsertQuery(jsonString);


                        ResultCrud resultadofac = new ResultCrud();

                        resultadofac = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cQuery.ToString());

                        if (resultadofac.resultado.ToString() == "false")
                        {
                            cError = resultadofac.data;

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + cDocto[1] + ",'" + cDocto[0] + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBase

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cQuery = Funciones.EqAppInsertQuery(jsonString);

                            return cError;
                        }





                        cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                        cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}')",
                                                       cId_CodigoClie, cDocto[1], cDocto[0], 1, Convert.ToDateTime(jsonEncabezado["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd"), Convert.ToDateTime(jsonEncabezado["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd"), 1, Convert.ToDateTime(jsonEncabezado["Fecha_de_emision"].ToString()).ToString("yyyy-MM-dd"),
                                                        jsonEncabezado["Monto_total"].ToString(), jsonEncabezado["Monto_total"].ToString(), "Operado en Modulo Recuperacion Dtes", "root", "FACTURA", 1);



                        /// vamos a insertar ya con el Api la facturación 
                        /// 



                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst,
                            BaseDatos = oBase

                        };

                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cQuery= Funciones.EqAppInsertQuery(jsonString);


                        ResultCrud resultadoctacc = new ResultCrud();

                        resultadoctacc = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cQuery.ToString());

                        if (resultadoctacc.resultado.ToString() == "false")
                        {
                            cError = resultadoctacc.data;

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + cDocto[1] + ",'" + cDocto[0] + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBase

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cQuery = Funciones.EqAppInsertQuery(jsonString);

                            return cError;
                        }



                        XmlDocument xmlDoc = new XmlDocument();

                        /// este es el xml del EQ guardarlo en una ruta x
                        xmlDoc.LoadXml(cRespuesta[5]);

                        XmlNodeList cXmlDetalle = xmlDoc.GetElementsByTagName("dte:Items");

                        int nContador = 0;
                        string cInst2 = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) VALUES ";
                        foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                        {
                            int codigo = 12;

                            string producto = node.ChildNodes[nContador].ChildNodes[2].InnerText;
                            string cantidad = node.ChildNodes[nContador].ChildNodes[0].InnerText;
                            string precio = node.ChildNodes[nContador].ChildNodes[4].InnerText;

                            string subtotal = node.ChildNodes[nContador].ChildNodes[7].InnerText;

                            cInst2 += string.Format("('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", cDocto[1], cDocto[0], codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), 0, producto.ToString().Replace("'", ""), 1);

                            nContador++;
                        }

                        cInst2 = cInst2.Replace(") (", "),(");



                        insertqueryapi = new EqAppQuery()
                        {
                            Nit = cNit,
                            Query = cInst2,
                            BaseDatos = oBase

                        };

                        jsonString = JsonConvert.SerializeObject(insertqueryapi);
                        cQuery = Funciones.EqAppInsertQuery(jsonString);


                        ResultCrud detfacturas = new ResultCrud();

                        detfacturas = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultCrud>(cQuery.ToString());

                        if (detfacturas.resultado.ToString() == "false")
                        {
                            cError = resultadoctacc.data;

                            cInst = "call ELIMINAR_VENTA_POS_FELWEB(" + cDocto[1] + ",'" + cDocto[0] + "')";
                            insertqueryapi = new EqAppQuery()
                            {
                                Nit = cNit,
                                Query = cInst,
                                BaseDatos = oBase

                            };
                            jsonString = JsonConvert.SerializeObject(insertqueryapi);
                            cQuery = Funciones.EqAppInsertQuery(jsonString);

                            return cError;
                        }

                    }

                }
            }

            return "RESULTADO POSITIVO";

        }



    }

   
}
