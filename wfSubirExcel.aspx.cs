using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace EqCrm
{
    public partial class wfSubirExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                //Response.Redirect("~/Views/Account/Login.cshtml");
                //Response.Redirect("C:\Users\Oscar\Documents\Proyectos\EqCrmPos\Views\Account\Login.cshtml", true);
                //return (ActionResult)this.RedirectToAction("Login", "Account");
            }
        }


        protected void btnCargar_Click(object sender, EventArgs e)
        {
            string extension = System.IO.Path.GetExtension(FileUpload1.FileName);

            string oDb = (string)this.Session["StringConexion"];

            bool lError = false;

            string cError = "";

            StringConexionMySQL mysql = new StringConexionMySQL();

            Importar imp = new Importar();

            DataTable dt = new  DataTable();

            string cInst = "SELECT id_codigo AS No,nit AS NIT,cliente AS CLIENTE,dtesemitidos AS DTES,ifnull(total,0)AS TOTAL,mesano AS MESANO FROM clientesrecurrencia";

            mysql.LlenarTabla(cInst, dt, oDb);

            grViewExcel.DataSource = dt;
            grViewExcel.DataBind();
            //cInst = "delete from clientesrecurrencia";
            //lError = mysql.ExecCommand(cInst, oDb, ref cError);

            //if (lError == true)
            //{
            //    mysql.CerrarConexion();
            //}
            //else
            //{
            //    mysql.CerrarConexion();
            //}


            //// No sirve por que A VECES FUNCIONA Y A VECES NO
            //if (FileUpload1.HasFile)
            //{
            //    if (extension == ".xlsx")
            //    {
            //        FileUpload1.SaveAs(Server.MapPath("/Archivos/" + FileUpload1.FileName));
            //        label1.Text = "Se subio un archivo";
            //        //imp.importarExcel(grViewExcel, "DATOS", "", true, "C:\\Proyectos\\EqCrmPos\\Archivos\\" + FileUpload1.FileName);
            //       imp.importarExcel(grViewExcel, "DATOS", "", true, "C:\\inetpub\\EqCrm\\Archivos\\" + FileUpload1.FileName);


            //        //limcred -> 

            //        foreach (GridViewRow row in grViewExcel.Rows)
            //        {

            //            cInst = " INSERT INTO clientesrecurrencia (cliente, nit,mesano,total,dtesemitidos,fecha) ";
            //            cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}') ", row.Cells[2].Text, row.Cells[1].Text, row.Cells[5].Text, row.Cells[4].Text, row.Cells[3].Text,DateTime.Now.ToString("yyyy-MM-dd"));

            //            lError = mysql.ExecCommand(cInst, oDb, ref cError);

            //            if (lError == true)
            //            {   
            //                mysql.CerrarConexion();
            //                row.Cells[5].Text = cError;
            //            }

            //            mysql.CerrarConexion();
            //        }

            //    } else
            //    {
            //        label1.Text = "Seleccionar un archivo excel";
            //    }
            //}
            //else
            //{
            //    label1.Text = "Debes seleccionar un archivo";
            //}

            cInst = "UPDATE clientesrecurrencia a, clientes b ";
            cInst += " set a.id_codigo = b.id_codigo,";
            cInst += " a.minimo = b.limcred,";
            cInst += " a.anualmensual = b.id_coordinador,";
            cInst += " a.preciodte = b.pcomi";
            cInst += " WHERE a.nit = b.nit ";

            lError = mysql.ExecCommand(cInst, oDb, ref cError);

            if (lError == true)
            {
                mysql.CerrarConexion();
             
            }

            mysql.CerrarConexion();

            cInst = "UPDATE clientesrecurrencia set totalafacturar = preciodte*dtesemitidos, facturarminimo=1, descripafacturar =concat(\"SERVICIO DE DTE MENSUAL DEL MES \" , RIGHT(mesano,2), \" DEL \", LEFT(mesano,4)  ) where dtesemitidos >= minimo ";
            
            lError = mysql.ExecCommand(cInst, oDb, ref cError);

            if (lError == true)
            {
                mysql.CerrarConexion();

            }

            mysql.CerrarConexion();

            cInst = "UPDATE clientesrecurrencia set totalafacturar = preciodte*minimo, facturarminimo=2, descripafacturar =concat(\"SERVICIO DE DTE MINIMO MENSUAL DEL MES \" , RIGHT(mesano,2), \" DEL \", LEFT(mesano,4)  ) where dtesemitidos < minimo ";

            lError = mysql.ExecCommand(cInst, oDb, ref cError);

            if (lError == true)
            {
                mysql.CerrarConexion();

            }

            mysql.CerrarConexion();




        }

        protected void btnFacturar_Click(object sender, EventArgs e)
        {

            string oDb = (string)this.Session["StringConexion"];
            bool lError = false;
            StringConexionMySQL mysql = new StringConexionMySQL();
            string cInst = "";
            DataTable dt = new DataTable();
            int numeroMayor = 0;
            string id_vendedor = "";
            Funciones f = new Funciones();
            string cDireccion = "";
            string cEmail = "";
            string cError = "";

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


            cInst = "SELECT id_codigo AS CODIGO, cliente AS CLIENTE,nit as NIT,fecha AS FECHA, minimo AS MINIMO, dtesemitidos AS EMITIDOS, preciodte AS PRECIO, totalafacturar AS SUBTOTAL, facturarminimo AS FACMIN, descripafacturar AS OBS,(SELECT direccion FROM clientes WHERE clientes.id_codigo = clientesrecurrencia.id_codigo) AS DIRECCION,(SELECT email FROM clientes WHERE clientes.id_codigo = clientesrecurrencia.id_codigo) AS EMAIL from clientesrecurrencia WHERE id_codigo > 0 ";
            mysql.LlenarTabla(cInst, dt, oDb);
            grViewExcel.DataSource = dt;
            grViewExcel.DataBind();

            numeroMayor = Funciones.NumeroMayor("facturas", "no_factura", "serie = 'FEL'", oDb);
            adenda.leyendafacc = f.ObtieneDatos("resolucionessat", "leyendafacc", "serie = 'FEL'", oDb);

            foreach (GridViewRow row in grViewExcel.Rows)
            {
                
                id_vendedor = "0";//f.ObtieneDatos("clientes", "id_vendedor", "id_codigo=" + row.Cells[0].Text.ToString(), oDb);
                cDireccion  = row.Cells[10].Text;
                cEmail = row.Cells[11].Text;


               

                cInst = "INSERT INTO facturas (no_factura, serie, fecha, status, id_cliente, cliente,id_vendedor,direccion,total,obs,id_agencia,nit,fechap,tdescto,email) ";
                cInst += string.Format("VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',ADDDATE('{12}', '{13}'),'{14}','{15}') ",
                    numeroMayor.ToString(),
                    "FEL",
                    Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd"),
                    "I",
                    row.Cells[0].Text,
                    row.Cells[1].Text,
                    id_vendedor.ToString(),
                    cDireccion.ToString(),
                    row.Cells[7].Text,
                    "INGRESO DESDE EL MODULO AUTOMATICO DE FACTURACION",
                    1,
                    row.Cells[2].Text,
                    Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd"),
                  30, 0, cEmail);




                lError = mysql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                   
                   mysql.CerrarConexion();
                    

                }
                mysql.CerrarConexion();

                if (row.Cells[8].Text == "2")
                {
                    cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                    numeroMayor.ToString(),
                    "FEL",
                    12,
                    row.Cells[4].Text,
                    row.Cells[6].Text,
                    row.Cells[7].Text,
                    1,
                    0,
                    row.Cells[9].Text, 1);
                }
                else
                {
                    cInst = " INSERT INTO detfacturas (no_factura, serie,id_codigo,cantidad,precio,subtotal,no_cor,tdescto,obs,id_agencia) ";
                    cInst += string.Format("VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                    numeroMayor.ToString(),
                    "FEL",
                    12,
                    row.Cells[5].Text,
                    row.Cells[6].Text,
                    row.Cells[7].Text,
                    1,
                    0,
                    row.Cells[9].Text, 1);
                }

                lError = mysql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {

                    mysql.CerrarConexion();

                }
                mysql.CerrarConexion();

                // vamos a insertar y asumir de momento que es al credito.
                cInst = "INSERT INTO ctacc (id_codigo, docto, serie, id_movi, fechaa, fechap, fechai, importe, saldo, obs, hechopor, tipodocto, id_agencia) ";
                cInst += string.Format("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ADDDATE('{5}', {6}), '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}') ",
                                               row.Cells[0].Text,
                                               numeroMayor.ToString(),
                                               "FEL", 
                                                1,
                                                Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd"),
                                                Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd"),
                                                30, Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd"),
                                                row.Cells[7].Text,
                                                row.Cells[7].Text,
                                                "INGRESO DESDE EL MODULO AUTOMATICO DE FACTURACION",
                                                "WEB", "FACTURA", 1);

                lError = mysql.ExecCommand(cInst, oDb, ref cError);

                if (lError == true)
                {
                    mysql.CerrarConexion();

                }

                mysql.CerrarConexion();

                datosEmisor.Tipo = "FCAM";//f.ObtieneDatos("resolucionessat", "tipoactivo", "serie ='FEL'", oDb);

                if (datosEmisor.Tipo == "FCAM")
                {
                    cfc.NumeroAbono = "1";
                    cfc.FechaVencimiento = Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd");
                    cfc.MontoAbono = row.Cells[7].Text;

                }
                datosEmisor.FechaHoraEmision = Convert.ToDateTime(row.Cells[3].Text).ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss").ToString();
                datosEmisor.CodigoMoneda = "GTQ";


                // de esta forma es parametrizado datosEmisor.cCorreos = cEmail;


                // de esta forma es para pruebas
                datosEmisor.cCorreos = cEmail;// "info @cgsersa.com,gerencia @cgsersa.com,joseochoaintec@gmail.com";
                datosEmisor.cAsunto = "FACTURA DE DTES FEL EMITIDA ";

                datosEmisor.NitEmisor = cNit;
                datosEmisor.NombreEmisor = cEmisor.Replace("&","&amp;");
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

                datosReceptor.IdReceptor = row.Cells[2].Text;
                datosReceptor.NombreReceptor = row.Cells[1].Text;
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
                double subtotal = Convert.ToDouble(row.Cells[7].Text);
                double nMontoImpuesto = 0.00;
                double nMontoTotalImpuesto = 0.00;

                items[0].BienOServicio = "S";
                
                items[0].NumeroLinea = "1";
                if (row.Cells[8].Text == "2")
                {
                    items[0].Cantidad = row.Cells[4].Text;
                    nCantidad = Convert.ToDouble(row.Cells[4].Text);
                }
                else
                {
                    items[0].Cantidad = row.Cells[5].Text;
                    nCantidad = Convert.ToDouble(row.Cells[5].Text);
                }
                                     
                items[0].UnidadMedida = "SER";
                items[0].Descripcion = row.Cells[9].Text;
                items[0].PrecioUnitario = f.FormatoDecimal(row.Cells[6].Text, 6, false).Replace(",", "");
                nPrecioSD = Convert.ToDouble(row.Cells[6].Text) * Convert.ToDouble(nCantidad);
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
                var Dte = CrearXml.CreaDteFact(datosEmisor, datosReceptor, cFrases, items, totales, true, cId_Interno, subtotal.ToString(), "FEL", adenda, cfc,"G4S",cnc,cfe,cex);

                Dte = Dte.Replace("{CONT}", "");


                var wsEnvio = wsConnector.wsEnvio("POST_DOCUMENT_SAT_PDF", Funciones.Base64Encode(Dte), cId_Interno, cUserFe, cUrlFel, cToken, "SYSTEM_REQUEST", "GT", cNit, false, "");

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

                    lError = mysql.ExecCommand(cUpdate, oDb, ref cError);
                    if (lError == true)
                    {

                        mysql.CerrarConexion();

                    }

                    mysql.CerrarConexion();

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

                    lError = mysql.ExecCommand(cUpdate, oDb, ref cError);
                    if (lError == true)
                    {

                        mysql.CerrarConexion();

                    }
                    mysql.CerrarConexion();

                }
                else
                {
                    row.Cells[11].Text = wsEnvio[2].ToString();
                }

                numeroMayor = numeroMayor + 1;

            }

            mysql.KillAllMySQL(oDb);


        }
    }
}