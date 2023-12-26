using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EqCrm.Controllers
{
    public class EmpresaLineaNegocioController : Controller
    {
        // GET: EmpresaLineaNegocio
        public ActionResult EmpresaLineaNegocio()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }

        [HttpPost]
        public string InsertarLineaNegocio(DatosEmpresas oEmpresa)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";
            string strconexion = "server=localhost;Port=3306;user id=root;password=Clave01*;persistsecurityinfo=True;Allow User Variables=True;database=" + oEmpresa.BaseDatos;

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            DB += ";convertzerodatetime=true;";
            string DBCreate = strconexion;
            DBCreate += ";convertzerodatetime=true;";
            string cRuta = @"C:\NuevaEmpresa\Inicializar.sql";
            string cRuta2 = @"C:\NuevaEmpresa\"+oEmpresa.BaseDatos+".sql";
            Funciones f = new Funciones();
                                

                        

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(cRuta2))
            {

                StreamReader objReader;
                string sLine = "";                

               
                    objReader = new StreamReader(cRuta);

                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                        sw.WriteLine(sLine.Replace("nuevaempresa", oEmpresa.BaseDatos)); 
                    }
                    objReader.Close();
                               
                
                sw.Flush();
                sw.Close();
            }



            try
            {

                string cInst = "INSERT INTO dlempresa.empresa ";
                cInst += "( nombre_empresa,base_datos,stringconexion) ";
                cInst += "VALUES ( ";
                cInst += "'" + oEmpresa.Empresa + "',";
                cInst += "'" + oEmpresa.BaseDatos + "',";
                cInst += "'" + strconexion + "')";

                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }

                
                
                
                    using (MySqlConnection conn = new MySqlConnection(DB))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();

                            
                                //mb.ImportFromStream(mStrm);
                                mb.ImportFromFile(cRuta2);
                               
                                conn.Close();
                            }
                        }
                    }
                    
                

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"REGISTRO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }

        [HttpPost]
        public string InsertarEmpresa(string jSonEmpresa)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";
            

            DatosEmpresa datos = JsonConvert.DeserializeObject<DatosEmpresa>(jSonEmpresa);
            datos.cfrase = Funciones.Base64Decode(datos.cfrase);


            string strconexion = "server=localhost;Port=3306;user id=root;password=Clave01*;persistsecurityinfo=True;Allow User Variables=True;database=" + datos.basedatos;

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL insertar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];
            DB += ";convertzerodatetime=true;";
            string DBCreate = strconexion;
            DBCreate += ";convertzerodatetime=true;";
            string cRuta = @"C:\NuevaEmpresa\Inicializar.sql";
            string cRuta2 = @"C:\NuevaEmpresa\" + datos.basedatos + ".sql";
            Funciones f = new Funciones();




            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(cRuta2))
            {

                StreamReader objReader;
                string sLine = "";


                objReader = new StreamReader(cRuta);

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        sw.WriteLine(sLine.Replace("nuevaempresa", datos.basedatos));
                }
                objReader.Close();


                sw.Flush();
                sw.Close();
            }


            //string cInst = String.Format("SELECT IFNULL(max({0}), 0) AS ID", "id_empresa");
            //cInst += String.Format("FROM {0} ", "dlempresa.empresa");

            int nEmpresa=Funciones.NumeroMayor("dlempresa.empresa","id_empresa","", DB);
            

            try
            {

               string cInst = "INSERT INTO dlempresa.empresa ";
                cInst += "( id_empresa,nombre_empresa,base_datos,nit,nombre_comercial,direccion,depto,municipio,requestorfe,cToken,cUserfe,autorizacion,wslfe,apifel,cCampo1,AfiliacionIVA,certificador,cTipoPersoneria,stringconexion) ";
                cInst += "VALUES ( ";
                cInst += "'" + nEmpresa.ToString() + "',";
                cInst += "'" + datos.empresa.Trim() + "',";
                cInst += "'" + datos.basedatos.Trim() + "',";
                cInst += "'" + datos.nit.Trim() + "',";
                cInst += "'" + datos.ncomercial.Trim() + "',";
                cInst += "'" + datos.direccion.Trim() + "',";
                cInst += "'" + datos.depto.Trim() + "',";
                cInst += "'" + datos.municipio.Trim() + "',"; 
                cInst += "'" + datos.token.Trim() + "',";
                cInst += "'" + datos.token.Trim() + "',";
                cInst += "'" + datos.user.Trim() + "',";
                cInst += "'" + datos.cAutorizacion + "',";
                cInst += "'" + datos.curl.Trim() + "',";
                cInst += "'" + datos.curl.Trim() + "',";
                cInst += "'" + datos.cfrase.Trim() + "',";
                cInst += "'" + datos.afilia.Trim() + "',";
                cInst += "'" + datos.certificador.Trim() + "',";
                cInst += "'" + datos.personeria.Trim() + "',";

                cInst += "'" + strconexion.Trim() + "')";

                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"MENSAJE\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }



                ///////////////// con este segmento de programamcion generaremos un rol automatico para nuevos usuarios
                cInst = "INSERT INTO dlempresa.usuarios(usuario,admin,nombre,autorizacion,id_vendedor,password) VALUES ('" + datos.UsuarioNit + "','','" + datos.UsuarioNit + "','"+datos.cAutorizacion+"',0,'')";
                
                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    return cError;
                }


                // vamos a generar todo para generar el rol y usuario automaticamente
                cInst = "INSERT INTO dlempresa.usuario_categorias(usuario, id_categoria, estado)";
                cInst += " SELECT 'REEMPLAZAR',id,0  FROM categorias WHERE id not in (SELECT id_categoria FROM usuario_categorias WHERE  usuario = 'REEMPLAZAR') ";
                cInst = cInst.Replace("REEMPLAZAR", datos.UsuarioNit   );
                insertar.EjecutarCommando(cInst, DB);



                cInst = "INSERT INTO dlempresa.usuario_menus(usuario, id_menu, estado)";
                cInst += " SELECT 'REEMPLAZAR',id,0  FROM menus WHERE id not in (SELECT id_menu FROM usuario_menus WHERE  usuario = 'REEMPLAZAR') ";
                cInst = cInst.Replace("REEMPLAZAR", datos.UsuarioNit);

                insertar.EjecutarCommando(cInst, DB);


                cInst = "INSERT INTO dlempresa.usuario_empresa(usuario, id_empresa, estado)";
                cInst += " SELECT 'REEMPLAZAR',id_empresa,0  FROM empresa WHERE id_empresa not in (SELECT id_empresa FROM usuario_empresa WHERE  usuario = 'REEMPLAZAR') ";
                cInst = cInst.Replace("REEMPLAZAR", datos.UsuarioNit);
                insertar.EjecutarCommando(cInst, DB);


                // vamos actualizar el rol para hacerlo que pertenece a la empresa que se esta creando
                if (datos.cTipoConsola == "FACTURADOR")
                {
                    cInst = "UPDATE dlempresa.usuario_categorias SET autorizacion = '" + datos.cAutorizacion + "',estado=1  WHERE usuario ='" + datos.UsuarioNit + "' AND id_categoria IN(1,7,8,13,4)";
                    insertar.EjecutarCommando(cInst, DB);
                }
                else
                {
                    cInst = "UPDATE dlempresa.usuario_categorias SET autorizacion = '" + datos.cAutorizacion + "',estado=1  WHERE usuario ='" + datos.UsuarioNit ;
                    insertar.EjecutarCommando(cInst, DB);
                }

                if (datos.cTipoConsola == "FACTURADOR")
                {
                    cInst = "UPDATE dlempresa.usuario_menus SET autorizacion = '" + datos.cAutorizacion + "',estado=1  WHERE usuario ='" + datos.UsuarioNit + "' AND id_menu IN(1,7,4,11,12,13,20,21,22,51,80,84)";
                    insertar.EjecutarCommando(cInst, DB);
                }
                else
                {
                    cInst = "UPDATE dlempresa.usuario_menus SET autorizacion = '" + datos.cAutorizacion + "',estado=1  WHERE usuario ='" + datos.UsuarioNit;
                    insertar.EjecutarCommando(cInst, DB);
                }

                cInst = "UPDATE dlempresa.usuario_empresa SET estado=1  WHERE id_empresa =" + nEmpresa.ToString();
                insertar.EjecutarCommando(cInst, DB);

                cInst = "INSERT INTO dlempresa.usuarios_web(usuario,nombre,password,rol,autorizacion,id_sucursal) VALUES ('" + datos.UsuarioNit + "','" + datos.ncomercial + "'," + "'" + datos.Password + "','" + datos.UsuarioNit + "','"+datos.cAutorizacion+"', 1)";
                insertar.EjecutarCommando(cInst, DB);






                using (MySqlConnection conn = new MySqlConnection(DB))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();

                            mb.ImportFromFile(cRuta2);

                            conn.Close();
                        }
                    }
                }

            str = "";

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"MENSAJE\": \"" + ex.Message.ToString() + "\", \"VALOR\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"MENSAJE\": \"REGISTRO GUARDADO \", \"VALOR\": 0}";

            
            return str;
        }





        [HttpPost]
        public string ConsultaLineaNegocios()
        {
            StringConexionMySQL lectura = new StringConexionMySQL();
            DataTable dt = new DataTable();
            string cResultado = "";
            string Base_Datos = (string)(Session["StringConexion"]);
            string instruccionSQL = "";


            instruccionSQL = "SELECT id_empresa AS ID,nombre_empresa AS LINEANEGOCIO FROM dlempresa.empresa";


            //cResultado = lectura.ScriptLlenarDTTableHTml(instruccionSQL, dt, Base_Datos).ToString();
            cResultado = lectura.ScriptLlenarDTTableHTmlSinFiltro(instruccionSQL, dt, Base_Datos).ToString();

            lectura.CerrarConexion();

            return cResultado;




        }
    }
}