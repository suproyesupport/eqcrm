using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace EqCrm.Controllers
{
    public class IngDoctosController : Controller
    {
        // GET: LineasConsultas
        public ActionResult IngDoctos()
        {
            string cUserConected = (string)(Session["Usuario"]);
            Funciones f = new Funciones();
            string drDesc = "";
            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }

            string Base_Datos = (string)(Session["StringConexion"]);
            StringConexionMySQL mysql = new StringConexionMySQL();
            /// Contacto Vendedores
            List<SelectListItem> listadoasesores = new List<SelectListItem>();
            string cInst = "SELECT id_codigo,nombre FROM vendedores";
            mysql.LLenarDropDownList(cInst, Base_Datos, listadoasesores);
            ViewData["asesores"] = listadoasesores;

            string oBase = (string)(Session["oBase"]);
            

            /// Tipo de Documentos
            //if (f.DamePermiso("9", cUserConected,Base_Datos) == true)
            //{
            //    List<SelectListItem> listadotipodoctos = new List<SelectListItem>();
            //    cInst = "SELECT id,descripcion FROM cattipodoctos";
            //    mysql.LLenarDropDownList(cInst, Base_Datos, listadotipodoctos);
            //    ViewData["tipodocto"] = listadotipodoctos;
            //}
            //else
            //{
            List<SelectListItem> listadotipodoctos = new List<SelectListItem>();
                cInst = "SELECT id,descripcion FROM cattipodoctos WHERE id='COTIZ'";
                mysql.LLenarDropDownList(cInst, Base_Datos, listadotipodoctos);
                ViewData["tipodocto"] = listadotipodoctos;
            //}


            DataTable dt = new DataTable();
            DataTable dtInventario = new DataTable();
            DataTable dtCotiz = new DataTable();

            cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT,diascred as DIASCREDITO,atencion as CONTACTO FROM clientes ";
            ViewBag.Tabla = mysql.LlenarDTTableHTmlCliente(cInst, dt, Base_Datos);

            cInst = "SELECT id_codigo AS CODIGO, codigoe AS CODIGOE, producto AS PRODUCTO, precio1 AS PRECIO FROM inventario WHERE status != 'B' ";
            ViewBag.TablaInventario = mysql.LlenarDTTableHTmlInventario(cInst, dtInventario, Base_Datos);



            string nLineasDescto = f.ObtieneDatos("parametros", "nfdescto", "", oBase);
            drDesc = "<select class=\"form-control\" id=\"descto\" name=\"descto\">";
            drDesc += "<option value = \"\" > Selecccionar Descuento...</option>";
            double nLineasDes = Convert.ToDouble(nLineasDescto);
            int lineasdescto = Convert.ToInt32(nLineasDes);

            for (int i = 0; i <= lineasdescto; i++)
            {
                drDesc += "<option value = " + '"' + i.ToString() + '"' + ">" + i.ToString() + "</option>";
            }

            drDesc += " </select>";



            ViewBag.Desctos = drDesc.ToString();

            //nLineasDescto = f.ObtieneDatos("parametros", "nfdescto", "", Base_Datos);
            drDesc = "<select class=\"form-control\" id=\"desctotable\" name=\"desctotable\">";
            drDesc += "<option value = \"\" > Selecccionar Descuento...</option>";
            //nLineasDes = Convert.ToDouble(nLineasDescto);
            //lineasdescto = Convert.ToInt32(nLineasDes);


            for (int i = 0; i <= nLineasDes; i++)
            {
                drDesc += "<option value = " + '"' + i.ToString() + '"' + ">" + i.ToString() + "</option>";
            }

            drDesc += " </select>";

            ViewBag.DesctosTable = drDesc.ToString();



            // Vamos a buscar solo las cotizaciones WEB
            cInst = "Select p.NO_FACTURA AS NO_COTI, p.SERIE, p.FECHA, p.CLIENTE, p.ID_CLIENTE, v.NOMBRE, p.DIRECCION, FORMAT(p.TOTAL,2) AS TOTAL from proformas p inner join vendedores v on p.id_vendedor = v.id_codigo";
            cInst += " WHERE p.SERIE ='CW' ";
            ViewBag.TablaCotizaciones = mysql.LlenarDTTableHTmlCotiz(cInst, dtCotiz, Base_Datos);


            // Cerrar conexion
            mysql.CerrarConexion();
            return View();
        }

        [HttpPost]
        public string GetDataCliente(DatosProspecto datos)
        {
            
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]); //(string)this.Session["StringConexion"];

            string sentenciaSQL1 = "SELECT json_object('CLIENTE', cliente," + "'DIRECCION', direccion," + "'NIT', nit," + "'DIASCRED',diascred,"+"'CONTACTO',atencion"+") FROM clientes " + " WHERE id_codigo = " + datos.id;
            
            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = stringConexionMySql.consulta.GetString(0).ToString();
            }
            else
            {
                str = "{\"NIT\": \"ERROR\", \"CLIENTE\": \"\", \"DIASCRED\": 0, \"DIRECCION\": \"\"}";
            }

           
            stringConexionMySql.CerrarConexion();
            return str;
        }


        

        [HttpPost]
        public string GetTasa()
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)(Session["StringConexion"]); //(string)this.Session["StringConexion"];
            Funciones f = new Funciones();

            string sentenciaSQL1 = "SELECT json_object('DESCTO', nfdescto,'NTASA', nTasaWeb," + "'HABPRECIO', (SELECT ingresa FROM permisossistema WHERE cmodulo = '2037')"+") FROM parametros ";

            stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = stringConexionMySql.consulta.GetString(0).ToString();
               
            }
            else
            {
                str = "{\"NTASA\": \"ERROR\"}";
            }


            stringConexionMySql.CerrarConexion();
            return str;
        }


        [HttpPost]
        public string GetDataInventario(DatosProspecto datos)
        {

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string str = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string sentenciaSQL1 = "SELECT json_object('CODIGO', id_codigo," + "'PRODUCTO', producto," + "'PRECIO', precio1," + "'CODIGOE', codigoe,"+"'DOLAR',dolares"+" ) FROM inventario " + " WHERE id_codigo = " + datos.id;

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    str = stringConexionMySql.consulta.GetString(0).ToString();
                }
                else
                {
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"\", \"PRECIO\": 0}";
                }
            }
            catch(Exception ex)
            {
                str= "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \""+ex.Message.ToString()+"\", \"PRECIO\": 0}";
            }



            stringConexionMySql.CerrarConexion();
            return str;
        }


        [HttpPost]
        public string BuscaDetalleCotizacion(string nodocto,string serie)
        {
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            DataTable dt = new DataTable();

            string no_factura = nodocto.ToString();
            string cserie = serie.ToString();

            //string queryVerCotizacion = "select p.no_factura, p.serie, p.fecha, p.cliente, p.nit, v.nombre as vendedor, p.direccion,if(p.tasa>=1,(p.total)/(p.tasa),p.total) as total, p.obs, i.producto, dp.cantidad, dp.precio, dp.tdescto, dp.subtotal, if(dp.url = '', i.foto, dp.url) as url , cl.telefono, cl.email,  v.nombre as nombre_vendedor, v.telefono as telefono_vendedor, v.email as correo_vendedor,p.tasa as tasa";
            string queryVerCotizacion = "select p.no_factura, p.serie, p.fecha, p.cliente, p.nit, v.nombre as vendedor, p.direccion,FORMAT(p.total,2) as total, p.obs, dp.obs as producto, dp.cantidad, FORMAT(dp.precio,2)as precio, FORMAT(dp.tdescto,2) as tdescto, FORMAT(dp.subtotal,2) as subtotal, if(dp.url = '', i.foto, dp.url) as url , cl.telefono, cl.email,  v.nombre as nombre_vendedor, v.telefono as telefono_vendedor, v.email as correo_vendedor,p.tasa as tasa,i.id_codigo,i.codigoe,dp.descto";
            queryVerCotizacion += " from proformas p inner join detproformas dp on (p.no_factura = dp.no_factura and p.serie = dp.serie)";
            queryVerCotizacion += " inner join vendedores v on v.id_codigo = p.id_vendedor";
            queryVerCotizacion += " inner join inventario i on i.id_codigo = dp.id_codigo";
            queryVerCotizacion += " inner join clientes cl on cl.id_codigo = p.id_cliente";
            queryVerCotizacion += " where p.no_factura = " + no_factura.ToString();
            queryVerCotizacion += " and p.serie = '" + cserie.ToString() + "' ORDER BY no_cor ASC";

            stringConexionMySql.LlenarTabla(queryVerCotizacion, dt, DB);


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
        public string InsertarDocumento(DatosProspecto datos)
        {
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
            string cFrases = (string)this.Session["cCampo1"];

            int nDocumento = 0;
            bool lError = false;
            string cError = "";
            int nContador = 1;            
            double nCosto = 0.00;
            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();

            string cId_Interno = "";

            int nLinea = 0;
            string cServicio = "";
            double nPrecioSD = 0.00;
            double nMontoGravable = 0.00;
            double nMontoImpuesto = 0.00;
            double nMontoTotalImpuesto = 0.00;
            double tasa = 0.00;
            double nPrecio = 0.00;
            double nSubtotal = 0.00;

            DateTime tiempo1 = new DateTime();
            DateTime tiempo2 = new DateTime();
            string difTiempo = "";

            string cDtalle = datos.cdetalle.Replace("~","<").Replace("}",">").Replace("|","/");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cDtalle);


            XmlNodeList cXmlDetalle = xmlDoc.GetElementsByTagName("DETALLE");

            /// vamos a sumar el total
            

            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");
            XmlNodeList contacto = xmlDoc.GetElementsByTagName("CONTACTO");
            XmlNodeList endolar = xmlDoc.GetElementsByTagName("ENDOLAR");
            XmlNodeList ntasa = xmlDoc.GetElementsByTagName("TASA");
            if (endolar[0].InnerText == "N")
            {
                tasa = 0.00;
            }
            else
            {
                tasa = Convert.ToDouble(ntasa[0].InnerText);
            }


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

            /// aca vamos hacer que ingrese una factura normal comun y corriente
            #region

            if (tipodocto[0].InnerText == "FACTU")
            {
                cInst  = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
                cInst += String.Format("FROM {0} ", "facturas");
                cInst += "where id_agencia = 1 and serie='FELWEB'";
                

                stringConexionMySql.EjecutarLectura(cInst, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                }
                // Cerramos la consulta
                stringConexionMySql.CerrarConexion();

                             
                stringConexionMySql.ExecAnotherCommand("BEGIN;",DB);               


                //Insertaremos el encabezado de la factura

                cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}') ",
                    nDocumento.ToString(),
                    "FELWEB",
                    fechadocto[0].InnerText,                     
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText, 
                    direccion[0].InnerText,
                    nTotal.ToString(), 
                    obs[0].InnerText,
                    1, 
                    nit[0].InnerText,
                    fechadocto[0].InnerText, 
                   dias[0].InnerText.ToString(),nDescto.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }

                // vamos a insertar y asumir de momento que es al credito.
                cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                               id_cliente[0].InnerText, nDocumento.ToString(), "FELWEB", 1, fechadocto[0].InnerText, fechadocto[0].InnerText, dias[0].InnerText.ToString(), fechadocto[0].InnerText,
                                                nTotal.ToString(), nTotal.ToString(), "Operado en Modulo Web", "Web", "FACTURA", 1);

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }



                /// vamos a insertar el detalle de la factura y kardex y denas
                /// 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;
                   
                    cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), "FELWEB", codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",",""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1);

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }


                /// vamos a insertar el detalle del kardex
                /// 
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

                    cInst = "INSERT INTO kardexinven ";
                    cInst += "( id_codigo, id_agencia, fecha, id_movi, docto, serie, obs, hechopor, salida, costo1,precio, codigoemp,correlativo ) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "51", nDocumento.ToString(), "FELWEB", "Ingresado en modulo sistema Web", "Web", cantidad.ToString().Replace(",",""), nCosto.ToString(), precio.ToString().Replace(",",""), id_cliente[0].InnerText, nContador.ToString());

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }

               

                datosEmisor.Tipo = f.ObtieneDatos("resolucionessat", "tipoactivo", "serie = 'FELWEB'", _BaseDatos);
                datosEmisor.FechaHoraEmision = fechadocto[0].InnerText + "T00:00:00";
                datosEmisor.CodigoMoneda = "GTQ";
                datosEmisor.NitEmisor = cNit;
                datosEmisor.NombreEmisor = cEmisor;
                datosEmisor.NombreComercial = cComercial;
                datosEmisor.AfiliacionIva = cAfiliacion;
                datosEmisor.CorreoEmisor = cEmail;
                datosEmisor.Direccion = cDireccion;
                datosEmisor.CodigoEstablecimiento = f.ObtieneDatos("resolucionessat", "codigoestablecimiento", "serie = 'FELWEB'", _BaseDatos);
                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Municipio = "GUATEMALA";
                datosEmisor.Departamento = "GUATEMALA";
                datosEmisor.Pais = "GT";
                datosEmisor.CodigoPostal = "01011";
                datosEmisor.Municipio = "GUATEMALA";
                datosEmisor.Departamento = "GUATEMALA";
                datosEmisor.Pais = "GT";

                cId_Interno = "FELWEB-" + nDocumento.ToString();




                datosReceptor.IdReceptor = nit[0].InnerText.ToString().Replace("-", "");
                datosReceptor.NombreReceptor = cliente[0].InnerText;
                datosReceptor.CorreoReceptor = "";
                datosReceptor.Direccion = direccion[0].InnerText;
                datosReceptor.CodigoPostal = "01011";
                datosReceptor.Municipio = "GUATEMALA";
                datosReceptor.Departamento = "GUATEMALA";
                datosReceptor.Pais = "GT";

                frases[0].CodigoEscenario = "1";
                frases[0].TipoFrase = "1";

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
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                    cServicio = f.ObtieneDatos("inventario", "servicio", "id_codigo="+ codigo, _BaseDatos);
                    if (cServicio.Trim() == "N")
                    {

                        items[nLinea].BienOServicio = "B";
                    }
                    else
                    {
                        items[nLinea].BienOServicio = "S";
                    }

                    items[nLinea].NumeroLinea = nContador.ToString();
                    items[nLinea].Cantidad = cantidad.ToString().Replace(",", "");
                    items[nLinea].UnidadMedida = "UNI";
                    items[nLinea].Descripcion = producto;
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

                FelEstructura.Adenda adenda = new FelEstructura.Adenda();
                FelEstructura.ComplementoFacturaCambiaria cfc = new FelEstructura.ComplementoFacturaCambiaria();

                nMontoTotalImpuesto = Convert.ToDouble(nTotal.ToString()) / 1.12;
                nMontoTotalImpuesto = nMontoTotalImpuesto * 0.12;
                totales.NombreCorto = "IVA";
                totales.TotalMontoImpuesto = f.FormatoDecimal(nMontoTotalImpuesto.ToString(), 6, false).Replace(",", "");
                totales.GranTotal = f.FormatoDecimal(nTotal.ToString(), 6, false).Replace(",", "");


                FelEstructura.ComplementoNotadeCredito cnc = new FelEstructura.ComplementoNotadeCredito();

                FelEstructura.ComplementoFacturaEspecial cfe = new FelEstructura.ComplementoFacturaEspecial();
                FelEstructura.ComplementoFacturaExportacion cex = new FelEstructura.ComplementoFacturaExportacion();
                var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, nTotal.ToString(), "", adenda, cfc,"G4S",cnc,cfe,cex);

                Dte = Dte.Replace("{CONT}", "");
                               
                var wsEnvio = wsConnector.wsEnvio("POST_DOCUMENT_SAT", Funciones.Base64Encode(Dte), cId_Interno,cUserFe ,cUrlFel,cToken , "SYSTEM_REQUEST", "GT", cNit, false, "");



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
                    cUpdate += " WHERE no_factura = " + nDocumento.ToString();
                    cUpdate += " AND serie ='FELWEB'";
                    cUpdate += " AND id_agencia = 1";

                    stringConexionMySql.ExecAnotherCommand(cUpdate, DB);
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
                    cUpdate += " AND serie ='FELWEB'";
                    cUpdate += " AND id_agencia = 1";

                    stringConexionMySql.ExecAnotherCommand(cUpdate, DB);
                    //stringConexionMySql.CerrarConexion();

                }
                else
                {

                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    cError = wsEnvio[2].ToString();
                    return cError;
                }
                               

                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);                
                stringConexionMySql.CerrarConexion();

                stringConexionMySql.CerrarConexion();

                cMensaje = "DOCUMENTO CREADO CON EXITO";
            }

            #endregion

            /// aca vamos hacer que ingrese un pedido
            #region
            if (tipodocto[0].InnerText == "PEDI")
            {
                cInst = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
                cInst += String.Format("FROM {0} ", "pedidos");
                cInst += "where id_agencia = 1 and serie='PW'";


                stringConexionMySql.EjecutarLectura(cInst, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                }
                // Cerramos la consulta
                stringConexionMySql.CerrarConexion();


                stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);


                //Insertaremos el encabezado de la factura

                cInst = "INSERT INTO pedidos (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}') ",
                    nDocumento.ToString(),
                    "PW",
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText,
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    1,
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString(), nDescto.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }

               

                /// vamos a insertar el detalle de la factura y kardex y denas
                /// 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                    cInst = " INSERT INTO detpedidos (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), "PW", codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1);

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }


                


                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                stringConexionMySql.CerrarConexion();

                stringConexionMySql.CerrarConexion();

                cMensaje = "DOCUMENTO CREADO CON EXITO";
            }
            #endregion


            if (tipodocto[0].InnerText == "COTIZ")
            {
                cInst = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
                cInst += String.Format("FROM {0} ", "proformas");
                cInst += "where id_agencia = 1 and serie='CW'";


                stringConexionMySql.EjecutarLectura(cInst, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                }
                // Cerramos la consulta
                stringConexionMySql.CerrarConexion();


                stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);


                //Insertaremos el encabezado de la factura

                cInst = "INSERT INTO proformas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,atencion,tasa,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}') ",
                    nDocumento.ToString(),
                    "CW",
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText,
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    1,
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString(),
                   contacto[0].InnerText.ToString(),
                   tasa.ToString(),
                   nDescto.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }



                /// vamos a insertar el detalle de la factura y kardex y denas
                /// 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    if (tasa >= 1)
                    {
                        nPrecio = Convert.ToDouble(node.ChildNodes[4].InnerText) * tasa;
                        precio = nPrecio.ToString();
                    }
                    else
                    {
                        precio = node.ChildNodes[4].InnerText;
                    }

                    descto = node.ChildNodes[5].InnerText;
                    path = node.ChildNodes[7].InnerText;
                    if (string.IsNullOrEmpty(descto))
                    {
                        descto = "0";
                    }

                    if (tasa >= 1)
                    {
                        nSubtotal = Convert.ToDouble(node.ChildNodes[6].InnerText) * tasa;
                        subtotal = nSubtotal.ToString();
                    }
                    else
                    {
                        subtotal = node.ChildNodes[6].InnerText;
                    }

                    if (path.Contains("Images"))
                    {
                        path = node.ChildNodes[7].InnerText;
                    }
                    else
                    {
                        path = "";
                    }



                    cInst = " INSERT INTO detproformas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,descto,obs,id_agencia, url) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ", nDocumento.ToString(), "CW", codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1, path.ToString());


                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }


                if (tasa>=1)
                { 
                    cInst = "UPDATE proformas SET tasa = " + tasa.ToString() + ",";
                    cInst += " total = total * " + tasa.ToString() + ",";
                    cInst += " tdescto = tdescto * " + tasa.ToString();
                    cInst += " WHERE no_factura=" + nDocumento.ToString();
                    cInst += " AND serie='CW'";
 
                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }

                    cInst = "UPDATE detfacturas SET ";
                    cInst += " precio = precio * " + tasa.ToString() + ",";
                    cInst += " subtotal = subtotal * " + tasa.ToString() + ",";
                    cInst += " tdescto = tdescto * " + tasa.ToString();
                    cInst += " WHERE no_factura=" + nDocumento.ToString();
                    cInst += " AND serie='CW'";

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }

                    


                }

                cInst = "UPDATE detproformas SET ";
                cInst += " tdescto = (precio * cantidad)-subtotal";
                cInst += " WHERE no_factura=" + nDocumento.ToString();
                cInst += " AND serie='CW'";

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                if (lError == true)
                {

                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cError;

                }




                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                stringConexionMySql.CerrarConexion();

                cMensaje = "COTIZACION CREADA CON EXITO CON CODIGO: " + nDocumento.ToString()+"|"+"CW";
            }
          
            if (tipodocto[0].InnerText == "ENVIO")
            {
                cInst = String.Format("SELECT IFNULL(max({0}), 0) ", "no_factura");
                cInst += String.Format("FROM {0} ", "envios");
                cInst += "where id_agencia = 1 and serie='EW'";


                stringConexionMySql.EjecutarLectura(cInst, DB);
                if (stringConexionMySql.consulta.Read())
                {
                    nDocumento = stringConexionMySql.consulta.GetInt32(0) + 1;
                }
                // Cerramos la consulta
                stringConexionMySql.CerrarConexion();


                stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);


                //Insertaremos el encabezado de la factura

                cInst = "INSERT INTO envios (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}') ",
                    nDocumento.ToString(),
                    "EW",
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText,
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    1,
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString(), nDescto.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }

                // vamos a insertar y asumir de momento que es al credito.
                cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                               id_cliente[0].InnerText, nDocumento.ToString(), "EW", 7, fechadocto[0].InnerText, fechadocto[0].InnerText, dias[0].InnerText.ToString(), fechadocto[0].InnerText,
                                                nTotal.ToString(), nTotal.ToString(), "Operado en Modulo Web", "Web", "ENVIO", 1);

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }



                /// vamos a insertar el detalle de la factura y kardex y denas
                /// 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    precio = node.ChildNodes[4].InnerText;
                    descto = node.ChildNodes[5].InnerText;
                    subtotal = node.ChildNodes[6].InnerText;

                    cInst = " INSERT INTO detalle_envios (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ", nDocumento.ToString(), "EW", codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1);

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }


                /// vamos a insertar el detalle del kardex
                /// 
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

                    cInst = "INSERT INTO kardexinven ";
                    cInst += "( id_codigo, id_agencia, fecha, id_movi, docto, serie, obs, hechopor, salida, costo1,precio, codigoemp,correlativo ) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", codigo.ToString(), "1", fechadocto[0].InnerText, "55", nDocumento.ToString(), "EW", "Ingresado en modulo sistema Web", "Web", cantidad.ToString().Replace(",", ""), nCosto.ToString(), precio.ToString().Replace(",", ""), id_cliente[0].InnerText, nContador.ToString());

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }

                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                stringConexionMySql.CerrarConexion();

                stringConexionMySql.CerrarConexion();

                cMensaje = "DOCUMENTO CREADO CON EXITO";
            }
            return cMensaje;
        }


        [HttpPost]
        public string ModificarDocumento(DatosProspecto datos)
        {
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

            int nDocumento = 0;
            bool lError = false;
            string cError = "";
            int nContador = 1;
            double nCosto = 0.00;
            FelEstructura.DatosEmisor datosEmisor = new FelEstructura.DatosEmisor();
            FelEstructura.DatosReceptor datosReceptor = new FelEstructura.DatosReceptor();
            FelEstructura.DatosFrases[] frases = new FelEstructura.DatosFrases[1];
            FelEstructura.Totales totales = new FelEstructura.Totales();

            string cId_Interno = "";

            int nLinea = 0;
            string cServicio = "";
            double nPrecioSD = 0.00;
            double nMontoGravable = 0.00;
            double nMontoImpuesto = 0.00;
            double nMontoTotalImpuesto = 0.00;
            double tasa = 0.00;
            double nPrecio = 0.00;
            double nSubtotal = 0.00;

            DateTime tiempo1 = new DateTime();
            DateTime tiempo2 = new DateTime();
            string difTiempo = "";

            string cDtalle = datos.cdetalle.Replace("~", "<").Replace("}", ">").Replace("|", "/");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(cDtalle);


            XmlNodeList cXmlDetalle = xmlDoc.GetElementsByTagName("DETALLE");

            /// vamos a sumar el total


            XmlNodeList tipodocto = xmlDoc.GetElementsByTagName("TIPODOCTO");
            XmlNodeList ndocto = xmlDoc.GetElementsByTagName("DOCTO");
            XmlNodeList fechadocto = xmlDoc.GetElementsByTagName("FECHADOCTO");
            XmlNodeList id_cliente = xmlDoc.GetElementsByTagName("CODIGOCLIENTE");
            XmlNodeList cliente = xmlDoc.GetElementsByTagName("NOMBRECLIENTE");
            XmlNodeList direccion = xmlDoc.GetElementsByTagName("DIRECCIONCLIENTE");
            XmlNodeList nit = xmlDoc.GetElementsByTagName("NITCLIENTE");
            XmlNodeList vendedor = xmlDoc.GetElementsByTagName("VENDEDOR");
            XmlNodeList obs = xmlDoc.GetElementsByTagName("ENCOBS");
            XmlNodeList dias = xmlDoc.GetElementsByTagName("DIAS");
            XmlNodeList contacto = xmlDoc.GetElementsByTagName("CONTACTO");
            XmlNodeList endolar = xmlDoc.GetElementsByTagName("ENDOLAR");
            XmlNodeList ntasa = xmlDoc.GetElementsByTagName("TASA");
            if (endolar[0].InnerText == "N")
            {
                tasa = 0.00;
            }
            else
            {
                tasa = Convert.ToDouble(ntasa[0].InnerText);
            }


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

            

       


            if (tipodocto[0].InnerText == "COTIZ")
            {
                nDocumento = Convert.ToInt32(ndocto[0].InnerText);

                stringConexionMySql.ExecAnotherCommand("BEGIN;", DB);

                cInst = "DELETE FROM proformas ";
                cInst += " WHERE no_factura=" + nDocumento.ToString();
                cInst += " AND serie='CW'";


                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                if (lError == true)
                {
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cError;
                }
                //Insertaremos el encabezado de la factura

                cInst = "INSERT INTO proformas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,atencion,tasa,tdescto) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}','{16}') ",
                    nDocumento.ToString(),
                    "CW",
                    fechadocto[0].InnerText,
                    "I",
                    id_cliente[0].InnerText,
                    cliente[0].InnerText,
                    vendedor[0].InnerText,
                    direccion[0].InnerText,
                    nTotal.ToString(),
                    obs[0].InnerText,
                    1,
                    nit[0].InnerText,
                    fechadocto[0].InnerText,
                   dias[0].InnerText.ToString(),
                   contacto[0].InnerText.ToString(),
                   tasa.ToString(),
                   nDescto.ToString());

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cMensaje;

                }



                /// vamos a insertar el detalle de la factura y kardex y denas
                /// 
                foreach (XmlNode node in cXmlDetalle) // for each <testcase> node
                {

                    codigo = node.ChildNodes[0].InnerText;
                    codigoe = node.ChildNodes[1].InnerText;
                    producto = node.ChildNodes[3].InnerText;
                    cantidad = node.ChildNodes[2].InnerText;
                    if (tasa >= 1)
                    {
                        nPrecio = Convert.ToDouble(node.ChildNodes[4].InnerText) * tasa;
                        precio = nPrecio.ToString();
                    }
                    else
                    {
                        precio = node.ChildNodes[4].InnerText;
                    }

                    descto = node.ChildNodes[5].InnerText;
                    path = node.ChildNodes[7].InnerText;
                    if (string.IsNullOrEmpty(descto))
                    {
                        descto = "0";
                    }

                    if (tasa >= 1)
                    {
                        nSubtotal = Convert.ToDouble(node.ChildNodes[6].InnerText) * tasa;
                        subtotal = nSubtotal.ToString();
                    }
                    else
                    {
                        subtotal = node.ChildNodes[6].InnerText;
                    }

                    if (path.Contains("Images"))
                    {
                        path = node.ChildNodes[7].InnerText;
                    }
                    else
                    {
                        path = "";
                    }



                    cInst = " INSERT INTO detproformas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,descto,obs,id_agencia, url) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ", nDocumento.ToString(), "CW", codigo.ToString(), cantidad.ToString(), precio.ToString().Replace(",", ""), subtotal.ToString().Replace(",", ""), nContador.ToString(), descto.ToString().Replace(",", ""), producto.ToString(), 1, path.ToString());


                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }



                    nContador++;
                }


                if (tasa >= 1)
                {
                    cInst = "UPDATE proformas SET tasa = " + tasa.ToString() + ",";
                    cInst += " total = total * " + tasa.ToString() + ",";
                    cInst += " tdescto = tdescto * " + tasa.ToString();
                    cInst += " WHERE no_factura=" + nDocumento.ToString();
                    cInst += " AND serie='CW'";

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }

                    cInst = "UPDATE detfacturas SET ";
                    cInst += " precio = precio * " + tasa.ToString() + ",";
                    cInst += " subtotal = subtotal * " + tasa.ToString() + ",";
                    cInst += " tdescto = tdescto * " + tasa.ToString();
                    cInst += " WHERE no_factura=" + nDocumento.ToString();
                    cInst += " AND serie='CW'";

                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                    if (lError == true)
                    {

                        stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                        stringConexionMySql.CerrarConexion();
                        return cError;

                    }




                }

                cInst = "UPDATE detproformas SET ";
                cInst += " tdescto = (precio * cantidad)-subtotal";
                cInst += " WHERE no_factura=" + nDocumento.ToString();
                cInst += " AND serie='CW'";

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                if (lError == true)
                {

                    stringConexionMySql.ExecAnotherCommand("ROLLBACK", DB);
                    stringConexionMySql.CerrarConexion();
                    return cError;

                }




                stringConexionMySql.ExecAnotherCommand("COMMIT;", DB);
                stringConexionMySql.CerrarConexion();

                cMensaje = "COTIZACION CREADA CON EXITO CON CODIGO: " + nDocumento.ToString() + "|" + "CW";
            }

         
            return cMensaje;
        }


        [Route("/{id}")]
        [HttpPost]
        public JsonResult uploadFile(string id)
        {
            // check if the user selected a file to upload
            if (Request.Files.Count > 0)
            {
                try
                { 
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    string fileName = file.FileName;
                    string extension = Path.GetExtension(fileName);
                    string nombreArchivo = id;
                    // create the uploads folder if it doesn't exist
                    if (!Directory.Exists(Server.MapPath("~/Images/IngDoctos/" + nombreArchivo + extension)))
                    { 
                        string path = Path.Combine(Server.MapPath("~/Images/IngDoctos/"), nombreArchivo + extension);

                        // save the file
                        file.SaveAs(path);
                        

                        return Json("/Images/IngDoctos/" + nombreArchivo + extension);
                    }
                    else
                    {
                        return Json("Archivo ya se encuentra registrado");
                    }
                }

                catch (Exception e)
                {
                    return Json("error" + e.Message);
                }
            }

            return Json("Ningun archivo seleccionado !");
        }
    }
}