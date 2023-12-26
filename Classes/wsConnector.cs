using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using DevExpress.XtraReports.Parameters;
using MySql.Data.MySqlClient.Memcached;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using ParameterType = RestSharp.ParameterType;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;


namespace EqCrm
{
    public class wsConnector
    {
        
        private static wsG4s.FactWSFront wsServicio = new wsG4s.FactWSFront();
        private static wsG4s.TransactionTag tt = new wsG4s.TransactionTag();

        // Clase que hara la transaccion en el servidor ws especificado
        //public static string[] wsEnvio(XmlDocument cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)

        public static string wsEnvioDigifact(string cUrl, string cToken, string cDte)
        {
            try
            {
            
              var client = new RestClient(cUrl);
              IRestResponse response;
              var requestpost = new RestRequest(cUrl, Method.POST);

              requestpost.AddHeader("Authorization", cToken);
              requestpost.AddHeader("Content-Type", "application/json");
              requestpost.AddParameter("application/json", cDte, ParameterType.RequestBody);
              response= client.Execute(requestpost);
                    

                

                return response.Content.ToString();

            }
            catch(WebException ex)
            {
                return "Error";
            }

            //OTRA FORMA DE CONSUMIR REST API 
            //var request = (HttpWebRequest)WebRequest.Create(cUrl);            
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.Accept = "application/json";
            //request.Headers.Add("Authorization", cToken);
            //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            //{
            //    streamWriter.Write(cDte);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}
            //try
            //{
            //    using (WebResponse response = request.GetResponse())
            //    {
            //        using (Stream strReader = response.GetResponseStream())
            //        {

            //            using (StreamReader objReader = new StreamReader(strReader))
            //            {
            //                string responseBody = objReader.ReadToEnd();

            //                return responseBody;
            //            }
            //        }
            //    }
            //}
            //catch (WebException ex)
            //{
            //    return "Error";
            //}






            // una forma de consumir REST API
            //var request = WebRequest.Create(cUrl);
            //request.ContentType = "application/json; charset=utf-8";
            //request.Method = "POST";
            //request.Headers.Add("Authorization",cToken);
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] data = encoding.GetBytes(cDte);
            //request.ContentLength = data.Length;
            //Stream newStream = request.GetRequestStream(); //open connection
            //newStream.Write(data, 0, data.Length); // Send the data.            
            //newStream.Close();


            //string text;
            //try
            //{


            //    var response = (HttpWebResponse)request.GetResponse();

            //    using (var sr = new StreamReader(response.GetResponseStream()))
            //    {
            //        text = sr.ReadToEnd();
            //    }
            //}
            //catch(Exception ex)
            //{
            //    return "Error";
            //}



            //return text.ToString();



        }


        public static string wsGetDigifact(string cUrl, string cToken)
        {
            try
            {


                var client = new RestClient(cUrl);
                IRestResponse response;

                
                    var requestget = new RestRequest(cUrl, Method.GET);

                    requestget.AddHeader("Authorization", cToken);
                    requestget.AddHeader("Content-Type", "application/json");
                    requestget.AddParameter("application/json", "", ParameterType.RequestBody);
                    response = client.Execute(requestget);

              

                return response.Content.ToString();

            }
            catch (WebException ex)
            {
                return "Error";
            }




        }

        public static string wsEnvioG4s(String cSoap,string cUrl)
        {            

            var client = new RestClient(cUrl);            
            var request = new RestRequest(cUrl,Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            var body = cSoap;
            request.AddParameter("text/xml", body, RestSharp.ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.ToString();

        }


        public static string obtieneTasaBG()
        {

            string cUrl = "http://www.banguat.gob.gt/variables/ws/TipoCambio.asmx?WSDL";
            string cSoap = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                  cSoap += "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ns0=\"http://www.banguat.gob.gt/variables/ws/\">";
                  cSoap += "<SOAP-ENV:Header/>";
                  cSoap += "<SOAP-ENV:Body>";
                  cSoap += "<ns0:TipoCambioDia/>";
                  cSoap += "</SOAP-ENV:Body>";
                  cSoap += "</SOAP-ENV:Envelope>";
            
            var client = new RestClient(cUrl);
            var request = new RestRequest(cUrl, Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            var body = cSoap;
            request.AddParameter("text/xml", body, RestSharp.ParameterType.RequestBody);
            var response = client.Execute(request);

            var nTasa = Funciones.ExtraerInfo(response.Content.ToString(), "<referencia>", "</referencia>");

            return nTasa.ToString();

        }

        public static string TokenDigifact(string cUrl, string cUserName, string cPasword)
        {
            var client = new RestClient("https://felgttestaws.digifact.com.gt/gt.com.fel.api.v3/api/login/get_token");

            var request = new RestRequest(cUrl, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //            var body = @"{
            //" + "\n" +
            //            @"    ""UserName"":""GT.000026544814.TESTUSER"",
            //" + "\n" +
            //            @"    ""Password"":""vm7Tw!w%""
            //" + "\n" +
            //            @"}";
            var body = @"{
            " + "\n" +
                        @"    ""UserName"":""{USER}"",
            " + "\n" +
                        @"    ""Password"":""{PASS}""
            " + "\n" +
                        @"}";

            body = body.Replace("{USER}", cUserName).Replace("{PASS}",cPasword);
            
            request.AddParameter("application/json", body, RestSharp.ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.ToString();
        }


            


        public static string[] wsEnvio(string cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)
        {
            //Variables que haran el retorno
            string[] cResultados = new string[13];
            XmlDocument cFirma = new XmlDocument();
            wsServicio.Url = new Uri(cUrl).ToString();

            //string cRutaXslt = "";
            //wsServicio.Proxy = System.Net.WebProxy.GetDefaultProxy();

            string cRespuestaXml = "";
            string cNombreDocumento = "";



            try
            {
                tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1.ToString(), cData2, cData3);
                //tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1, cData2.InnerXml, cData3);
            }
            catch (Exception ex)
            {
                //Variable para controlar Errores
                cResultados[0] = ex.Message.ToString().ToUpper();


                cResultados[1] = "False";
                cResultados[2] = "";
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";

                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[0] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";
                cResultados[12] = cRespuestaXml;


            }
            try
            {
                //Variable para controlar si es true o false
                cResultados[1] = tt.Response.Result.ToString();
                //Es el Response Data
                cResultados[2] = tt.ResponseData.ResponseData1.ToString();
            }
            catch (Exception ex)
            {

                cResultados[1] = "False";

                cResultados[2] = "POR FAVOR VERIFICAR SU CONEXION DE INTERNET SI NO COMUNIQUESE CON SU PROVEEDOR DE FACTURA ELECTRONICA " + ex.Message.ToString();
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";


                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";

                cResultados[12] = cRespuestaXml;
            }


            try
            {

                if (tt.Response.Result == true)
                {
                    cResultados[3] = tt.Response.Identifier.Batch.ToString();
                    cResultados[4] = tt.Response.Identifier.Serial.ToString();
                    cResultados[5] = Base64String_String(tt.ResponseData.ResponseData1);
                    cFirma.InnerXml = cResultados[5];
                    cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();


                    cResultados[7] = tt.Response.TimeStamp.ToString();
                    cResultados[8] = "6001020-7";

                    cResultados[9] = "";// tt.ResponseData.ResponseData3.ToString();
                    
                    
                    
                    
                    cResultados[10] = tt.Response.Identifier.InternalID.Trim();
                    cResultados[11] = "";

                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Dte>" + cResultados[2] + "</Dte>";
                    cRespuestaXml += "<NumeroIdentificacion>" + cResultados[6].ToString() + "</NumeroIdentificacion>";
                    cRespuestaXml += "<NumeroDocumento>" + cResultados[4].ToString() + "</NumeroDocumento>";
                    cRespuestaXml += "<SerieDocumento>" + cResultados[3].ToString() + "</SerieDocumento>";
                    cRespuestaXml += "</Resultados>";
                    cResultados[12] = cRespuestaXml;

                    if (lPdf == true)
                    {
                        try
                        {

                            /*
                            cNombreDocumento = cResultados[3] + "-" + cResultados[4] + ".xml";
                            cRutaXslt = ".\\Dte\\" + cNombreDocumento;
                            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaXslt, System.IO.FileMode.Create);


                            oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData1), 0, Base64String_ByteArray(tt.ResponseData.ResponseData1).Length);
                            oFileStream.Close();

                            XPathDocument myXPathDoc = new XPathDocument(cRutaXslt);

                            XslTransform myXslTrans = new XslTransform();
                            myXslTrans.Load(".\\Xslt\\formato.xslt");

                            XmlTextWriter myWriter = new XmlTextWriter(".\\Html\\"+cNombreDocumento.Replace(".xml", ".html"), null);

                            myXslTrans.Transform(myXPathDoc, null, myWriter);
                            */

                            cNombreDocumento = cResultados[3] + "-" + cResultados[4] + ".pdf";
                            //cNombreDocumento = cFace + ".pdf";
                            cRutaPdf = cRutaPdf + cNombreDocumento;
                            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                            oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData3), 0, Base64String_ByteArray(tt.ResponseData.ResponseData3).Length);
                            oFileStream.Close();
                        }
                        catch (Exception ex)
                        {

                            //System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    cResultados[0] = "";
                    cResultados[1] = "False";
                    cResultados[2] = tt.Response.Data.ToString();
                    cResultados[3] = tt.Response.Code.ToString();
                    cResultados[4] = "";
                    cResultados[5] = "";
                    cResultados[6] = "";
                    cResultados[7] = "";
                    cResultados[8] = "";
                    cResultados[9] = "";
                    cResultados[10] = "";
                    cResultados[11] = "";
                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                    cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                    cRespuestaXml += "</Resultados>";

                    cResultados[12] = cRespuestaXml;

                }
            }
            catch
            {
            }
            return cResultados;
        }


        public static string[] wsGet(string cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)
        {
            //Variables que haran el retorno
            string[] cResultados = new string[13];
            XmlDocument cFirma = new XmlDocument();
            wsServicio.Url = new Uri(cUrl).ToString();

            //string cRutaXslt = "";
            //wsServicio.Proxy = System.Net.WebProxy.GetDefaultProxy();

            string cRespuestaXml = "";
            string cNombreDocumento = "";



            try
            {
                tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1.ToString(), cData2, cData3);
                //tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1, cData2.InnerXml, cData3);
            }
            catch (Exception ex)
            {
                //Variable para controlar Errores
                cResultados[0] = ex.Message.ToString().ToUpper();


                cResultados[1] = "False";
                cResultados[2] = "";
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";

                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[0] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";
                cResultados[12] = cRespuestaXml;


            }
            try
            {
                //Variable para controlar si es true o false
                cResultados[1] = tt.Response.Result.ToString();
                //Es el Response Data
                cResultados[2] = tt.ResponseData.ResponseData1.ToString();
            }
            catch (Exception ex)
            {

                cResultados[1] = "False";

                cResultados[2] = "POR FAVOR VERIFICAR SU CONEXION DE INTERNET SI NO COMUNIQUESE CON SU PROVEEDOR DE FACTURA ELECTRONICA " + ex.Message.ToString();
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";


                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";

                cResultados[12] = cRespuestaXml;
            }


            try
            {

                if (tt.Response.Result == true)
                {
                    cResultados[3] = tt.Response.Identifier.Batch.ToString();
                    cResultados[4] = tt.Response.Identifier.Serial.ToString();
                    cResultados[5] = Base64String_String(tt.ResponseData.ResponseData1);
                    cFirma.InnerXml = cResultados[5];
                    cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();


                    cResultados[7] = tt.Response.TimeStamp.ToString();
                    cResultados[8] = "6001020-7";
                    cResultados[9] = tt.ResponseData.ResponseData3.ToString() ;
                    cResultados[10] = "";
                    cResultados[11] = "";

                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Dte>" + cResultados[2] + "</Dte>";
                    cRespuestaXml += "<NumeroIdentificacion>" + cResultados[6].ToString() + "</NumeroIdentificacion>";
                    cRespuestaXml += "<NumeroDocumento>" + cResultados[4].ToString() + "</NumeroDocumento>";
                    cRespuestaXml += "<SerieDocumento>" + cResultados[3].ToString() + "</SerieDocumento>";
                    cRespuestaXml += "</Resultados>";
                    cResultados[12] = cRespuestaXml;

                    if (lPdf == true)
                    {
                        try
                        {
                                                      

                            cNombreDocumento = cResultados[6]+".pdf";

                            cNombreDocumento = "c:\\PDF\\"+cResultados[6]+".pdf";

                            //cRutaPdf = cRutaPdf + cNombreDocumento;
                            cRutaPdf = cNombreDocumento;
                            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                            oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData3), 0, Base64String_ByteArray(tt.ResponseData.ResponseData3).Length);
                            oFileStream.Close();
                        }
                        catch (Exception ex)
                        {

                            //System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    cResultados[0] = "";
                    cResultados[1] = "False";
                    cResultados[2] = tt.Response.Data.ToString();
                    cResultados[3] = tt.Response.Code.ToString();
                    cResultados[4] = "";
                    cResultados[5] = "";
                    cResultados[6] = "";
                    cResultados[7] = "";
                    cResultados[8] = "";
                    cResultados[9] = "";
                    cResultados[10] = "";
                    cResultados[11] = "";
                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                    cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                    cRespuestaXml += "</Resultados>";

                    cResultados[12] = cRespuestaXml;

                }
            }
            catch
            {
            }
            return cResultados;
        }

        public static string[] wsObtener(string cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)
        {
            //Variables que haran el retorno
            string[] cResultados = new string[12];
            XmlDocument cFirma = new XmlDocument();
            //wsServicio.Url = new Uri(cUrl).ToString();
            string cNombreDocumento = "";



            try
            {
                tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1, cData2, cData3);
            }
            catch (Exception ex)
            {
                //Variable para controlar Errores
                cResultados[0] = ex.Message.ToString().ToUpper();


                cResultados[1] = "False";
                cResultados[2] = "";
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";

            }
            try
            {
                //Optra general motor gm Eduardo Martinez 
                //Variable para controlar si es true o false
                cResultados[1] = tt.Response.Result.ToString();
                //Es el Response Data
                cResultados[2] = tt.ResponseData.ResponseData1.ToString();
            }
            catch (Exception ex)
            {

                cResultados[1] = "False";

                cResultados[2] = "PROBLEMA ENCONTRADO AL TRATAR DE HACER LA COMUNICACIÓN " + ex.Message.ToString();
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";

            }

            try
            {

                if (tt.Response.Result == true)
                {
                    cResultados[3] = tt.Response.Identifier.Batch.ToString();
                    cResultados[4] = tt.Response.Identifier.Serial.ToString();
                    cResultados[5] = "";

                    cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();

                    cResultados[10] = "6001020-7";
                    cResultados[11] = "";
                    cResultados[12] = "";



                }
                else
                {
                    cResultados[0] = "False";
                    cResultados[1] = "False";
                    cResultados[2] = tt.Response.Data.ToString();
                    cResultados[3] = "";
                    cResultados[4] = "";
                    cResultados[5] = "";
                    cResultados[6] = "";
                    cResultados[7] = "";
                    cResultados[8] = "";
                    cResultados[9] = "";
                    cResultados[10] = "";
                    cResultados[11] = "";



                }
            }
            catch
            {
            }
            return cResultados;

        }


        /// <summary>
        /// Vamos hacer el webservice para obtener las sucursales o establecimientos de un cliente
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>

        public static string[] wsEstablecimientos(string cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)
        {
            //Variables que haran el retorno
            string[] cResultados = new string[13];
            XmlDocument cFirma = new XmlDocument();
            wsServicio.Url = new Uri(cUrl).ToString();

            //string cRutaXslt = "";
            //wsServicio.Proxy = System.Net.WebProxy.GetDefaultProxy();

            string cRespuestaXml = "";
            string cNombreDocumento = "";



            try
            {
                tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1.ToString(), cData2, cData3);
                //tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1, cData2.InnerXml, cData3);
            }
            catch (Exception ex)
            {
                //Variable para controlar Errores
                cResultados[0] = ex.Message.ToString().ToUpper();


                cResultados[1] = "False";
                cResultados[2] = "";
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";

                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[0] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";
                cResultados[12] = cRespuestaXml;


            }
            try
            {
                //Variable para controlar si es true o false
                cResultados[1] = tt.Response.Result.ToString();
                //Es el Response Data
                cResultados[2] = tt.ResponseData.ResponseData1.ToString();
            }
            catch (Exception ex)
            {

                cResultados[1] = "False";

                cResultados[2] = "POR FAVOR VERIFICAR SU CONEXION DE INTERNET SI NO COMUNIQUESE CON SU PROVEEDOR DE FACTURA ELECTRONICA " + ex.Message.ToString();
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";


                cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                cRespuestaXml = "<Resultados>";
                cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                cRespuestaXml += "</Resultados>";

                cResultados[12] = cRespuestaXml;
            }


            try
            {

                if (tt.Response.Result == true)
                {
                    cResultados[3] = tt.Response.Identifier.Batch.ToString();
                    cResultados[4] = tt.Response.Identifier.Serial.ToString();
                    cResultados[5] = Base64String_String(tt.ResponseData.ResponseData1);
                    cFirma.InnerXml = cResultados[5];
                    cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();


                    cResultados[7] = tt.Response.TimeStamp.ToString();
                    cResultados[8] = "6001020-7";

                    cResultados[9] = tt.ResponseData.ResponseData3.ToString();




                    cResultados[10] = "";
                    cResultados[11] = "";

                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Dte>" + cResultados[2] + "</Dte>";
                    cRespuestaXml += "<NumeroIdentificacion>" + cResultados[6].ToString() + "</NumeroIdentificacion>";
                    cRespuestaXml += "<NumeroDocumento>" + cResultados[4].ToString() + "</NumeroDocumento>";
                    cRespuestaXml += "<SerieDocumento>" + cResultados[3].ToString() + "</SerieDocumento>";
                    cRespuestaXml += "</Resultados>";
                    cResultados[12] = cRespuestaXml;

                    if (lPdf == true)
                    {
                        try
                        {

                            /*
                            cNombreDocumento = cResultados[3] + "-" + cResultados[4] + ".xml";
                            cRutaXslt = ".\\Dte\\" + cNombreDocumento;
                            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaXslt, System.IO.FileMode.Create);


                            oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData1), 0, Base64String_ByteArray(tt.ResponseData.ResponseData1).Length);
                            oFileStream.Close();

                            XPathDocument myXPathDoc = new XPathDocument(cRutaXslt);

                            XslTransform myXslTrans = new XslTransform();
                            myXslTrans.Load(".\\Xslt\\formato.xslt");

                            XmlTextWriter myWriter = new XmlTextWriter(".\\Html\\"+cNombreDocumento.Replace(".xml", ".html"), null);

                            myXslTrans.Transform(myXPathDoc, null, myWriter);
                            */

                            cNombreDocumento = cResultados[3] + "-" + cResultados[4] + ".pdf";
                            //cNombreDocumento = cFace + ".pdf";
                            cRutaPdf = cRutaPdf + cNombreDocumento;
                            System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                            oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData3), 0, Base64String_ByteArray(tt.ResponseData.ResponseData3).Length);
                            oFileStream.Close();
                        }
                        catch (Exception ex)
                        {

                            //System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    cResultados[0] = "";
                    cResultados[1] = "False";
                    cResultados[2] = tt.Response.Data.ToString();
                    cResultados[3] = tt.Response.Code.ToString();
                    cResultados[4] = "";
                    cResultados[5] = "";
                    cResultados[6] = "";
                    cResultados[7] = "";
                    cResultados[8] = "";
                    cResultados[9] = "";
                    cResultados[10] = "";
                    cResultados[11] = "";
                    cRespuestaXml = "<?xml version = '1.0' encoding = 'utf8'?>";
                    cRespuestaXml = "<Resultados>";
                    cRespuestaXml += "<Resultado>" + cResultados[1].ToString() + "</Resultado>";
                    cRespuestaXml += "<Error>" + cResultados[2] + "</Error>";
                    cRespuestaXml += "<Code>" + cResultados[3].ToString() + "</Code>";
                    cRespuestaXml += "</Resultados>";

                    cResultados[12] = cRespuestaXml;

                }
            }
            catch
            {
            }
            return cResultados;
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


    }
}