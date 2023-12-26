using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EqCrm.Controllers
{
    public class WebUserController : Controller
    {
        // GET: WebUser
        public ActionResult WebUser()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");

            //SUCURSAL
            string Db = (string)this.Session["StringConexion"];
            StringConexionMySQL stconexion = new StringConexionMySQL();
            string cAuto = (string)this.Session["cAutorizacion"];

            //string[] valores = Db.Split(';');
            //string nombrebasededatos = valores[6];
            //nombrebasededatos = nombrebasededatos.Replace("database=", "");



            //string cInst = "SELECT USUARIO , NOMBRE, ID_SUCURSAL from usuarios_web";
            //string cInst = "SELECT u.usuario AS USUARIO, u.nombre AS NOMBRE, u.rol AS ROL, u.id_sucursal AS IDSUCURSAL, a.nombre AS SUCURSAL ";
            string cInst = "SELECT u.usuario AS USUARIO, u.nombre AS NOMBRE, u.rol AS ROL, u.id_sucursal AS IDSUCURSAL ";
            cInst += "FROM usuarios_web u WHERE u.autorizacion = '"+cAuto+"'";
            //cInst += "LEFT JOIN " + nombrebasededatos + ".catagencias a ON u.id_sucursal = a.id_agencia";

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            DataTable Tabla_para_Datos = new DataTable();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlListarUsuarioWeb(cInst, Tabla_para_Datos, DB);
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> listadoroles = new List<SelectListItem>();
            cInst = "SELECT usuario,nombre FROM usuarios WHERE autorizacion ='"+cAuto+"'";
            stringConexionMySql.LLenarDropDownList(cInst, DB, listadoroles);
            ViewData["rol"] = listadoroles;

            //SUCURSAL
            List<SelectListItem> listasucursal = new List<SelectListItem>();
            cInst = "SELECT id_agencia,nombre FROM catagencias";
            stconexion.LLenarDropDownList(cInst, Db, listasucursal);
            ViewData["sucursal"] = listasucursal;

            return (ActionResult)this.View();
        }
    }
}