using EqCrm.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EqCrm.Controllers
{
    public class ConsultarCotizacionesController : Controller
    {


        [Route("/{id}")]
        public ActionResult VerCotizacion(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string[] strIdInterno = id.Split('|');

            string no_factura = strIdInterno[0].ToString();
            string serie = strIdInterno[1].ToString();

            //string queryVerCotizacion = "select p.no_factura, p.serie, p.fecha, p.cliente, p.nit, v.nombre as vendedor, p.direccion,if(p.tasa>=1,(p.total)/(p.tasa),p.total) as total, p.obs, i.producto, dp.cantidad, dp.precio, dp.tdescto, dp.subtotal, if(dp.url = '', i.foto, dp.url) as url , cl.telefono, cl.email,  v.nombre as nombre_vendedor, v.telefono as telefono_vendedor, v.email as correo_vendedor,p.tasa as tasa";
            string queryVerCotizacion = "select p.no_factura, p.serie, p.fecha, p.cliente, p.nit, v.nombre as vendedor, p.direccion,FORMAT(p.total,2) as total, p.obs, dp.obs as producto, dp.cantidad, FORMAT(dp.precio,2)as precio, FORMAT(dp.tdescto,2) as tdescto, FORMAT(dp.subtotal,2) as subtotal, if(dp.url = '', i.foto, dp.url) as url , cl.telefono, cl.email,  v.nombre as nombre_vendedor, v.telefono as telefono_vendedor, v.email as correo_vendedor,p.tasa as tasa,p.atencion";
            queryVerCotizacion += " from proformas p inner join detproformas dp on (p.no_factura = dp.no_factura and p.serie = dp.serie)";
            queryVerCotizacion += " inner join vendedores v on v.id_codigo = p.id_vendedor";
            queryVerCotizacion += " inner join inventario i on i.id_codigo = dp.id_codigo";
            queryVerCotizacion += " inner join clientes cl on cl.id_codigo = p.id_cliente";
            queryVerCotizacion += " where p.no_factura = " + no_factura.ToString();
            queryVerCotizacion += " and p.serie = '"+serie.ToString()+"' ORDER BY no_cor ASC";

            List<Cotizacion> resultado = stringConexionMySql.EjecutarCommandoVerCotizacion(queryVerCotizacion, DB);

            var model = resultado.Select(x => new Cotizacion
            {
                no_factura = x.no_factura,
                serie = x.serie,
                fecha = x.fecha,
                cliente = x.cliente,
                nit = x.nit,
                vendedor = x.vendedor,
                direccion = x.direccion,
                total = x.total,
                obs = x.obs,
                producto = x.producto,
                cantidad = x.cantidad,
                precio = x.precio,
                tdescto = x.tdescto,
                subtotal = x.subtotal,
                url = x.url,
                telefono = x.telefono,
                email = x.email,
                total_letras = _enletras(x.total,x.tasa.ToString()),
                nombre_vendedor = x.nombre_vendedor,
                telefono_vendedor = x.telefono_vendedor,
                correo_vendedor = x.correo_vendedor,
                tasa = x.tasa,
                atencion = x.atencion
            }).ToList();




            return (ActionResult)this.View(model);

        }

        //convertir a numeros

        public string _enletras(string num, string Tasa)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;
            double nTasa = Convert.ToDouble(Tasa.ToString());

            try

            {
                if (nTasa >= 1)
                {
                    nro = Convert.ToDouble(num) / Convert.ToDouble(nTasa);
                    
                }
                else
                {
                    nro = Convert.ToDouble(num);
                }

            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));


            
            if (nTasa >= 1)
            {
                if (decimales > 0)
                {
                    dec = " DOLARES CON " + decimales.ToString()+ "/100"; //+ " CENTAVOS";
                }
                else
                {
                    dec = " DOLARES ";
                }
            }
            else
            { 
                if (decimales > 0)
                {
                    dec = " QUETZALES CON " + decimales.ToString() + "/100"; // + " CENTAVOS";
                }
                else
                {
                    dec = " QUETZALES ";
                }
            }

            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }
        public string enletras(string num)
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
                dec = " CON " + decimales.ToString() + " CENTAVOS";
            }


            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        private string toText(double value)
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


        [HttpPost]
        public object GetCotizaciones(FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

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

                string queryCotizaciones = "Select p.NO_FACTURA, p.SERIE, p.FECHA, p.CLIENTE, p.NIT, v.NOMBRE, p.DIRECCION, p.TOTAL, p.TDESCTO, p.OBS from proformas p inner join vendedores v on p.id_vendedor = v.id_codigo";

                string pattern = "MM/dd/yyyy";
                DateTime parsedFecha1, parsedFecha2;

                if (!string.IsNullOrEmpty(doctos.fecha1) && !string.IsNullOrEmpty(doctos.fecha2))
                {
                    if (DateTime.TryParseExact(doctos.fecha1, pattern, null, DateTimeStyles.None, out parsedFecha1))
                    {
                        string fecha = parsedFecha1.Year + "-" +
                            (parsedFecha1.Month < 10 ? "0" + parsedFecha1.Month.ToString() : parsedFecha1.Month.ToString()) +
                            "-" + (parsedFecha1.Day < 10 ? "0" + parsedFecha1.Day.ToString() : parsedFecha1.Day.ToString());
                        queryCotizaciones += " WHERE p.fecha >= '" + fecha + "'";
                    }

                }

                if (!string.IsNullOrEmpty(doctos.fecha2) && !string.IsNullOrEmpty(doctos.fecha1))
                {
                    if (DateTime.TryParseExact(doctos.fecha2, pattern, null, DateTimeStyles.None, out parsedFecha2))
                    {
                        string fecha = parsedFecha2.Year + "-" +
                            (parsedFecha2.Month < 10 ? "0" + parsedFecha2.Month.ToString() : parsedFecha2.Month.ToString()) +
                            "-" + (parsedFecha2.Day < 10 ? "0" + parsedFecha2.Day.ToString() : parsedFecha2.Day.ToString());
                        queryCotizaciones += " AND p.fecha <= '" + fecha + "'";
                    }
                }

                LLenarListaCotizaciones.listaCotizaciones = llenar.ListaCotizaciones(queryCotizaciones, DB, LLenarListaCotizaciones.listaCotizaciones);
            }
            return JsonConvert.SerializeObject(LLenarListaCotizaciones.listaCotizaciones);
        }


        
        public ActionResult ConsultarCotizaciones(FiltroGenerico doctos)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

    }
}

