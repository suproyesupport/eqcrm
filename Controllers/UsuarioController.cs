using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EqCrm.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

/// <summary>
/// Controlador que lleva el control de los usuarios y empresas para loguearse
/// PRUEBA 05082022
/// </summary>

namespace EqCrm.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Usuario()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string cAuto = (string)this.Session["cAutorizacion"];

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string cInst = "SELECT USUARIO AS ROL, NOMBRE from usuarios WHERE autorizacion = '"+cAuto+"'";
          

            DataTable Tabla_para_Datos = new DataTable();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlListarUsuario(cInst, Tabla_para_Datos, DB);
            stringConexionMySql.CerrarConexion();

            return (ActionResult)this.View();
        }

        [Route("/{id}")]
        public ActionResult Editar(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string test = id;
            string cAuto = (string)this.Session["cAutorizacion"];
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            

            string queryInfoGeneral = "SELECT admin, nombre FROM usuarios where usuario = '" + id.ToString() + "'";

            Tuple<string, string> resultadoQueryInfoGeneral = stringConexionMySql.EjecutarCommandoUsuario(queryInfoGeneral, DB);
            //Devolviendo informacion al modelo para usarlo en la vista.
            var UsuarioDevuelto = new Usuario();
            UsuarioDevuelto.User = id.ToString();
            UsuarioDevuelto.Nombre = resultadoQueryInfoGeneral.Item2.ToString();
            UsuarioDevuelto.Admin = resultadoQueryInfoGeneral.Item1.ToString();


            string cInst = "INSERT INTO usuario_categorias(usuario, id_categoria, estado,autorizacion)";
            cInst += " SELECT 'REEMPLAZAR',id,0,'AUTORIZACION'  FROM categorias WHERE id not in (SELECT id_categoria FROM usuario_categorias WHERE  usuario = 'REEMPLAZAR') ";
            cInst = cInst.Replace("REEMPLAZAR", id);
            cInst = cInst.Replace("AUTORIZACION", cAuto);
            stringConexionMySql.EjecutarCommando(cInst, DB);


            // esto es temporal
            cInst = "UPDATE dlempresa.usuario_categorias SET autorizacion = '" + cAuto + "',estado=0  WHERE autorizacion='' AND  usuario ='" + id + "' AND id_categoria IN(1,7,8,13,4) ";
            stringConexionMySql.EjecutarCommando(cInst, DB);


            cInst = "INSERT INTO usuario_menus(usuario, id_menu, estado,autorizacion)";
            cInst += " SELECT 'REEMPLAZAR',id,0,'AUTORIZACION'  FROM menus WHERE id not in (SELECT id_menu FROM usuario_menus WHERE  usuario = 'REEMPLAZAR') ";
            cInst = cInst.Replace("REEMPLAZAR", id);
            cInst = cInst.Replace("AUTORIZACION", cAuto);
            stringConexionMySql.EjecutarCommando(cInst, DB);

            cInst = "UPDATE dlempresa.usuario_menus SET autorizacion = '" + cAuto + "',estado=0  WHERE autorizacion='' AND usuario ='" + id + "' AND id_menu IN(1,7,4,11,12,13,20,21,22,51,80,84)";
            stringConexionMySql.EjecutarCommando(cInst, DB);

            cInst = "INSERT INTO usuario_empresa(usuario, id_empresa, estado)";
            cInst += " SELECT 'REEMPLAZAR',id_empresa,0  FROM empresa WHERE id_empresa not in (SELECT id_empresa FROM usuario_empresa WHERE  usuario = 'REEMPLAZAR') ";
            cInst = cInst.Replace("REEMPLAZAR", id);
            stringConexionMySql.EjecutarCommando(cInst, DB);

            string usuarioCategoria = "select uc.usuario AS USUARIO, uc.id_categoria AS CATEGORIA, c.nombre AS NOMBRE, uc.estado AS INGRESA from usuario_categorias uc inner join categorias c on uc.id_categoria = c.id where usuario = '" + id + "' AND uc.autorizacion = '" + cAuto + "'"; 
            string usuarioEmpresa = "select ue.usuario AS USUARIO, ue.id_empresa AS EMPRESA, e.nombre_empresa AS EMPRESA, ue.estado AS INGRESA from usuario_empresa ue inner join empresa e on ue.id_empresa = e.id_empresa where usuario = '" + id + "' AND e.autorizacion = '" + cAuto + "'"; 
            string usuarioMenu = "select um.usuario AS USUARIO , um.id_menu AS MENU, m.nombre AS NOMBRE, m.modulo AS MODULO,um.estado AS INGRESA from usuario_menus um inner join menus m on um.id_menu = m.id where usuario = '" + id + "' AND um.autorizacion = '" + cAuto + "' ORDER BY m.modulo";


            DataTable Tabla_para_Datos2 = new DataTable();
            ViewBag.Tabla2 = stringConexionMySql.LlenarDTTableHTmlUser(usuarioCategoria, Tabla_para_Datos2, DB, 1);

            DataTable Tabla_para_Datos3 = new DataTable();
            ViewBag.Tabla3 = stringConexionMySql.LlenarDTTableHTmlEmpresa(usuarioEmpresa, Tabla_para_Datos3, DB, 2);

            DataTable Tabla_para_Datos4 = new DataTable();
            ViewBag.Tabla4 = stringConexionMySql.LlenarDTTableHTmlUser(usuarioMenu, Tabla_para_Datos4, DB, 3);

            return (ActionResult)this.View(UsuarioDevuelto);
        }


        [Route("/{id}")]
        public ActionResult EditarWebUser(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string cAuto = (string)this.Session["cAutorizacion"];

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");

            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string cInst = "SELECT usuario , nombre,ifnull(correo,'') from usuarios_web WHERE usuario='"+id+"'";

            Usuario usuario = new Usuario();
            stringConexionMySql.EjecutarLectura(cInst, DB);
            if (stringConexionMySql.consulta.HasRows)
            {
                while (stringConexionMySql.consulta.Read())
                {
                    usuario.User = stringConexionMySql.consulta.GetString(0).ToString();
                    usuario.Nombre = stringConexionMySql.consulta.GetString(1).ToString();
                    usuario.Id_vendedor = stringConexionMySql.consulta.GetString(2).ToString();
                }
            }

            DataTable Tabla_para_Datos = new DataTable();

            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlListarUsuarioWeb(cInst, Tabla_para_Datos, DB);
            stringConexionMySql.CerrarConexion();

            List<SelectListItem> listadoroles = new List<SelectListItem>();
            cInst = "SELECT usuario,nombre FROM usuarios WHERE autorizacion = '" + cAuto + "'";
            stringConexionMySql.LLenarDropDownList(cInst, DB, listadoroles);
            ViewData["rol"] = listadoroles;


            string Db = (string)this.Session["StringConexion"];
            StringConexionMySQL stconexion = new StringConexionMySQL();
            List<SelectListItem> listasucursal = new List<SelectListItem>();
            cInst = "SELECT id_agencia,nombre FROM catagencias";
            stconexion.LLenarDropDownList(cInst, Db, listasucursal);
            ViewData["sucursal"] = listasucursal;

            return (ActionResult)this.View(usuario);
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
            string DB = conexionMySql.generarStringDB("dlempresa");
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string queryEliminarUsuario = "DELETE FROM usuarios WHERE usuario = '" + id.ToString() + "'";

            string cError = "";
            bool lError = false;

            lError = stringConexionMySql.ExecCommand(queryEliminarUsuario, DB, ref cError);

            if (lError == true)
            {
                return (ActionResult)this.RedirectToAction("Usuario", "Usuario");
            }


            return (ActionResult)this.RedirectToAction("Usuario", "Usuario");
        }

        [Route("/{id}")]
        [HttpPost]
        public ActionResult EliminarUWeb(string id)
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string test = id;

            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

            string queryEliminarUsuario = "DELETE FROM usuarios_web WHERE usuario = '" + id.ToString() + "'";

            string cError = "";
            bool lError = false;

            lError = stringConexionMySql.ExecCommand(queryEliminarUsuario, DB, ref cError);

            if (lError == true)
            {
                return (ActionResult)this.RedirectToAction("Usuario", "Usuario");
            }


            return (ActionResult)this.RedirectToAction("Usuario", "Usuario");
        }
        [HttpPost]
        public bool ActualizarInformacion(Usuario usuario)
        {
            try
            {
                var user = usuario;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string DB = conexionMySql.generarStringDB("dlempresa");

                string actualizarUsuario = "UPDATE usuarios SET admin = '" + user.Admin + "'";
                actualizarUsuario += ", nombre = '" + user.Nombre + "'";
                if (!string.IsNullOrEmpty(user.Password))
                {
                    actualizarUsuario += ", password = '" + user.Password + "'";
                }
                actualizarUsuario += " WHERE usuario = '" + user.User + "'";
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                string cError = "";
                bool lError = false;

                lError = stringConexionMySql.ExecCommand(actualizarUsuario, DB, ref cError);

                if (lError == true)
                {
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }



        [HttpPost]
        public bool ActualizaWebUser(Usuario usuario)
        {
            try
            {
                var user = usuario;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string DB = conexionMySql.generarStringDB("dlempresa");

                string actualizarUsuario = "UPDATE usuarios_web SET rol = '" + user.Depto + "'";
                actualizarUsuario += ", nombre = '" + user.Nombre + "'";
                actualizarUsuario += ", correo = '" + user.Id_vendedor + "'";
                actualizarUsuario += ", id_sucursal = " + user.Sucursal + "";
                if (!string.IsNullOrEmpty(user.Password))
                {
                    actualizarUsuario += ", password = '" + user.Password + "'";
                }
                actualizarUsuario += " WHERE usuario = '" + user.User + "'";
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                string cError = "";
                bool lError = false;

                lError = stringConexionMySql.ExecCommand(actualizarUsuario, DB, ref cError);

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
        [HttpPost]
        public string Create(Usuario usuario)
        {

            try
            {
                var user = usuario;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string DB = conexionMySql.generarStringDB("dlempresa");

                string cAuto = (string)this.Session["cAutorizacion"];

                string cInst = "INSERT INTO usuarios(usuario,admin,nombre,autorizacion,id_vendedor,password) VALUES ('" + usuario.User + "','" + usuario.Admin + "','" + usuario.Nombre + "','"+ cAuto + "',0,'" + usuario.Password + "')";
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();


                string cError = "";
                bool lError = false;

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if(lError == true)
                {
                    return cError;
                }

                return "true";

            }
            catch( Exception ex)
            {
                return ex.StackTrace.ToString();
            }
        }

        [HttpPost]
        public string CreateWebUser(Usuario usuario)
        {
            try
            {
                var user = usuario;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string DB = conexionMySql.generarStringDB("dlempresa");
                Funciones f = new Funciones();
                string cAuto = (string)this.Session["cAutorizacion"];

                string sucu = usuario.Sucursal;

                string cInst = "INSERT INTO usuarios_web(usuario,nombre,autorizacion,password,rol,correo,id_sucursal) VALUES ('" + usuario.User + "','" + usuario.Nombre + "','" + cAuto + "',"+"'" + usuario.Password + "','" + usuario.Depto + "','" + usuario.Id_vendedor + "', " +  usuario.Sucursal + ")";
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                string cError = "";
                bool lError = false;
                string ckTecnico = usuario.Tecnico;


                if (ckTecnico == "S")
                {
                    string cUserConected = (string)(Session["Usuario"]);
                    string str = "";

                    string oDb = (string)(Session["StringConexion"]);
                    StringConexionMySQL insertarTecnico = new StringConexionMySQL();
                    string DB_ = (string)this.Session["StringConexion"];

                    string cInstTec = "INSERT INTO tecnico VALUES (NULL, '" + usuario.Nombre + "', 'A');";

                    bool lErrorT = false;

                    lErrorT = stringConexionMySql.ExecCommand(cInstTec, DB_, ref cError);
                }

                lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    return cError;
                }

                f.EnviarCorreoUsuario(usuario.User, usuario.Nombre, usuario.Id_vendedor, usuario.Password);

                return "true";


                

            }
            catch (Exception ex)
            {
                return ex.StackTrace.ToString();
            }
        }

        [HttpPost]
        public string[] ActualizarPermiso(UsuarioPermiso permiso)
        {
            try
            { 
                var user = permiso;
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string DB = conexionMySql.generarStringDB("dlempresa");
                string cError = "";
                bool lError = false;
                string cambioPermiso = "";
                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                if (user.servicio == 1)
                {
                    string cInst = "UPDATE usuario_categorias SET estado = " + user.valor + " WHERE id_categoria = " + user.id + " and usuario = '" + user.usuario + "'";
                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                }
                else if (user.servicio == 2)
                {
                    if (user.valor == true)
                    {
                        cambioPermiso = "1";
                    }
                    else
                    {
                        cambioPermiso = "0";
                    }
                    string cInst = "UPDATE usuario_empresa SET estado = " + cambioPermiso + " WHERE id_empresa = " + user.id + " and usuario = '" + user.usuario + "'";
                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                }
                else if (user.servicio == 3)
                {
                    string cInst = "UPDATE usuario_menus SET estado = " + user.valor + " WHERE id_menu = " + user.id + " and usuario = '" + user.usuario + "'";
                    lError = stringConexionMySql.ExecCommand(cInst, DB, ref cError);
                }    

                string[] respuesta = { "" };
                return respuesta;

            }
            catch (Exception ex)
            {
                string[] respuesta = { "false" ,ex.ToString() };
                return respuesta;
            }
        }


    }
}
