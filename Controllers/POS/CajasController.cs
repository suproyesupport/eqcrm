using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EqCrm.Models.POS;

namespace EqCrm.Controllers.POS
{
    public class CajasController : Controller
    {
        // GET: Cajas
        public ActionResult Cajas()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)(Session["StringConexion"]);

            string queryCajasAsignadas = "select cu.id_usuario as USUARIO, c.ID_CAJA, c.nombre as CAJA, cu.FECHA_ASIGNADA from cajas_usuarios cu inner join cajas c on c.id_caja = cu.id_caja inner join `dlempresa`.`usuarios_web` u on u.usuario = cu.id_usuario where cu.ESTATUS = 1";
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
           // DataTable TablaCajas = new DataTable();
           // ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlListarCajasAsignadas(queryCajasAsignadas, TablaCajas, DB);
           // stringConexionMySql.CerrarConexion();


            string queryCajasRegistradas = "SELECT id_caja, nombre FROM cajas";
            DataTable TablaCajas2 = new DataTable();
            ViewBag.Tabla2 = stringConexionMySql.LlenarDTTableHTmlListarCajasRegistradas(queryCajasRegistradas, TablaCajas2, DB);
            stringConexionMySql.CerrarConexion();


            return View("~/Views/POS/Cajas/Cajas.cshtml");
        }

        [HttpPost]
        public string Create(Caja caja)
        {
            try
            {
                var cajaVar = caja;
                if (!String.IsNullOrEmpty((cajaVar.Nombre)))
                {
                    ConexionMySQL conexionMySql = new ConexionMySQL();
                    string DB = (string)(Session["StringConexion"]);

                    string queryCaja = "INSERT INTO cajas(nombre) VALUES ('" + cajaVar.Nombre + "')";
                    StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                    string cError = "";
                    bool lError = false;

                    lError = stringConexionMySql.ExecCommand(queryCaja, DB, ref cError);

                    if (lError == true)
                    {
                        return cError;
                    }
                    return "true";
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {
                return ex.StackTrace.ToString();
            }
        }

        [Route("/{id}")]
        public ActionResult Editar(string id)
        {
            //EjecutarCommandoCaja
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)(Session["StringConexion"]);
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


            /*PARA OBTENER INFORMACION DE USUARIOS EN UN DROPDOWN LIST EN OTRA BASE DE DATOS DLEMPRESA*/

            List<SelectListItem> listadousuarios = new List<SelectListItem>();
            string validarUsuarios = "SELECT usuario AS USUARIO, nombre as NOMBRE FROM usuarios_web";
            ConexionMySQL llenarEmpresas = new ConexionMySQL();
            llenarEmpresas.LLenarDropDownList(validarUsuarios, "dlempresa", listadousuarios);
            ViewData["DlUsuarios"] = listadousuarios;
            llenarEmpresas.CerrarConexion();


            //FINAL
            string queryInfoCajas = "SELECT id_caja, nombre FROM cajas WHERE id_caja = '" + id.ToString() + "'";

            Tuple<string, string> resultadoQueryInfoGeneral = stringConexionMySql.EjecutarCommandoCaja(queryInfoCajas, DB);
            //Devolviendo informacion al modelo para usarlo en la vista.
            var CajaDevuelta = new Caja();
            CajaDevuelta.id_caja = resultadoQueryInfoGeneral.Item1.ToString();
            CajaDevuelta.Nombre = resultadoQueryInfoGeneral.Item2.ToString();
            return View("~/Views/POS/Cajas/Editar.cshtml", CajaDevuelta);
        }

        [Route("/{id}")]
        public ActionResult Asignar(int id)
        {
            //EjecutarCommandoCaja
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)(Session["StringConexion"]);
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            //FINAL
            string queryInfoCajas = "SELECT id_caja, nombre FROM cajas WHERE id_caja = '" + id.ToString() + "'";

            Tuple<string, string> resultadoQueryInfoGeneral = stringConexionMySql.EjecutarCommandoCaja(queryInfoCajas, DB);
            //Devolviendo informacion al modelo para usarlo en la vista.
            var CajaDevuelta = new Caja();
            CajaDevuelta.id_caja = resultadoQueryInfoGeneral.Item1.ToString();
            CajaDevuelta.Nombre = resultadoQueryInfoGeneral.Item2.ToString();



            /*PARA OBTENER INFORMACION DE USUARIOS EN UN DROPDOWN LIST EN OTRA BASE DE DATOS DLEMPRESA*/
            List<SelectListItem> listadousuarios = new List<SelectListItem>();
            string validarUsuarios = "SELECT usuario as USUARIO, nombre as NOMBRE FROM usuarios_web";
            ConexionMySQL llenarEmpresas = new ConexionMySQL();
            llenarEmpresas.LLenarDropDownList(validarUsuarios, "dlempresa", listadousuarios);
            ViewData["DlUsuarios"] = listadousuarios;
            llenarEmpresas.CerrarConexion();


            //FINAL
            return View("~/Views/POS/Cajas/Asignar.cshtml", CajaDevuelta);
        }

        [HttpPost]
        public bool ActualizarCaja(Caja caja)
        {
            try
            {
                var cajaVar = caja;
                string cError = "";
                bool lError = false;

                ConexionMySQL conexionMySql = new ConexionMySQL();
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
                string DB = (string)(Session["StringConexion"]);


                string queryCajaActualizar = "UPDATE cajas SET nombre = '" + cajaVar.Nombre + "' WHERE id_caja = " + cajaVar.id_caja;
                lError = stringConexionMySql.ExecCommand(queryCajaActualizar, DB, ref cError);
                if (lError == true)
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Route("/{id}")]
        [HttpPost]
        public bool AsignarCaja(Caja caja)
        {
            try
            {
                var cajaVar = caja;
                string cError = "";
                bool lError = false;


                ConexionMySQL conexionMySql = new ConexionMySQL();
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
                string DB = (string)(Session["StringConexion"]);

                string storedProcedure = "ventaPOS_asignar_cajas";
                List<string> listaAsignarCaja = new List<string>();
                listaAsignarCaja.Add(Convert.ToString(cajaVar.id_caja));
                listaAsignarCaja.Add(cajaVar.Usuario);

                bool errorSP = stringConexionMySql.EjecutarStoredProcedure(storedProcedure, DB, listaAsignarCaja,ref cError);
                if (lError == true)
                {
                    return false;
                }

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Route("/{id}")]
        [HttpPost]
        public ActionResult Eliminar(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string test = id;

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)(Session["StringConexion"]);
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string queryEliminarCaja = "DELETE FROM cajas WHERE id_caja = '" + id.ToString() + "'";

            string cError = "";
            bool lError = false;

            lError = stringConexionMySql.ExecCommand(queryEliminarCaja, DB, ref cError);

            if (lError == true)
            {
                return View("~/Views/POS/Cajas/Cajas.cshtml");
            }


            return View("~/Views/POS/Cajas/Cajas.cshtml");
        }

        [HttpPost]
        public ActionResult Desasignar(Caja caja)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            var cajaVar = caja;
            string idCaja = caja.id_caja.ToString();
            string idUsuario = caja.Usuario.ToString();

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = (string)(Session["StringConexion"]);
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string queryEliminarCaja = "UPDATE cajas_usuarios set estatus = 0 where id_usuario = '" + idUsuario + "' and id_caja = " + idCaja;

            string cError = "";
            bool lError = false;

            lError = stringConexionMySql.ExecCommand(queryEliminarCaja, DB, ref cError);

            if (lError == true)
            {
                return View("~/Views/POS/Cajas/Cajas.cshtml");
            }


            return View("~/Views/POS/Cajas/Cajas.cshtml");
        }
    }
}