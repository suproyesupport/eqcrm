using EqCrm.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;

namespace EqCrm
{
    public class EmpresasController : Controller
    {
        /*
            * Dentro de este apartado se modifica la consulta para la base de datos en relacion a recibir las empresas 
            * a las que tiene acceso de ingresar o no ingresar el usuario y se deja comentado el query anterior para control 
            * de cambios.
        */
        [Route("/{name}")]
        public ActionResult Empresas(string name)
        {
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string[] parametros = name.Split('|');
            string cName = parametros[0].ToString();
            string cRol = parametros[1].ToString();
            string cUserConected = cName;

            string cUrlApiQuery = System.Configuration.ConfigurationManager.AppSettings["DireccionApiQuery"];
            string oBaseDatos = "dlempresa";
            



            if (string.IsNullOrEmpty(cUserConected))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<SelectListItem> listadoempresas = new List<SelectListItem>();
                
              //  string validarEmpresas = "SELECT base_datos, nombre_empresa FROM dlempresa.empresa e INNER JOIN usuario_empresa ue ON e.id_empresa = ue.id_empresa and usuario = '" + cRol + "' and estado = 1";


                
                //string cInst = "SELECT base_datos,nombre_empresa FROM dlempresa.empresa";
                //string validarEmpresas = "SELECT base_datos, nombre_empresa FROM dlempresa.empresa e INNER JOIN usuario_empresa ue ON e.id_empresa = ue.id_empresa and usuario = '" + Session["Rol"] + "' and estado = 1";
                string validarEmpresas = "SELECT e.base_datos, e.nombre_empresa FROM dlempresa.empresa e INNER JOIN usuario_empresa ue ON e.id_empresa = ue.id_empresa AND ue.usuario = '" + cRol + "' AND ue.estado = 1 ORDER BY e.nombre_empresa ASC";
                ConexionMySQL llenarEmpresas = new ConexionMySQL();
                llenarEmpresas.LLenarDropDownList(validarEmpresas, "dlempresa", listadoempresas);
                ViewData["DlEmpresa"] = listadoempresas;
                //llenarEmpresas.CerrarConexion();






                this.Session["Usuario"] = (object)cName;                
                this.Session["Rol"] = (object)cRol;
                System.Web.HttpContext.Current.Session["Usuario"] = cName;

            }


            return View();

        }

        [HttpPost]
        public ActionResult ObtenerDatosEmpresa(DatosEmpresas emp)
        {
            string cInst = "";
            ConexionMySQL conectar = new ConexionMySQL();
            StringConexionMySQL cerrar = new StringConexionMySQL();
            cInst = "SELECT stringconexion,base_datos,nit,nombre_empresa,nombre_comercial,AfiliacionIVA,email,direccion,apifel,requestorfe,cUserFe, id_empresa, telefono,certificador,ctoken,jSonWebApp,cCampo6,cCampo1,cCampo5,cCampo4,panelweb1,cTipoPersoneria,autorizacion,nrc_sv,cod_actividad_sv,tipo_establecimiento_sv FROM dlempresa.empresa WHERE base_datos ='" + emp.BaseDatos + "'";


            string DBDlempresa = conectar.generarStringDB("dlempresa");
            

            using (IDbConnection dbConnection = new MySqlConnection(DBDlempresa))
            {
                dbConnection.Open();

                // Realizar la consulta SQL utilizando Dapper                
                IEnumerable<DatosGenralesEmp> empresas = dbConnection.Query<DatosGenralesEmp>(cInst);

                // Iterar a través de los resultados
                foreach (var empr in empresas)
                {
                    Session["StringConexion"] = empr.stringconexion;
                    Session["oBase"] = empr.base_datos;
                    Session["cNit"] = empr.nit;
                    Session["cNombreEmisor"] = empr.nombre_empresa;
                    Session["cNombreComercial"] = empr.nombre_comercial;
                    Session["cAfiliacion"] = empr.AfiliacionIVA;
                    Session["cEmail"] = empr.email;
                    Session["cDireccion"] = empr.direccion;
                    Session["cUrlFel"] = empr.apifel;
                    Session["cToken"] = empr.requestorfe;
                    Session["cUserFe"] = empr.cUserFe;
                    Session["cIdEmpresa"] = empr.id_empresa;
                    Session["cTelefono"] = empr.telefono;
                    Session["certificador"] = empr.certificador;
                    Session["tokenfel"] = empr.ctoken;
                    Session["jSonWebApp"] = empr.jSonWebApp;
                    Session["cCampo6"] = empr.cCampo6;
                    Session["cCampo1"] = empr.cCampo1;
                    Session["cCampo5"] = empr.cCampo5;
                    Session["cCampo4"] = empr.cCampo4;
                    Session["cPanel1"] = empr.panelweb1;
                    Session["cPersoneria"] = empr.cTipoPersoneria;
                    Session["cAutorizacion"] = empr.autorizacion;
                    Session["IDCajaAsignada"] = "1";
                    Session["cNrc_sv"] = empr.nrc_sv;
                    Session["cCod_actividad_sv"] = empr.cod_actividad_sv;
                    Session["cTipo_establecimiento_sv"] = empr.tipo_establecimiento_sv;

                }
                dbConnection.Close();
            }

            return RedirectToAction("Index", "Home");

           

        }
    }
}