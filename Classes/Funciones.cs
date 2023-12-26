using DevExpress.CodeParser;
using EqCrm.ConsultaNitInfile;
using EqCrm.Models;
using EqCrm.NitsInfile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

/// Clase para llevar control de funciones principales del EqWeb

namespace EqCrm
{
    public class Funciones
    {
        // Creamos una instancia del conector de MySQL
        
        static StringConexionMySQL mysql = new StringConexionMySQL();

        private static SoapDigifact.FELWSFRONT wsServicio = new SoapDigifact.FELWSFRONT();
        private static SoapDigifact.TransactionTag tt = new SoapDigifact.TransactionTag();




        public static bool ProcesarAnulacion(string docto,string serie, string idcliente, string ctabla,string oDb, string cUser )
        {

            string cInst = "";
            dapperConnect eqdaper = new dapperConnect();
            bool lError = false;
            string cError = "";
            

            cInst = "UPDATE "+ctabla+" SET status = 'A' WHERE no_factura = @NoFactura AND serie = @Serie";

            // Creamos los parametros para factura
            object[] parametrosfactura = { new { NoFactura = docto, Serie = serie } };
            lError = eqdaper.EqExecuteParam(cInst, oDb, ref cError, parametrosfactura);
            if (lError == true)
            {
                return false;
            }


            cInst = "UPDATE ctacc SET importe = 0, saldo=0 WHERE docto = @NoFactura AND serie = @Serie AND id_codigo=@IdCliente";

            object[] parametrosctacc = { new { NoFactura = docto, Serie = serie, IdCliente=idcliente, cObs="ANULADO DESDE SISTEMA WEB POR EL USUARIO "+cUser } };
            lError = eqdaper.EqExecuteParam(cInst, oDb, ref cError, parametrosctacc);
            if (lError == true)
            {
                return false;
            }


            cInst = "UPDATE kardexinven SET OBS = @cObs, salida=0 WHERE docto = @NoFactura AND serie = @Serie AND id_movi = 51";

            object[] parametroskardex = { new { NoFactura = docto, Serie = serie, cObs = "ANULADO DESDE SISTEMA WEB POR EL USUARIO " + cUser } };
            lError = eqdaper.EqExecuteParam(cInst, oDb, ref cError, parametroskardex);
            if (lError == true)
            {
                return false;
            }




            return true;

        }




        public static string cCadenaJson(string json,string cValor)
        {
            string cCadena = "";
            JArray jsonParseo = JArray.Parse(json);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    cCadena = jsonOperaciones[cValor].ToString();
                }
            }
            return cCadena;
        }



        public static string LlenarTableHTmlInventarioPOS(string jSon)
        {
            string cTableHTml = "";
            bool lEncabezado = true;
            JArray jsonParseo = JArray.Parse(jSon);

            StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablainventario\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {
                    foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                    {
                        if (lEncabezado == true)
                        {

                            sb.Append("<th>");
                            sb.Append(jsonOPropiedades.Name);
                            sb.Append("</th>");
                        }
                    }
                    lEncabezado = false;
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                

                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionarinventario(this);\">");
                    foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                    {
                        sb.Append("<td>");
                        sb.Append(jsonOPropiedades.Value.ToString());
                        sb.Append("</td>");

                    }
                    sb.Append("</tr>");

                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
           

            return cTableHTml;
        }


        public static string LlenarTableHTmlVentas(string jSon)
        {
            string cTableHTml = "";
            bool lEncabezado = true;
            JArray jsonParseo = JArray.Parse(jSon);
            string cScript = "";

            cScript = "<script>";
            cScript += "$('#tablafac').dataTable(";
            cScript += "{";
            cScript += "responsive: true,";
            cScript += "    lengthChange: false,";
            cScript += "    dom:";
            cScript += "    \" < 'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB >> \" +";
            cScript += "    \" < 'row'<'col-sm-12'tr >> \" +";
            cScript += "    \" < 'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p >> \",";
            cScript += "    buttons: [";           
            cScript += "    ]";
            cScript += "});";
            cScript += "</script>";


            StringBuilder sb = new StringBuilder();
            sb.Append(cScript);
            sb.Append("<table id=\"tablafac\" class=\"table table-bordered table-hover table-striped w-100 dataTable dtr-inline\">");
                                                        
            sb.Append("<thead class=\"bg-primary-600\">");
            sb.Append("<tr>");
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    if (lEncabezado == true)
                    {

                        sb.Append("<th>");
                        sb.Append(jsonOPropiedades.Name);
                        sb.Append("</th>");
                    }
                }
                lEncabezado = false;
            }
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append(" <tbody>");


            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                sb.Append("<tr id=\"fila\" onclick=\"seleccionarfactura(this);\">");
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    sb.Append("<td>");
                    sb.Append(jsonOPropiedades.Value.ToString());
                    sb.Append("</td>");

                }
                sb.Append("</tr>");

            }
            sb.Append(" <tbody>");
            sb.Append("</table>");
            cTableHTml = sb.ToString();


            return cTableHTml;
        }


        /// <summary>
        /// Funcion para llenar htmltable con Json powered by YiPonce
        /// </summary>
        /// <param name="jSon"></param>
        /// <returns></returns>
        public static string LlenarTableHTmlClientePOS(string jSon)
        {
                string cTableHTml = "";
                bool lEncabezado = true;

               JArray jsonParseo = JArray.Parse(jSon);

                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
              

                foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                {
                    foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                    {
                        if (lEncabezado == true)
                        {

                            sb.Append("<th>");
                            sb.Append(jsonOPropiedades.Name);
                            sb.Append("</th>");
                        }
                    }
                   lEncabezado = false; 
                }

                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                
                    foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
                    {
                       sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
                      foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                        {
                            sb.Append("<td>");
                            sb.Append(jsonOPropiedades.Value.ToString());
                            sb.Append("</td>");

                        }
                      sb.Append("</tr>");

                    }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
           

            return cTableHTml;
        }




        public static string LlenarTableHTmlFactNC(string jSon)
        {
            string cTableHTml = "";
            bool lEncabezado = true;

            string cScript = "<script>";
            cScript += "$('#tablafactnc').dataTable(";
            cScript += "{";
            cScript += "responsive: true,";
            cScript += "    lengthChange: false,";
            cScript += "    dom:";
            cScript += "    \" < 'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB >> \" +";
            cScript += "    \" < 'row'<'col-sm-12'tr >> \" +";
            cScript += "    \" < 'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p >> \",";
            cScript += "    buttons: [";
            cScript += "    ]";
            cScript += "});";
            cScript += "</script>";

            JArray jsonParseo = JArray.Parse(jSon);

            StringBuilder sb = new StringBuilder();
            sb.Append(cScript);
            sb.Append("<table id = \"tablafactnc\" class=\"table table-bordered table-hover table-striped w-100\">");

            sb.Append("<thead class=\"bg-primary-600\">");
            sb.Append("<tr>");


            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    if (lEncabezado == true)
                    {

                        sb.Append("<th>");
                        sb.Append(jsonOPropiedades.Name);
                        sb.Append("</th>");
                    }
                }
                lEncabezado = false;
            }

            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append(" <tbody>");

            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                sb.Append("<tr id=\"fila\" onclick=\"seleccionarFactura(this);\">");
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    sb.Append("<td>");
                    sb.Append(jsonOPropiedades.Value.ToString());
                    sb.Append("</td>");

                }
                sb.Append("</tr>");

            }
            sb.Append(" <tbody>");
            sb.Append("</table>");
            cTableHTml = sb.ToString();


            return cTableHTml;
        }









        public static string LlenarTableHTmlOrdenes(string jSon)
        {
            string cTableHTml = "";
            bool lEncabezado = true;

            string cScript = "<script>";
            cScript += "$('#tablaorden').dataTable(";
            cScript += "{";
            cScript += "responsive: true,";
            cScript += "    lengthChange: false,";
            cScript += "    dom:";
            cScript += "    \" < 'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB >> \" +";
            cScript += "    \" < 'row'<'col-sm-12'tr >> \" +";
            cScript += "    \" < 'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p >> \",";
            cScript += "    buttons: [";
            cScript += "    ]";
            cScript += "});";
            cScript += "</script>";

            JArray jsonParseo = JArray.Parse(jSon);

            StringBuilder sb = new StringBuilder();
            sb.Append(cScript);
            sb.Append("<table id = \"tablaorden\" class=\"table table-bordered table-hover table-striped w-100\">");

            sb.Append("<thead class=\"bg-primary-600\">");
            sb.Append("<tr>");


            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    if (lEncabezado == true)
                    {
                        sb.Append("<th>");
                        sb.Append(jsonOPropiedades.Name);
                        sb.Append("</th>");
                    }
                }
                lEncabezado = false;
            }

            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append(" <tbody>");

            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                sb.Append("<tr id=\"fila\" onclick=\"seleccionarOrden(this);\">");
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    sb.Append("<td>");
                    sb.Append(jsonOPropiedades.Value.ToString());
                    sb.Append("</td>");

                }
                sb.Append("</tr>");

            }
            sb.Append(" <tbody>");
            sb.Append("</table>");
            cTableHTml = sb.ToString();


            return cTableHTml;
        }




        /// <summary>
        /// Funcion que sirve para llenar un Select a Base de un Json esta funcion es una gran fumada que viene de mi cabeza y que gracias a DIOS aun estoy vivo powered by YeiPonce
        /// </summary>
        /// <param name="jSon"></param>
        /// <param name="idselect"></param>
        /// <returns></returns>
        public static string llenarSelect(string jSon, string idselect)
        {
            string cValue = "";
            string cValue2 = "";


            //string cSelect = "<select class=\"form-control\" id=\"BaseDatos\" name=\"BaseDatos\"><option value=\"\" >Selecccionar </option>";

            string cSelect = "<select class=\"form-control\" id=\"idselect\" name=\"idselect\"><option value=\"\" >Selecccionar </option>";
            cSelect = cSelect.Replace("idselect", idselect);
            JArray jsonParseo = JArray.Parse(jSon);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                //Aqui para poder identificar las propiedades y sus valores  con esto hare el htmltable
                 foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                 {
                    //cSelect += "<option value=\"" + jsonOPropiedades.Name + "\">" + jsonOPropiedades.Value.ToString() + "</option>";
                    if (cValue == "")
                    {
                        cValue = jsonOPropiedades.Value.ToString();
                    }
                    else
                    {
                        cValue2 = jsonOPropiedades.Value.ToString();
                    }

                }

                cSelect += "<option value=\"" + cValue + "\">" + cValue2 + "</option>";
                cValue = "";
                cValue2 = "";
                //Aqui se puede acceder al objeto y obtener sus valores
                //string base_datos = jsonOperaciones["base_datos"].ToString();
                //string nombre_empresa = jsonOperaciones["nombre_empresa"].ToString();



            }
            cSelect += "</select>";


            return cSelect;
        }


        public static string llenarEstablecimientos(string jSon, string idselect)
        {
            string cValue = "";

            
            string cSelect = "<div id=\"idselect\" class=\"col-lg-2\"><div class=\"btn-group\"> ";
            cSelect = cSelect + "<button type=\"button\" id=\"idbtnestablecimiento\" class=\"btn btn-dark dropdown-toggle bg-brand-gradient waves-effect waves-themed\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">Establecimientos</button> <div class=\"dropdown-menu fadeinup\" style=\"\"><h6 class=\"dropdown-header\">Favor seleccionar Establecimiento</h6>";
            cSelect = cSelect.Replace("idselect", idselect);



            JArray jsonParseo = JArray.Parse(jSon);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                //Aqui para poder identificar las propiedades y sus valores  con esto hare el htmltable
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    //cSelect += "<option value=\"" + jsonOPropiedades.Name + "\">" + jsonOPropiedades.Value.ToString() + "</option>";
                        cValue = jsonOperaciones["id_establecimiento"].ToString() + " | " +jsonOperaciones["Nombre"].ToString();
                    

                }

                cSelect += "<button id=\""+ jsonOperaciones["id_establecimiento"].ToString() + "\" class=\"dropdown-item\"  onClick=\"SetearEstablecimiento("+ jsonOperaciones["id_establecimiento"].ToString() + ")\"         type=\"button\">" + cValue + "</button>";
                cValue = "";
                
                //Aqui se puede acceder al objeto y obtener sus valores
                //string base_datos = jsonOperaciones["base_datos"].ToString();
                //string nombre_empresa = jsonOperaciones["nombre_empresa"].ToString();



            }
            cSelect += "</div></div></div>";


            return cSelect;
        }



        public static string llenarFrases(string jSon, string idselect)
        {
            string cValue = "";


            string cSelect = "<div id=\"idselect\" class=\"col-lg-2\"><div class=\"btn-group\"> ";
            cSelect = cSelect + "<button type=\"button\" id=\"idbtnfrases\" class=\"btn btn-dark dropdown-toggle bg-brand-gradient waves-effect waves-themed\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">Frases Exentas</button> <div class=\"dropdown-menu fadeinup\" style=\"\"><h6 class=\"dropdown-header\">Favor seleccionar Frase</h6>";
            cSelect = cSelect.Replace("idselect", idselect);



            JArray jsonParseo = JArray.Parse(jSon);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                //Aqui para poder identificar las propiedades y sus valores  con esto hare el htmltable
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    
                    cValue = jsonOperaciones["codigoEscenario"].ToString() + " | " + jsonOperaciones["escenario"].ToString();


                }

                cSelect += "<button id=\"" + cValue + "\" class=\"dropdown-item\"  onClick=\"SetearFrase('" + cValue + "')\"         type=\"button\">" + cValue + "</button>";
                cValue = "";

                //Aqui se puede acceder al objeto y obtener sus valores
                //string base_datos = jsonOperaciones["base_datos"].ToString();
                //string nombre_empresa = jsonOperaciones["nombre_empresa"].ToString();



            }
            cSelect += "</div></div></div>";


            return cSelect;
        }


        public static string llenarSelectOnChange(string jSon, string idselect,string cFunction)
        {
            string cValue = "";
            string cValue2 = "";


            //string cSelect = "<select class=\"form-control\" id=\"BaseDatos\" name=\"BaseDatos\"><option value=\"\" >Selecccionar </option>";

            string cSelect = "<select onchange=\"function\" class=\"form-control\" id=\"idselect\" name=\"idselect\"><option value=\"\" >Selecccionar </option>";
            cSelect = cSelect.Replace("idselect", idselect).Replace("function",cFunction);
            JArray jsonParseo = JArray.Parse(jSon);
            foreach (JObject jsonOperaciones in jsonParseo.Children<JObject>())
            {
                //Aqui para poder identificar las propiedades y sus valores  con esto hare el htmltable
                foreach (JProperty jsonOPropiedades in jsonOperaciones.Properties())
                {
                    //cSelect += "<option value=\"" + jsonOPropiedades.Name + "\">" + jsonOPropiedades.Value.ToString() + "</option>";
                    if (cValue == "")
                    {
                        cValue = jsonOPropiedades.Value.ToString();
                    }
                    else
                    {
                        cValue2 = jsonOPropiedades.Value.ToString();
                    }

                }

                cSelect += "<option value=\"" + cValue + "\">" + cValue2 + "</option>";
                cValue = "";
                cValue2 = "";
                //Aqui se puede acceder al objeto y obtener sus valores
                //string base_datos = jsonOperaciones["base_datos"].ToString();
                //string nombre_empresa = jsonOperaciones["nombre_empresa"].ToString();



            }
            cSelect += "</select>";


            return cSelect;
        }



        /// <summary>
        /// Funcion para obtner un query de consulta a un api rest del Equilibrium
        /// </summary>
        /// <param name="jSon"></param>
        /// <returns></returns>
        public static string  EqAppQuery(string jSon)
        {
            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];

            var client = new RestClient(cUrlApiQuery);
            IRestResponse response;
            var requestpost = new RestRequest(cUrlApiQuery,RestSharp.Method.POST);

            requestpost.AddHeader("Content-Type", "text/plain");
            requestpost.AddParameter("text/plain", jSon, ParameterType.RequestBody);
            response = client.Execute(requestpost);

            return response.Content.ToString();
        }

        public static string EqAppInsertQuery(string jSon)
        {
            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiInsertQuery"];

            var client = new RestClient(cUrlApiQuery);
            IRestResponse response;
            var requestpost = new RestRequest(cUrlApiQuery, RestSharp.Method.POST);

            requestpost.AddHeader("Content-Type", "text/plain");
            requestpost.AddParameter("text/plain", jSon, ParameterType.RequestBody);
            response = client.Execute(requestpost);

            return response.Content.ToString();
        }

        public static string ExtraerInfo(string cCadena, string cInicio, string cFin)
        {
            string pattern = cInicio + "([^|]*)" + cFin;
            string input = cCadena;
            Match match = Regex.Match(input, pattern);
            return match.Groups[1].Value.ToString();
        }


        public static string ExtraerInfoEqJson(string cCadena )
        {
            string cInicio = "<inicio>";
            string cFin = "<fin>";
            cCadena = cCadena.Replace("[", "<inicio>[");
            cCadena = cCadena.Replace("]", "]<fin>");
            string pattern = cInicio + "([^|]*)" + cFin;
            string input = cCadena;
            Match match = Regex.Match(input, pattern);
            return match.Groups[1].Value.ToString().Replace("<inicio>","[").Replace("<fin>", "]");
        }

        public string GetDataNit(string nit)
        {

            DatosCliente cliente = new DatosCliente();
            NitsInfile.ingfaceWsServicesClient ingface = new NitsInfile.ingfaceWsServicesClient();
            string cCui = "CUI|";

            if (nit.Length >= 13)
            {
                DataSet ds = new DataSet();
                cCui = cCui + nit.ToString().Trim();
                tt = wsServicio.RequestTransaction("043EAD3C-D4DC-4850-83FF-CA1EFF7E37D6", "SHARED_INFO_EFACE", "GT", "000108284255","043EAD3C-D4DC-4850-83FF-CA1EFF7E37D6", "GT.000108284255.108284255", "SHARED_GETINFOCUI", cCui, "");

                ds = tt.ResponseData.ResponseDataSet;
                if (tt.Response.Result == true)
                {
                    DataTable dt = ds.Tables[0];

                    DataRow row = dt.Rows[0];

                    cliente.NIT = nit;
                    cliente.CORREO = "";
                    cliente.CLIENTE = row["NOMBRE"].ToString();
                    cliente.DIASCRED = 0;
                    cliente.FACTURAR = row["NOMBRE"].ToString();
                    cliente.DIRECCION = "CIUDAD";
                }
                if (tt.Response.Result == false)
                {
                    cliente.NIT = nit;
                    cliente.CLIENTE = "CUI NO ENCONTRADO";
                    cliente.DIRECCION = "CIUDAD";

                }




            }
            else
            {
                var cRespuesta = ingface.nitContribuyentes("CONSUMO_NIT", "58B45D8740C791420C53A49FFC924A1B58B45D8740C791420C53A49FFC924A1B", nit.Replace("-", ""));

                if (cRespuesta.nombre == "Nit no valido")
                {
                    cliente.NIT = nit;
                    cliente.CORREO = "";
                    cliente.CLIENTE = "NIT NO SE ENCUENTRA EN LA SAT O NO ESTA ACTUALIZADO";
                    cliente.DIASCRED = 0;
                    cliente.FACTURAR = "NIT NO SE ENCUENTRA EN LA SAT O NO ESTA ACTUALIZADO";
                    cliente.DIRECCION = "CIUDAD";

                }
                else
                {
                    cliente.NIT = nit;
                    cliente.CORREO = "";
                    cliente.CLIENTE = cRespuesta.nombre;
                    cliente.DIASCRED = 0;
                    cliente.FACTURAR = cRespuesta.nombre;
                    cliente.DIRECCION = cRespuesta.direccion_completa;

                }
            }


            string str = JsonConvert.SerializeObject(cliente).ToString();

            return str;
        }

       


        public bool DamePermisos(string nCodigo, string oDb)
        {
            //Obtenemos la url del api de consulta
            //string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            
            // Declaramos una variable para guardar el numero total de registros


            string cEstado = "";

            // Verificamos que los dos parametros tengan valores
            // Creamos la instruccion con los parametros obtenidos
            string instruccionSQL = "SELECT ingresa AS JSON FROM permisossistema WHERE cmodulo='" + nCodigo.ToString() + "'";

            EqAppQuery queryapi = new EqAppQuery()
            {
                Nit = "",
                Query = instruccionSQL,
                BaseDatos = oDb

            };

            string jsonString = JsonConvert.SerializeObject(queryapi);

            string cRespuesta = Funciones.EqAppQuery(jsonString);


            Resultado resultado = new Resultado();
            resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

            if (resultado.resultado.ToString() == "true")
            {                
                cEstado = resultado.Data[0].JSON.ToString();
            }

            
            if (cEstado == "S")
            {
                return true;
            }
            else
            {
                return false;
            }
            // Devolvemos la variable con el valor obtenido

        }


        public bool DamePermisosApp(string nCodigo, string cRol, string oDb)
        {
            // Declaramos una variable para guardar el numero total de registros
            int cEstado = 0;

            // Verificamos que los dos parametros tengan valores
            // Creamos la instruccion con los parametros obtenidos
            string instruccionSQL = "SELECT estado FROM dlempresa.usuario_menus WHERE id_menu='" + nCodigo.ToString() + "'";
            instruccionSQL += " AND usuario = '" + cRol + "'";

            mysql.EjecutarLectura(instruccionSQL, oDb);

            // Verificamos si la consulta obtuvo resultados
            if (mysql.consulta != null)
            {
                // Recorremos los datos encontrados
                while (mysql.consulta.Read())
                {
                    // Asignamos el registro a la variable global
                    cEstado = mysql.consulta.GetInt32(0);
                }

                // Cerramos la consulta
                mysql.CerrarConexion();
            }

            // Cerramos la consulta
            mysql.CerrarConexion();
            if (cEstado == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            // Devolvemos la variable con el valor obtenido

        }



        public void CrearBitacoraOS(string no_orden,string serie,string status,string fecha,string obs,string id_agencia, string oDb)
        {
            bool lError = false;
            string cMensaje = "";
            string cError = "";
            string str = "";
            string cInst = "";
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            cInst = "INSERT INTO logordcompras (no_factura, serie, fecha, status, obs,id_agencia) ";
            cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}')",
                no_orden.ToString(),
                serie.ToString(),
                fecha.ToString(),
                status.ToString(),
                obs.ToString(),
                id_agencia.ToString());

            lError = stringConexionMySql.ExecCommand(cInst, oDb, ref cError);

            if (lError == true)
            {
                cMensaje = cError;
                stringConexionMySql.CerrarConexion();                

            }

        }

        public bool DamePermiso(string nCodigo, string cUser,string oDb)
        {
            // Declaramos una variable para guardar el numero total de registros
            int cEstado = 0;
            


            // Verificamos que los dos parametros tengan valores
                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "SELECT estado FROM dlempresa.usuario_categorias WHERE id_categoria='"+nCodigo.ToString()+"'";
                       instruccionSQL+= " AND usuario = '"+cUser+"'";       

                mysql.EjecutarLectura(instruccionSQL,oDb);

                // Verificamos si la consulta obtuvo resultados
                if (mysql.consulta != null)
                {
                    // Recorremos los datos encontrados
                    while (mysql.consulta.Read())
                    {
                        // Asignamos el registro a la variable global
                        cEstado = mysql.consulta.GetInt32(0);
                    }

                    // Cerramos la consulta
                    mysql.CerrarConexion();
                }
            
            // Cerramos la consulta
            mysql.CerrarConexion();
            if (cEstado==1)
            {
                return true;
            }
            else
            {
                return false;
            }
            // Devolvemos la variable con el valor obtenido
            
        }

        public void EnviarCorreoOS(string user, string username, string email,string nOS,string oDb)
        {
            /*-------------------------MENSAJE DE CORREO----------------------*/

            string id = nOS.ToString() + "|" + oDb.ToString();
            id = Funciones.Base64Encode(id);

            //Creamos un nuevo Objeto de mensaje
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //Direccion de correo electronico a la que queremos enviar el mensaje
            mmsg.To.Add(email.ToString());

            //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

            //Asunto
            mmsg.Subject = "SE HA GENERADO UNA ORDEN PARA SU AUTORIZACION ORDEN NO. " + username.ToUpper().ToString();
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            //Cuerpo del Mensaje
            mmsg.Body = " El usuario " + username.ToString() + " ha generado una Orden de Solicitud .\n\n";
            mmsg.Body += "Con el NO. "+nOS.ToString()+"\n\n";
            mmsg.Body += " Debe entrar a la dirección web de http://104.46.4.174:8085/IngresoOrden/VerOrden/" + id + " para atorizar la órden. \n\n";

           // "/ConsultarCotizaciones/VerCotizacion/" + id,"_blank"



            mmsg.Body += "\n\n\n\n\n\n\n\n";
            mmsg.Body += "Atentamente \n\n";
            mmsg.Body += "Sistema de Control de Ordenes \n\n";




            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

            //Correo electronico desde la que enviamos el mensaje
            mmsg.From = new System.Net.Mail.MailAddress("eqintcomex@gmail.com");


            /*-------------------------CLIENTE DE CORREO----------------------*/

            //Creamos un objeto de cliente de correo
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

            //Hay que crear las credenciales del correo emisor
            cliente.Credentials = new System.Net.NetworkCredential("eqintcomex@gmail.com", "Clave01*");

            //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
            /*
            cliente.Port = 587;
            cliente.EnableSsl = true;
            */

            cliente.Port = 587;
            cliente.EnableSsl = true;


            cliente.Host = "smtp.gmail.com"; //Para Gmail "smtp.gmail.com";


            /*-------------------------ENVIO DE CORREO----------------------*/

            try
            {
                //Enviamos el mensaje      
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                //Aquí gestionamos los errores al intentar enviar el correo
            }
        }

        public void EnviarCorreoProceso(string user, string username, string email, string nOS, string oDb)
        {
            /*-------------------------MENSAJE DE CORREO----------------------*/

            string id = nOS.ToString() + "|" + oDb.ToString();
            id = Funciones.Base64Encode(id);

            //Creamos un nuevo Objeto de mensaje
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //Direccion de correo electronico a la que queremos enviar el mensaje
            mmsg.To.Add(email.ToString());

            //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

            //Asunto
            mmsg.Subject = "Sistema de  " + username.ToUpper().ToString();
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            //Cuerpo del Mensaje
            mmsg.Body = " Le Informa que la orden ingresada por  " + username.ToString() + " ha sido autorizada con .\n\n";
            mmsg.Body += "el NO. " + nOS.ToString() + "\n\n";
            mmsg.Body += " Debe entrar a la dirección web de http://104.46.4.174:8085/IngresoOrden/VerOrden/" + id + " para continuar con el proceso. \n\n";

            // "/ConsultarCotizaciones/VerCotizacion/" + id,"_blank"



            mmsg.Body += "\n\n\n\n\n\n\n\n";
            mmsg.Body += "Atentamente \n\n";
            mmsg.Body += "Sistema de Control de Ordenes \n\n";




            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

            //Correo electronico desde la que enviamos el mensaje
            mmsg.From = new System.Net.Mail.MailAddress("eqintcomex@gmail.com");


            /*-------------------------CLIENTE DE CORREO----------------------*/

            //Creamos un objeto de cliente de correo
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

            //Hay que crear las credenciales del correo emisor
            cliente.Credentials = new System.Net.NetworkCredential("eqintcomex@gmail.com", "Clave01*");

            //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
            /*
            cliente.Port = 587;
            cliente.EnableSsl = true;
            */

            cliente.Port = 587;
            cliente.EnableSsl = true;


            cliente.Host = "smtp.gmail.com"; //Para Gmail "smtp.gmail.com";


            /*-------------------------ENVIO DE CORREO----------------------*/

            try
            {
                //Enviamos el mensaje      
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                //Aquí gestionamos los errores al intentar enviar el correo
            }
        }


        public void EnviarCorreoUsuario(string user, string username, string email, string cPass )
        {
            /*-------------------------MENSAJE DE CORREO----------------------*/

            

            //Creamos un nuevo Objeto de mensaje
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //Direccion de correo electronico a la que queremos enviar el mensaje
            mmsg.To.Add(email.ToString());

            //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

            //Asunto
            mmsg.Subject = "Estimado Usuario  " + username.ToUpper().ToString();
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            //Cuerpo del Mensaje
            mmsg.Body = " Le Informamos que ha sido dado de alta al sistema su cuenta con Usuario  " + username.ToString() + "\n\n";
            mmsg.Body += "Con el Password " + cPass.ToString() + "\n\n";
            mmsg.Body += " Debe entrar a la dirección web de http://104.46.4.174:8085 para continuar con el proceso. \n\n";

            

            mmsg.Body += "\n\n\n\n\n\n\n\n";
            mmsg.Body += "Atentamente \n\n";
            mmsg.Body += "Sistema EqWeb \n\n";


            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

            //Correo electronico desde la que enviamos el mensaje
            mmsg.From = new System.Net.Mail.MailAddress("eqintcomex@gmail.com");


            /*-------------------------CLIENTE DE CORREO----------------------*/

            //Creamos un objeto de cliente de correo
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

            //Hay que crear las credenciales del correo emisor
            cliente.Credentials = new System.Net.NetworkCredential("eqintcomex@gmail.com", "Clave01*");

            //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
            /*
            cliente.Port = 587;
            cliente.EnableSsl = true;
            */

            cliente.Port = 587;
            cliente.EnableSsl = true;


            cliente.Host = "smtp.gmail.com"; //Para Gmail "smtp.gmail.com";


            /*-------------------------ENVIO DE CORREO----------------------*/

            try
            {
                //Enviamos el mensaje      
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                //Aquí gestionamos los errores al intentar enviar el correo
            }
        }



        public bool DamePermisoApp(string nCodigo, string oDb)
        {
            // Declaramos una variable para guardar el numero total de registros
            string cEstado = "";



            // Verificamos que los dos parametros tengan valores
            // Creamos la instruccion con los parametros obtenidos
            string instruccionSQL = "SELECT ingresa FROM permisossistema WHERE cmodulo='" + nCodigo.ToString() + "'";
            

            mysql.EjecutarLectura(instruccionSQL, oDb);

            // Verificamos si la consulta obtuvo resultados
            if (mysql.consulta != null)
            {
                // Recorremos los datos encontrados
                while (mysql.consulta.Read())
                {
                    // Asignamos el registro a la variable global
                    cEstado = mysql.consulta.GetString(0);
                }

                // Cerramos la consulta
                mysql.CerrarConexion();
            }

            // Cerramos la consulta
            mysql.CerrarConexion();
            if (cEstado == "S")
            {
                return true;
            }
            else
            {
                return false;
            }
            // Devolvemos la variable con el valor obtenido

        }

        public static string[] leeCarpeta(string Ruta)
        {
            string[] dirs;           
            dirs = Directory.GetFiles(Ruta);
            return dirs;
        }


        public static string NumLetras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try

            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
            if (decimales > 0)
            {
                dec = " CON " + decimales.ToString() + "/100";
            }

            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        public static string toText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            return Num2Text;

        }

    

    public static string RemoveBom(string p)
        {
            string BOMMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (p.StartsWith(BOMMarkUtf8))
                p = p.Remove(0, BOMMarkUtf8.Length);
            return p.Replace("\0", "");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);

        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        // vamos a obtener la fecha del servidor

        public string dFechaServer(string BaseDeDatos)
        {

            StringConexionMySQL fecha = new StringConexionMySQL();
            string cFecha = "";

                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "SELECT CURDATE()";
               

                fecha.EjecutarLectura(instruccionSQL, BaseDeDatos);

                // Verificamos si la consulta obtuvo resultados
                if (fecha.consulta != null)
                {
                    // Recorremos los datos encontrados
                    while (fecha.consulta.Read())
                    {
                        // Asignamos el registro a la variable global
                        cFecha = fecha.consulta.GetDateTime(0).ToString("yyyy-MM-dd");
                    }

                    // Cerramos la consulta
                    fecha.CerrarConexion();
                }

            fecha.CerrarConexion();
            return cFecha;
        }
            
            
        

        // Procedimiento para obtener el numero mayor en determinado campo de una tabla
        public static int NumeroMayor(string Tabla, string Campo, string Condicion, string BaseDeDatos)
        {
            // Declaramos una variable para guardar el numero total de registros
            int NumeroMayor = 0;
            ConexionMySQL mysql = new ConexionMySQL();


            // Verificamos que los dos parametros tengan valores
            if (Tabla != "" && Campo != "")
            {
                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "";
                instruccionSQL += String.Format("SELECT IFNULL(max({0}), 0) ", Campo);
                instruccionSQL += String.Format("FROM {0} ", Tabla);
                if (Condicion != "")
                {
                    instruccionSQL += string.Format("WHERE {0} ", Condicion);
                }


                mysql.EjecutarLectura(instruccionSQL, BaseDeDatos);

                // Verificamos si la consulta obtuvo resultados
                if (mysql.consulta != null)
                {
                    // Recorremos los datos encontrados
                    while (mysql.consulta.Read())
                    {
                        // Asignamos el registro a la variable global
                        NumeroMayor = mysql.consulta.GetInt32(0);
                    }

                    // Cerramos la consulta
                    mysql.CerrarConexion();
                }
            }
            // Cerramos la consulta
            mysql.CerrarConexion();
            // Devolvemos la variable con el valor obtenido
            NumeroMayor = NumeroMayor + 1;
            return NumeroMayor;
        }
        public string ObtieneDatos(string Tabla, string Campo, string Condicion, string BaseDeDatos)
        {
            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            string cCampo="";

            // Verificamos que los dos parametros tengan valores
            if (Tabla != "" && Campo != "" && Condicion != "id=VACIO")
            {
                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "";
                instruccionSQL += string.Format("SELECT {0} AS JSON ", Campo);
                instruccionSQL += string.Format("FROM {0}  ", Tabla);
                if (Condicion != "")
                {
                    instruccionSQL += string.Format("WHERE {0} ", Condicion);
                }

                EqAppQuery queryapi = new EqAppQuery()
                {
                    Nit = "",
                    Query = instruccionSQL,
                    BaseDatos = BaseDeDatos

                };

                string jsonString = JsonConvert.SerializeObject(queryapi);

                string cRespuesta = Funciones.EqAppQuery(jsonString);

                Resultado resultado = new Resultado();
                resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<Resultado>(cRespuesta.ToString());

                if (resultado.resultado.ToString() == "true")
                {
                    cCampo = resultado.Data[0].JSON.ToString();
                }

                
            }

            // Devolvemos la variable con el valor obtenido
            return cCampo;
        }


        



        public string leetexto(string cRuta)
        {
            
            StreamReader objReader;
            string sLine = "";
            string Archivo = "";

            try
            {
                objReader = new StreamReader(cRuta);

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        Archivo += sLine;
                }
                objReader.Close();

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
                
                                
            }

            return Archivo.Trim();


        }



        public static string Left(string param, int length)
        {
            string result = param.Substring(0, length);
            return result;
        }
        


       public static string Right(string param, int length)
        {
            int value = param.Length - length;
            string result = param.Substring(value, length);
            return result;
        }

        
        #region Funciones Variadas
        /// <summary>
        /// Aplica a una cadena un formato numerico con los decimales especificados
        /// </summary>
        /// <param name="cadena">Valor al cual se le aplicara el formato</param>
        /// <param name="decimales">Numero de digitos decimales para el formato</param>
        /// <param name="blanco">Verdadero si se desea devolver un formato vacio cuando el valor enviado es 0</param>
        /// <returns></returns>
        /// 
        // Varible que contendra las especificaciones para el formato de numeros
        NumberFormatInfo digitosDecimales = new CultureInfo("en-US", false).NumberFormat;
        public string FormatoDecimal(string cadena, int decimales, bool blanco)
        {
            // Verificamos si la cadena de texto esta en blanco
            if (cadena.Trim() == "")
            {
                // Verificamos si la opcion de blanco esta en verdadero
                if (blanco == true)
                {
                    // Devolvemos un valor nulo
                    return "";
                }
                else
                {
                    // Cambiamos el valor de la cadena de texto
                    cadena = "0.00";
                }
            }

            // Especificamos los digitos decimales se utizaran
            digitosDecimales.NumberDecimalDigits = decimales;

            try
            {
                // Asignamos el texto a una variable numerica que maneja decimales
                double numero = Convert.ToDouble(cadena);

                // Convertimos la variable numerica a texto con los decimales especificados
                cadena = numero.ToString("N", digitosDecimales);
            }
            catch (System.FormatException e)
            {
                // Mostramos el mensaje de error
                return e.Message;

                // Cambiamos el valor de la cadena
                
            }

            // Devolvemos el valor generado
            return cadena;
        }

        /// <summary>
        /// Aplica a una cadena un formato numerico
        /// </summary>
        /// <param name="cadena">Valor al cual se le aplicara el formato</param>
        /// <param name="blanco">Verdadero si se desea devolver un formato vacio cuando el valor enviado es 0</param>
        /// <returns></returns>
        public string FormatoNumerico(string cadena, bool blanco)
        {
            // Verificamos si la cadena de texto esta en blanco
            if (cadena.Trim() == "")
            {
                // Verificamos si la opcion de blanco esta en verdadero
                if (blanco == true)
                {
                    // Devolvemos un valor nulo
                    return "";
                }
                else
                {
                    // Cambiamos el valor de la cadena de texto
                    cadena = "0.00";
                }
            }

            try
            {
                // Asignamos el texto a una variable numerica que maneja decimales
                double numero = Convert.ToDouble(cadena);

                // Convertimos la variable numerica a texto
                cadena = numero.ToString();
            }
            catch (System.FormatException e)
            {
                // Mostramos el mensaje de error
                return e.Message;

                
            }

            // Devolvemos el valor generado
            return cadena;
        }

        /// <summary>
        /// Aplica a una cadena un formato numerico de un decimal sin separadores de miles 
        /// </summary>
        /// <param name="cadena">Cadena de caracteres a los cuales se les aplicara el formato</param>
        /// <param name="longitud">Longitud de la cadena que se devolvera</param>
        /// <returns></returns>
        public string FormatoDocumento(string cadena, int longitud)
        {
            // Verificamos si la cadena de texto esta en blanco
            if (cadena.Trim() == "")
            {
                // Cambiamos el valor de la cadena de texto
                cadena = "0.0";
            }

            // Declaramos una variable para convertir la cadena a numero
            double numero = Convert.ToDouble(cadena);

            // Verificamos si la longitud ingresada es mayor de 3
            if (longitud > 3)
            {
                // Declaramos una variable para el formato
                string formato = "0.0";

                // Ejecutamos un ciclo para crear el formato especifico
                for (int i = 3; i < longitud; i++)
                {
                    formato = "#" + formato;
                }

                // Devolvemos el numero con el formato aplicado
                return numero.ToString(formato);
            }

            // Devolvemos el formato estandar
            return numero.ToString("0.0");
        }

        /// <summary>
        /// Aplica a una cadena un formato numerico de un decimal sin separadores de miles 
        /// </summary>
        /// <param name="cadena">Cadena de caracteres a los cuales se les aplicara el formato</param>
        /// <param name="longitud">Longitud de la cadena que se devolvera</param>
        /// <param name="blanco">Verdadero si se desea devolver un formato vacio cuando el valor enviado es 0</param>
        /// <returns></returns>
        public string FormatoDocumento(string cadena, int longitud, bool blanco)
        {
            // Verificamos si la cadena de texto esta en blanco
            if (cadena.Trim() == "")
            {
                // Verificamos si la opcion de blanco esta en verdadero
                if (blanco == true)
                {
                    // Devolvemos un valor nulo
                    return "";
                }
                else
                {
                    // Cambiamos el valor de la cadena de texto
                    cadena = "0.0";
                }
            }

            // Declaramos una variable para convertir la cadena a numero
            double numero = Convert.ToDouble(cadena);

            // Verificamos si la longitud ingresada es mayor de 3
            if (longitud > 3)
            {
                // Declaramos una variable para el formato
                string formato = "0.0";

                // Ejecutamos un ciclo para crear el formato especifico
                for (int i = 3; i < longitud; i++)
                {
                    formato = "#" + formato;
                }

                // Devolvemos el numero con el formato aplicado
                return numero.ToString(formato);
            }

            // Devolvemos el formato estandar
            return numero.ToString("0.0");
        }

        /// <summary>
        /// Funcion para validar si el caracter a evaluar es un digito o un punto
        /// </summary>
        /// <param name="Digito">Caracter que se verificara</param>
        /// <param name="ValidarPunto">Verdadero si se desea validar el punto</param>
        /// <returns>Verdadero si es un digito valido</returns>
        public static bool ValidarDigito(char Digito, bool ValidarPunto)
        {
            // Creamos una variable para almacenar el resultado de la validacion
            bool DigitoValidado = false;

            // Verificamos si la tecla pulsada es valida
            switch (Digito)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '\b':
                    {
                        // Cambiamos el valor de la variable de validacion
                        DigitoValidado = true;

                        // Cortamos la definicion CASE
                        break;
                    }
                default:
                    {
                        // Verificamos si se valida el punto
                        if (ValidarPunto == true && Digito == '.')
                        {
                            // Cambiamos el valor de la variable de validacion
                            DigitoValidado = true;
                        }

                        // Cortamos la definicion CASE
                        break;
                    }
            }

            // Devolvemos el valor de la variable de validacion
            return DigitoValidado;
        }

        // Agrega a la izquierda de la cadena el caracter especificado hasta alcanzar la longitud especifica
        public string CaracteresIzquierda(string cadena, int longitud, char relleno)
        {
            // Agregamos el caracter especificado a la cadena
            string texto = cadena.PadLeft(longitud, relleno);

            // Devolvemos el valor generado
            return texto;
        }

        /// <summary>
        /// Obtiene el numero de registros de una tabla
        /// </summary>
        /// <param name="Tabla">Nombre de la tabla sobre cual se realizara la busqueda</param>
        /// <param name="Campo">Nombre del campo que se desea contar</param>
        /// <returns>Entero con la cantidad de registros encontrados</returns>
        public int NumerodeRegistros(string Tabla, string Campo)
        {
            // Declaramos una variable para guardar el numero total de registros
            int NumerodeRegistros = 0;

            // Verificamos que los dos parametros tengan valores
            if (Tabla != "" && Campo != "")
            {
                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "";
                instruccionSQL += string.Format("SELECT count({0}) ", Campo);
                instruccionSQL += string.Format("FROM {0} ", Tabla);

                // Ejecutamos la instruccion
                int NumeroError = 0;// mysql.EjecutarLectura(instruccionSQL, BaseDeDatos);

                // Verificamos si la consulta obtuvo resultados
                if (NumeroError == 0 && mysql.consulta != null)
                {
                    // Recorremos la consulta
                    while (mysql.consulta.Read())
                    {
                        // Asignamos el registro a la variable global
                        NumerodeRegistros = mysql.consulta.GetInt32(0);
                    }
                    // Cerramos la consulta
                    mysql.consulta.Close();
                }
            }

            // Devolvemos la variable con el valor obtenido
            return NumerodeRegistros;
        }

        /// <summary>
        /// Obtiene el numero de registros de una tabla correspondientes a una condicion especifica
        /// </summary>
        /// <param name="Tabla">Nombre de la tabla sobre cual se realizara la busqueda</param>
        /// <param name="Campo">Nombre del campo que se desea contar</param>
        /// <param name="Condicion">Filtro que se aplicara en el conteo</param>
        /// <returns>Entero con la cantidad de registros encontrados</returns>
        public int NumerodeRegistros(string Tabla, string Campo, string Condicion, string BaseDeDatos)
        {
            // Declaramos una variable para guardar el numero total de registros
            int NumerodeRegistros = 0;

            // Verificamos que los dos parametros tengan valores
            if (Tabla != "" && Campo != "")
            {
                // Creamos la instruccion con los parametros obtenidos
                string instruccionSQL = "";
                instruccionSQL += string.Format("SELECT count({0}) ", Campo);
                instruccionSQL += string.Format("FROM {0} ", Tabla);
                instruccionSQL += string.Format("WHERE {0} ", Condicion);

                // Ejecutamos la instruccion
                mysql.EjecutarLectura(instruccionSQL, BaseDeDatos);

                // Verificamos si la consulta obtuvo resultados
                if (mysql.consulta != null)
                {
                    // Recorremos los registros encontrados
                    while (mysql.consulta.Read())
                    {
                        // Asignamos el registro a la variable global
                        NumerodeRegistros = mysql.consulta.GetInt32(0);
                    }

                    // Cerramos la consulta
                    mysql.consulta.Close();
                }
            }

            // Devolvemos la variable con el valor obtenido
            return NumerodeRegistros;
        }


             


        // Funcion para devolver el nombre del mes que se solicite
        public string NombreMes(int NumeroMes)
        {
            // Declaramos la variable para almacenar el nombre del mes
            string NombreMes = "";

            // Verificamos el numero del mes
            switch (NumeroMes)
            {
                case 1:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Enero";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 2:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Febrero";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 3:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Marzo";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 4:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Abril";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 5:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Mayo";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 6:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Junio";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 7:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Julio";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 8:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Agosto";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 9:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Septiembre";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 10:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Octubre";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 11:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Noviembre";

                        // Cortamos la definicion CASE
                        break;
                    }
                case 12:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "Diciembre";

                        // Cortamos la definicion CASE
                        break;
                    }
                default:
                    {
                        // Devolvemos el nombre del mes
                        NombreMes = "N/A";

                        // Cortamos la definicion CASE
                        break;
                    }
            }

            // Devolvemos el mes generado
            return NombreMes;
        }

        /// <summary>
        /// Verifica si la opcion a buscar esta activa
        /// </summary>
        /// <param name="TablaPermisos">Tabla en donde se buscara el codigo de permiso</param>
        /// <param name="CodigoPermiso">Codigo de permisos a buscar dentro del objeto DataTable</param>
        /// <returns>True si el codigo de permiso esta activo, False si no lo esta</returns>
        public static bool VerificarPermiso(DataTable TablaPermisos, object CodigoPermiso)
        {
            // Declaramos una variable para almacenar el filtro para consultar los permisos
            string Condicion = string.Format("cmodulo = '{0}' ", CodigoPermiso);

            // Buscamos el registro segun el filtro creado
            DataRow[] RegistroTabla = TablaPermisos.Select(Condicion);

            // Verificamos si existen registros con la condicion indicada
            if (RegistroTabla.Length > 0)
            {
                // Verificamos si el codigo de permiso es 0
                if (Convert.ToInt32(RegistroTabla[0][1]) == 0)
                {
                    // Devolvemos un valor falso
                    return false;
                }
                else
                {
                    // Devolvemos un valor verdadero
                    return true;
                }
            }
            else
            {
                // Devolvemos un valor falso
                return false;
            }
        }

              
        // Procedimiento para averigurar el ultimo dia del mes
        public DateTime UltimoDiaMes(int NumeroMes, int NumeroAno)
        {
            // Creamos una fecha con el dia 28 mas el mes y año a verificar
            DateTime UltimoDia = Convert.ToDateTime("28/" + NumeroMes.ToString().PadLeft(2, '0') + "/" + NumeroAno.ToString("0000"));

            // Creamos un ciclo para recorrer los dias hasta que cambie de mes
            while (UltimoDia.Month == NumeroMes)
            {
                // Incrementamos el numero de dia
                UltimoDia = UltimoDia.AddDays(1);
            }

            // Restamos un dia a la variable de control
            UltimoDia = UltimoDia.AddDays(-1);

            // Devolvemos el valor generado
            return UltimoDia;
        }
        #endregion

        
        

       
        
    }
}