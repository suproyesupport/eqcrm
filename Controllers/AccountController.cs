using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Dapper;
using MySql.Data.MySqlClient;

namespace EqCrm
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return (ActionResult)this.View();
        }


        [HttpPost]
        public string ModificarPassword(Account acc)
        {
            string str = "";

            StringConexionMySQL modificar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string rol = "";
            ConexionMySQL conexionMySql = new ConexionMySQL();
            string DB = conexionMySql.generarStringDB("dlempresa");
            string cResultado;
            try
            {
                string cInst = "UPDATE usuarios_web SET ";
                cInst += "password = " + "'" + acc.nPass1 + "' ";
                cInst += "WHERE usuario = '" + acc.Name + "' AND password='" + acc.Password + "';";

                lError = modificar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    modificar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
                else
                {
                    cResultado = "false";
                    rol = "ACTUALIZACION DE CONTRASEÑA HA SIDO REALIZADA CON EXTIO";
                    //str = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResultado + "\", \"OBS\": \"Autenticación Exitosa\"}";
                    str = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResultado + "\", \"OBS\": \"" + rol + "\"}";
                }
            }
            catch (Exception ex)
            {
                string cResulttError = "true";
                string strresult = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResulttError + "\", \"OBS\": \"" + ex.Message.ToString() + "\"}";
                return strresult;
            }

           

            modificar.CerrarConexion();
            return str;
        }


        [HttpPost]
        public ActionResult Verify(Account acc)
        {
            try
            {
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string sentenciaSQL = "SELECT usuario, password FROM usuarios WHERE usuario ='" + acc.Name + "' AND password='" + acc.Password + "'";
                string str = conexionMySql.EjecutarLectura(sentenciaSQL, "dlempresa");
                if (conexionMySql.consulta.HasRows)
                {
                    this.Session["Usuario"] = (object)acc.Name;
                     
                    conexionMySql.CerrarConexion();
                    return (ActionResult)this.RedirectToAction("Empresas", "Empresas");
                }
                conexionMySql.CerrarConexion();
                return (ActionResult)this.Content("<html><head><script>alert('" + str + "');</script></head></html>");
            }
            catch (Exception ex)
            {
                return (ActionResult)this.Content("<html><head><script>alert('" + ex.StackTrace + "');</script></head></html>");
            }
        }
              

        
        public string _Login(Account acc)
        {
            //Session.Contents.RemoveAll();
            try
            {
                ConexionMySQL conexionMySql = new ConexionMySQL();

                string DB = conexionMySql.generarStringDlempresa();     //generarStringDB("dlempresa");



                StringConexionMySQL stringConexionMySql = new StringConexionMySQL();

                string rol = "";
                string id_sucursal = "";
                string intercompany = "";
                string sentenciaSQL = "";
                string str = "";

                IDbConnection db;


                using (db = new MySqlConnection(DB))
                {
                    db.Open();
                    
                        // Realizar la consulta SQL utilizando Dapper
                        string query = "SELECT rol,id_sucursal,intercompany FROM dlempresa.usuarios_web WHERE usuario ='" + acc.Name + "'";
                        IEnumerable<AccountDatos> accounts = db.Query<AccountDatos>(query);

                        // Iterar a través de los resultados
                        foreach (var webuser in accounts)
                        {
                            rol = webuser.rol;
                            id_sucursal = webuser.id_sucursal;
                            intercompany = webuser.intercompany;
                        }
                    
                    db.Close();
                    
                    
                }

                              


                /*
                 * En esta parte es donde vamos a ver el rol al que pertenece
                 */

                //string sentenciaSQL = "SELECT rol FROM usuarios_web WHERE usuario ='" + acc.Name + "'";
                //string str = conexionMySql.EjecutarLectura(sentenciaSQL, "dlempresa");
                //if (conexionMySql.consulta.HasRows)
                //{
                //    while (conexionMySql.consulta.Read())
                //    {
                //        rol = conexionMySql.consulta.GetString(0);
                //      //  conexionMySql.CerrarConexion();

                //    }
                //}





                /*
                * Fruta madre Coca aca si te estas viendo campeon cuando se puede hacer en una sola consulta campeon
                * 
                */
                //string sentenciaSQL2 = "SELECT id_sucursal FROM usuarios_web WHERE usuario ='" + acc.Name + "'";
                //string str2 = conexionMySql.EjecutarLectura(sentenciaSQL2, "dlempresa");
                //if (conexionMySql.consulta.HasRows)
                //{
                //    while (conexionMySql.consulta.Read())
                //    {
                //        id_sucursal = conexionMySql.consulta.GetString(0);
                //        //  conexionMySql.CerrarConexion();

                //    }
                //}



                //string sentenciaSQL3 = "SELECT intercompany FROM usuarios_web WHERE usuario ='" + acc.Name + "'";
                //string str3 = conexionMySql.EjecutarLectura(sentenciaSQL3, "dlempresa");
                //if (conexionMySql.consulta.HasRows)
                //{
                //    while (conexionMySql.consulta.Read())
                //    {
                //        intercompany = conexionMySql.consulta.GetString(0);
                //        //  conexionMySql.CerrarConexion();

                //    }
                //}



                /*
                 * Dentro de este apartado se envia a consultar los permisos que tiene asignado al rol tanto para menus como 
                 * para las categorias para que muestre o no muestre los menus en la pantalla principal y se guardan como cookies
                 * para que se eliminen esas variables al momento de cerrar la sesion. 
                 */

                string permisosMenus = "SELECT m.clave, um.estado FROM dlempresa.usuario_menus um INNER JOIN dlempresa.menus m ON um.id_menu = m.id WHERE usuario = '" + rol + "'";
                string permisosCategorias = "SELECT m.clave, uc.estado FROM dlempresa.usuario_categorias uc INNER JOIN dlempresa.categorias m on uc.id_categoria = m.id WHERE usuario = '" + rol + "'";
                string cResultado = "";
                HttpCookie permisosUserMenus = stringConexionMySql.EjecutarCommandoPermisosUsuarioMenus(permisosMenus, DB);
                Response.Cookies.Add(permisosUserMenus);

                HttpCookie permisosUserCategorias = stringConexionMySql.EjecutarCommandoPermisosUsuarioCategorias(permisosCategorias, DB);
                Response.Cookies.Add(permisosUserCategorias);

                sentenciaSQL = "SELECT usuario, password, id_sucursal, intercompany FROM dlempresa.usuarios_web WHERE usuario ='" + acc.Name + "' AND password='" + acc.Password + "'";

                using (IDbConnection dbConnection = new MySqlConnection(DB))
                {
                    dbConnection.Open();

                    // Realizar la consulta SQL utilizando Dapper

                    IEnumerable<AccountPassword> accountspassword = dbConnection.Query<AccountPassword>(sentenciaSQL);

                    // Iterar a través de los resultados                    

                    if ( !accountspassword.Any() )
                    {
                            cResultado = "true";
                             str = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResultado + "\", \"OBS\": \"Por favor revise si sus credenciales estan correctas\"}";
                    }
                    foreach (var webuser in accountspassword)                     
                    {
                        this.Session["Usuario"] = (object)acc.Name;
                        //Session["Usuario"] = acc.Name;
                        this.Session["Rol"] = (object)rol;
                        //Variable de sesión / Coca
                        this.Session["Sucursal"] = (object)id_sucursal;
                        this.Session["Intercompany"] = (object)intercompany;

                        //Session["Rol"] = rol;
                        cResultado = "false";

                        
                        str = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResultado + "\", \"OBS\": \"" + rol + "\"}";
                  

                    }
                    dbConnection.Close();
                }


               

                return str;

            }
            catch (Exception ex)
            {
                string cResulttError = "true";                
                string strresult = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResulttError + "\", \"OBS\": \""+ ex.Message.ToString() + "\"}";
                return strresult;
            }
        }

        [HttpPost]
        public string ReenvioCorreoPassword(Account acc)
        {
            string correo = "";
            string Password = "";
            string user = "";
            try
            {
                ConexionMySQL conexionMySql = new ConexionMySQL();
                string sentenciaSQL = "SELECT usuario, password,correo FROM usuarios_web WHERE usuario ='" + acc.Name + "'";
                string str = conexionMySql.EjecutarLectura(sentenciaSQL, "dlempresa");
                if (conexionMySql.consulta.HasRows)
                {
                    while (conexionMySql.consulta.Read())
                    {
                        correo = conexionMySql.consulta.GetString(2).ToString();
                        Password = conexionMySql.consulta.GetString(1).ToString();
                        user = conexionMySql.consulta.GetString(0).ToString();

                        string cAsunto = "Recuperacion de Contraseña";
                        string Body = "Estimado Usuario :" + user.ToString()+ "\n\n";
                        Body += "La contraseña que le pertenece a su usario es la siguiente: " + Password.ToString() +"\n\n";
                        Body += "Atentamente" + "\n\n";
                        Body += "Equipo de Soporte IT";

                        SmtpClient client = new SmtpClient("smtp.Outlook.office365.com", 587);
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential("Proveeduria.intcomexgt@xgbsa.com", "Despacho.2022$$S", "Operaciones");
                        
                        MailMessage message = new MailMessage("Proveeduria.intcomexgt@xgbsa.com", correo, cAsunto, Body);

                        try
                        {
                            client.Send(message);
                        }
                        catch (System.Net.Mail.SmtpException ex)
                        {
                            string cResulttError = "false";
                            string strresult = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResulttError + "\", \"OBS\": \"" + ex.Message.ToString() + "\"}";
                            return strresult;
                        }
                    }
                
                }
                else
                {
                    conexionMySql.CerrarConexion();
                    return "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"false\", \"OBS\": \"El usuario no se encuentra registrado en la aplicación\"}";
                }

                conexionMySql.CerrarConexion();

                return "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"true\", \"OBS\": \"true\"}";

            }
            catch (Exception ex)
            {
                string cResulttError = "false";
                string strresult = "{\"CODIGO\": \"ERROR\", \"RESULTADO\": \"" + cResulttError + "\", \"OBS\": \"" + ex.Message.ToString() + "\"}";
                return strresult;
            }
        }
    }
}
