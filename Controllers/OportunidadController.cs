using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EqCrm.Models;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class OportunidadController : Controller
    {
        public ActionResult IngresoOportunidad()
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            { 
                return RedirectToAction("Login", "Account");
            }
            else
            {

                string Base_Datos = (string)(Session["StringConexion"]);
                StringConexionMySQL mysql = new StringConexionMySQL();
                /// Contacto Vendedores
                List<SelectListItem> listadoasesores = new List<SelectListItem>();
                string cInst = "SELECT id_codigo,nombre FROM vendedores";
                mysql.LLenarDropDownList(cInst, Base_Datos,listadoasesores);
                ViewData["asesores"] = listadoasesores;
                
                // Contacto Principal empresa
                List<SelectListItem> listadocontacto1 = new List<SelectListItem>();
                cInst = "SELECT id_prospecto,nombre FROM prospectos";
                mysql.LLenarDropDownList(cInst, Base_Datos, listadocontacto1);
                ViewData["contacto1"] = listadocontacto1;
                
                // Contactos 
                List<SelectListItem> listadocontacto2 = new List<SelectListItem>();
                cInst = "SELECT id_contacto,nombre FROM contactos";                
                mysql.LLenarDropDownList(cInst, Base_Datos, listadocontacto2);
                ViewData["contacto2"] = listadocontacto2;

                // Contactos 
                List<SelectListItem> listatipoclie = new List<SelectListItem>();
                cInst = "SELECT id_tipo,descripcion FROM cattipoclie";
                mysql.LLenarDropDownList(cInst, Base_Datos, listatipoclie);
                ViewData["tipoclie"] = listatipoclie;


                List<SelectListItem> listadoasesores2 = new List<SelectListItem>();
                cInst = "SELECT id_codigo,nombre FROM vendedores";
                mysql.LLenarDropDownList(cInst, Base_Datos, listadoasesores2);
                ViewData["asesores2"] = listadoasesores2;


                // Cerrar conexion
                mysql.CerrarConexion();


            }

            return View();

        }

        [HttpPost]
        public ActionResult IngresoProspecto(DatosProspecto prospecto)
        {
            StringConexionMySQL insert = new StringConexionMySQL();
            string Base_Datos = (string)(Session["StringConexion"]);

            string instruccionSQL = "";
            instruccionSQL += "INSERT INTO prospectos (id_tipo,nombre,id_vendedor,direccion,telefono,celular,email,web_page) ";
            instruccionSQL += string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}')",
                                            prospecto.id_tipo,
                                            prospecto.nombre,
                                            prospecto.id_vendedor,
                                            prospecto.direccion,
                                            prospecto.telefono,
                                            prospecto.celular,
                                            prospecto.email,
                                            prospecto.web_page
                                           );
            insert.EjecutarCommando(instruccionSQL, Base_Datos);
            insert.CerrarConexion();

            return RedirectToAction("IngresoOportunidad", "Oportunidad");
            
        }

        [HttpPost]
        public ActionResult IngresoContacto(DatosContactos contactos)
        {
            StringConexionMySQL insert = new StringConexionMySQL();
            string Base_Datos = (string)(Session["StringConexion"]);

            string instruccionSQL = "";
            instruccionSQL += "INSERT INTO contactos (id_tipo,nombre,id_vendedor,direccion,telefono,celular,email,web_page) ";
            instruccionSQL += string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}')",
                                            contactos.id_tipo,
                                            contactos.nombre,
                                            contactos.id_vendedor,
                                            contactos.direccion,
                                            contactos.telefono,
                                            contactos.celular,
                                            contactos.email,
                                            contactos.web_page
                                           );

            insert.EjecutarCommando(instruccionSQL, Base_Datos);
            insert.CerrarConexion();
            return View();
            //return RedirectToAction("IngresoOportunidad", "Oportunidad");

        }

        [HttpPost]
        public ActionResult IngresoOportunidad(DatosIngresoProspectos op)
        {
            StringConexionMySQL insert = new StringConexionMySQL();
            string Base_Datos = (string)(Session["StringConexion"]);

            string instruccionSQL = "";
            instruccionSQL += "INSERT INTO oportunidades_prospectos (oportunidad,tipo,id_vendedor,id_prospecto,id_contacto,observaciones,precio,fecha,fecha_cierre,id_pipeline) ";
            instruccionSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}','{8}','{9}')",
                                            op.oportunidades.oportunidad,
                                            "O",
                                            op.oportunidades.id_vendedor,
                                            op.oportunidades.id_prospecto,
                                            op.oportunidades.id_contacto,
                                            op.oportunidades.observaciones,
                                            op.oportunidades.precio,
                                            op.oportunidades.fecha,
                                            op.oportunidades.fecha_cierre,
                                            "1"
                                           );

            insert.EjecutarCommando(instruccionSQL, Base_Datos);
            insert.CerrarConexion();

           // return View();
            return RedirectToAction("IngresoOportunidad", "Oportunidad");

        }
    }
}