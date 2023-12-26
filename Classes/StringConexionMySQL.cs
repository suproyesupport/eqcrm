using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Threading;
using DevExpress.XtraRichEdit.Import.Rtf;
using System.Web;
using DevExpress.DataProcessing;
using Microsoft.Ajax.Utilities;
using EqCrm.Models;
using DevExpress.Data.Helpers;
using System.Collections.ObjectModel;
using EqCrm.Models.POS;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;

namespace EqCrm
{
    public class StringConexionMySQL
    {
        // Definimos las variables publicas que se utilizaran en el sistema
        public MySqlConnection conexion;
        public MySqlDataAdapter daSQL;
        public MySqlCommandBuilder cbSQL;
        public MySqlCommand comando;
        public MySqlDataReader consulta = null;

        public ObservableCollection<Cotizacion> CotizacionList { get; private set; }


        /// <summary>
        /// Procedimiento que abre y verifica la conexion al servidor de datos
        /// </summary>
        /// <param name="Base_de_Datos">Nombre de la base de datos a consultar</param>
        /// <returns>Devolvera el numero de error si este existiera, 0 si no hay error</returns>
        public int AbrirConexionServidor(string Base_de_Datos)
        {
            // Creamos una variable de cadena en blanco para guardar la instruccion de conexion al servidor
            string Cadena_Conexion = "";



            // Cadena de conexion con base de datos
            Cadena_Conexion = Base_de_Datos;


            // Inicializamos la variable para la conexion
            conexion = new MySqlConnection(Cadena_Conexion);

            // Bloque para verificar si existira error en la conexion
            try
            {
                if (conexion.State == System.Data.ConnectionState.Closed)
                {
                    conexion.Open();
                }
            }
            catch (MySqlException ex)           // Si hay error se ejecutaran las instrucciones siguientes
            {
                // Cerramos la conexion al servidor
                //conexion.Close();

                string mensaje = "";            // Cadena para guardar el mensaje que se mostrara en el error
                string mostrar_mensaje = "S";   // Definimos si se mostrara el mensaje de error

                // Verificamos los diferentes errores que pueden suceder
                switch (ex.Number)
                {
                    case 1045:
                        {
                            mensaje = "El usuario ingresado no existe o la contrase�a es incorrecta";
                            break;
                        }
                    case 1049:      // No existe la Base de Datos
                        {
                            // mensaje = "No se encuentra la base de datos [" + Base_de_Datos + "]";
                            mostrar_mensaje = "N";
                            break;
                        }
                    case 0:
                        {
                            //  mensaje = "No se puede encontrar el servidor especifico [" + Servidor + "]";
                            break;
                        }
                    default:
                        {
                            mensaje = String.Concat(ex.Message, " - ", ex.Number.ToString());
                            break;
                        }
                }

                // Si la variable contiene el valor "S" se mostrara el mensaje de error
                if (mostrar_mensaje == "S")
                    //  MessageBox.Show(mensaje, "Error de conexi�n", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // No existe numero de error cuando no existe el servidor, por eso se regresa un -1
                    if (ex.Number == 0)
                        return -1;
                    else
                        return ex.Number;   // Regresamos el numero de error
            }

            return 0;   // Regresamos 0 si no existe error
        }


        public void EqReplica(string sentenciaSQL, string DB)
        {
            string cUser = (string)System.Web.HttpContext.Current.Session["Usuario"]; // (string)(Session["Usuario"]);

            string query = "INSERT INTO eqreplica (id_agencia, fecha, hora, hechopor, sqltxt, status, error) VALUES (";
            query += "100, CURDATE(), CURTIME(), ";
            query += "\"" + cUser + "\", ";
            query += "\"" + sentenciaSQL + "\", ";
            query += "1, ";
            query += "\" \" );";


            try
            {
                using (conexion = new MySqlConnection(DB))
                {
                    // Abrir la conexión
                    conexion.Open();

                    using (comando = new MySqlCommand(query, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();


                }

            }
            catch (MySqlException ex)
            {
                conexion.Close();

            }
        }





        public static byte[] convertirAvatarAByte(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] avatar = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return avatar;
        }



        /// <summary>
        /// Procedimiento para ejecurtar consultas e instrucciones SQL
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (ej. INSERT, DELETE, UPDATE)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        /// <returns>Devolvera un valor Falso si no existio error en al ejecucion de la sentencia, en caso contrario Verdadero</returns>
        public bool EjecutarCommando(string sentenciaSQL, string DB)
        {

            try
            {
                using (conexion = new MySqlConnection(DB))
                {
                    // Abrir la conexión
                    conexion.Open();


                    using (comando = new MySqlCommand(sentenciaSQL, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                    
                    return false;
                }

            }
            catch 
            {
                
                conexion.Close();
                return true;
            }

        }


        



        ////////public bool EjecutarCommando(string sentenciaSQL, string DB)
        ////////{

        ////////    // Abrimos la conexion y capturamos el error
        ////////    int numero_error = AbrirConexionServidor(DB);

        ////////    // Si no hay error ejecutarmos la consulta
        ////////    if (numero_error == 0)
        ////////    {
        ////////        // Instruccion para capturar si existe error en el comando que se ejecutara
        ////////        try
        ////////        {
        ////////            // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
        ////////            comando = new MySqlCommand(sentenciaSQL, conexion);

        ////////            // Ejecutamos el comando
        ////////            comando.ExecuteNonQuery();



        ////////            // Cerramos la conexion al servidor
        ////////            CerrarConexion();



        ////////            // Devolvemos un valor falso para informar que no existe error
        ////////            return false;
        ////////        }
        ////////        // Capturamos el error y lo controlamos para mostrar el mensaje especifico del error
        ////////        catch //(MySqlException ex)
        ////////        {
        ////////            // Mostramos el mensaje de error
        ////////            // MessageBox.Show(ex.Message.Substring(6), "Error No. " + ex.Number.ToString());

        ////////            // Mostramos un mensaje con la sentencia ingresada
        ////////            //MessageBox.Show(sentenciaSQL, "Informaci�n...");

        ////////            // Cerramos la conexion al servidor


        ////////            CerrarConexion();

        ////////            // Devolvemos un valor para indicar que existio un error
        ////////            return true;
        ////////        }
        ////////    }
        ////////    else
        ////////    {
        ////////        // Devolvemos un valor para indicar que existio un error

        ////////        return true;
        ////////    }
        ////////}



        ////////public bool ExecCommand(string sentenciaSQL, string DB, ref string cError)
        ////////{
        ////////    // Abrimos la conexion y capturamos el error
        ////////    int numero_error = AbrirConexionServidor(DB);

        ////////    // Si no hay error ejecutarmos la consulta
        ////////    if (numero_error == 0)
        ////////    {
        ////////        // Instruccion para capturar si existe error en el comando que se ejecutara
        ////////        try
        ////////        {
        ////////            // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
        ////////            comando = new MySqlCommand(sentenciaSQL, conexion);

        ////////            // Ejecutamos el comando
        ////////            comando.ExecuteNonQuery();

        ////////            // Cerramos la conexion al servidor
        ////////            CerrarConexion();

        ////////            cError = "";

        ////////            // Devolvemos un valor falso para informar que no existe error
        ////////            return false;
        ////////        }
        ////////        // Capturamos el error y lo controlamos para mostrar el mensaje especifico del error
        ////////        catch (MySqlException ex)
        ////////        {
        ////////            // Mostramos el mensaje de error
        ////////            // MessageBox.Show(ex.Message.Substring(6), "Error No. " + ex.Number.ToString());

        ////////            // Mostramos un mensaje con la sentencia ingresada
        ////////            //MessageBox.Show(sentenciaSQL, "Informaci�n...");

        ////////            // Cerramos la conexion al servidor

        ////////            cError = "Error No. " + ex.Number.ToString() + "  " + ex.Message.ToString() + "  " + sentenciaSQL;
        ////////            CerrarConexion();

        ////////            // Devolvemos un valor para indicar que existio un error
        ////////            return true;
        ////////        }
        ////////    }
        ////////    else
        ////////    {
        ////////        // Devolvemos un valor para indicar que existio un error
        ////////        cError = "";
        ////////        return true;
        ////////    }
        ////////}



        public bool ExecCommand(string sentenciaSQL, string DB, ref string cError)
        {
            // Abrimos la conexion y capturamos el error


            try
            {
                using (conexion = new MySqlConnection(DB))
                {
                    // Abrir la conexión
                    conexion.Open();


                    using (comando = new MySqlCommand(sentenciaSQL, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                    EqReplica(sentenciaSQL, DB);
                    cError = "";
                    return false;
                }

            }
            catch (MySqlException ex)
            {
                cError = "Error No. " + ex.Number.ToString() + "  " + ex.Message.ToString() + "  " + sentenciaSQL;
                conexion.Close();
                return true;
            }


        }



        public bool ExecAnotherCommand(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    comando.ExecuteNonQuery();
                    
                    // Cerramos la conexion al servidor
                    CerrarConexion();


                    EqReplica(sentenciaSQL, DB);
                    // Devolvemos un valor falso para informar que no existe error
                    return false;
                }
                // Capturamos el error y lo controlamos para mostrar el mensaje especifico del error
                catch //(MySqlException ex)
                {
                    // Mostramos el mensaje de error
                    // MessageBox.Show(ex.Message.Substring(6), "Error No. " + ex.Number.ToString());

                    // Mostramos un mensaje con la sentencia ingresada
                    //MessageBox.Show(sentenciaSQL, "Informaci�n...");

                    // Cerramos la conexion al servidor

                    CerrarConexion();

                    // Devolvemos un valor para indicar que existio un error
                    return true;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error

                return true;
            }
        }
        /// <summary>
        /// Procedimiento para ejecutar comandos e instrucciones SQL
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (ej. INSERT, DELETE, UPDATE)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        /// <param name="AbrirConexion">Verdadero si la funcion abrira la conexion, en otro caso Falso</param>
        /// <returns>Devolvera un valor Falso si no existio error en al ejecucion de la sentencia, en caso contrario Verdadero</returns>
        public bool EjecutarCommando(string sentenciaSQL, string DB, bool AbrirConexion, ref string cError)
        {

            AbrirConexion = true;
            try
            {
                StringConexionMySQL eqreplica = new StringConexionMySQL();
                using (conexion = new MySqlConnection(DB))
                {
                    // Abrir la conexión
                    conexion.Open();


                    using (comando = new MySqlCommand(sentenciaSQL, conexion))
                    {
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                    cError = "";
                    EqReplica(sentenciaSQL, DB);
                    return false;
                }




            }
            catch (MySqlException ex)
            {
                cError = "Error No. " + ex.Number.ToString() + "  " + ex.Message.ToString() + "  " + sentenciaSQL;
                conexion.Close();
                return true;
            }





        }

        /// <summary>
        /// Procedimiento para ejecutar procedimientos almacenados
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</para
        public bool EjecutarStoreProcedure(string namestoreprocedure, string DB)
        {

            try
            {
                // Abrimos la conexion
                int numero_error = AbrirConexionServidor(DB);

                if (numero_error == 0)
                {
                    comando = new MySqlCommand(namestoreprocedure, conexion);
                    string procedimiento = namestoreprocedure.ToString();
                    comando.CommandType = CommandType.StoredProcedure;
                    // Ejecutamos el comando
                    comando.ExecuteNonQuery();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }

        }
        //STORED PROCEDURE ELABORADO POR SISGT
        public bool EjecutarStoredProcedure(string namestoreprocedure, string DB, List<String> listado, ref string cError)
        {
            // = "";
            try
            {
                // Abrimos la conexion
                int numero_error = AbrirConexionServidor(DB);

                if (numero_error == 0)
                {
                    
                    string procedimiento = namestoreprocedure.ToString();
                    comando = new MySqlCommand(procedimiento, conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    // Ejecutamos el comando
                    switch (procedimiento)
                    {
                        case "ventaPOS_insertar_facturas":
                            comando.Parameters.Add("@no_factura", MySqlDbType.Double).Value = listado[0].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[1].ToString();
                            comando.Parameters.Add("@fecha", MySqlDbType.Date).Value = listado[2].ToString();
                            comando.Parameters.Add("@status", MySqlDbType.VarChar).Value = listado[3].ToString();
                            //comando.Parameters.Add("@id_cliente", MySqlDbType.Int32).Value = listado[4].ToString();
                            comando.Parameters.Add("@id_cliente", MySqlDbType.Int64).Value = listado[4].ToString();
                            comando.Parameters.Add("@cliente", MySqlDbType.VarChar).Value = listado[5].ToString();
                            //comando.Parameters.Add("@id_vendedor", MySqlDbType.Int32).Value = listado[6].ToString();
                            comando.Parameters.Add("@id_vendedor", MySqlDbType.Int64).Value = listado[6].ToString();
                            comando.Parameters.Add("@direccion", MySqlDbType.Text).Value = listado[7].ToString();
                            comando.Parameters.Add("@total", MySqlDbType.Double).Value = listado[8].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[9].ToString();
                            //comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[10].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int64).Value = listado[10].ToString();
                            comando.Parameters.Add("@nit", MySqlDbType.VarChar).Value = listado[11].ToString();
                            comando.Parameters.Add("@dias", MySqlDbType.Int64).Value = listado[12].ToString();
                            comando.Parameters.Add("@tdescto", MySqlDbType.Double).Value = listado[13].ToString();
                            comando.Parameters.Add("@cont_cred", MySqlDbType.Int64).Value = listado[14].ToString();
                            comando.Parameters.Add("@id_caja", MySqlDbType.Int64).Value = listado[15].ToString();
                            comando.Parameters.Add("@efectivo", MySqlDbType.Double).Value = listado[16].ToString();
                            comando.Parameters.Add("@tarjeta", MySqlDbType.Double).Value = listado[17].ToString();
                            comando.Parameters.Add("@cheque", MySqlDbType.Double).Value = listado[18].ToString();
                            comando.Parameters.Add("@vale", MySqlDbType.Double).Value = listado[19].ToString();
                            comando.Parameters.Add("@transferencia", MySqlDbType.Double).Value = listado[20].ToString();


                            break;
                        case "ventaPOS_insertar_ctacc":
                            comando.Parameters.Add("@id_codigo", MySqlDbType.Int32).Value = listado[0].ToString();
                            comando.Parameters.Add("@docto", MySqlDbType.Double).Value = listado[1].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[2].ToString();
                            comando.Parameters.Add("@id_movi", MySqlDbType.Int32).Value = listado[3].ToString();
                            comando.Parameters.Add("@fechaa", MySqlDbType.Date).Value = listado[4].ToString();
                            comando.Parameters.Add("@importe", MySqlDbType.Double).Value = listado[5].ToString();
                            comando.Parameters.Add("@saldo", MySqlDbType.Double).Value = listado[5].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[6].ToString();
                            comando.Parameters.Add("@hechopor", MySqlDbType.VarChar).Value = listado[7].ToString();
                            comando.Parameters.Add("@tipodocto", MySqlDbType.VarChar).Value = listado[8].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[9].ToString();
                            comando.Parameters.Add("@dias", MySqlDbType.Int32).Value = listado[10].ToString();
                            break;
                        case "ventaPOS_insertar_detfacturas":                              
                            comando.Parameters.Add("@no_factura", MySqlDbType.Double).Value = listado[0].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[1].ToString();
                            comando.Parameters.Add("@id_codigo", MySqlDbType.Int32).Value = listado[2].ToString();
                            comando.Parameters.Add("@cantidad", MySqlDbType.Double).Value = listado[3].ToString();
                            comando.Parameters.Add("@precio", MySqlDbType.Double).Value = listado[4].ToString();
                            comando.Parameters.Add("@subtotal", MySqlDbType.Double).Value = listado[5].ToString();
                            comando.Parameters.Add("@no_cor", MySqlDbType.VarChar).Value = listado[6].ToString();
                            comando.Parameters.Add("@tdescto", MySqlDbType.Double).Value = listado[7].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[8].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[9].ToString();
                            break;
                        case "ventaPOS_insertar_kardexinven":
                            comando.Parameters.Add("@id_codigo", MySqlDbType.Int32).Value = listado[0].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[1].ToString();
                            comando.Parameters.Add("@fecha", MySqlDbType.Date).Value = listado[2].ToString();
                            comando.Parameters.Add("@id_movi", MySqlDbType.Int32).Value = listado[3].ToString();
                            comando.Parameters.Add("@docto", MySqlDbType.VarChar).Value = listado[4].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[5].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[6].ToString();
                            comando.Parameters.Add("@hechopor", MySqlDbType.VarChar).Value = listado[7].ToString();
                            comando.Parameters.Add("@salida", MySqlDbType.Double).Value = listado[8].ToString();
                            comando.Parameters.Add("@costo1", MySqlDbType.Double).Value = listado[9].ToString();
                            comando.Parameters.Add("@precio", MySqlDbType.Double).Value = listado[10].ToString();
                            comando.Parameters.Add("@codigoemp", MySqlDbType.Int32).Value = listado[11].ToString();
                            comando.Parameters.Add("@correlativo", MySqlDbType.Int32).Value = listado[12].ToString();
                            break;
                        case "ventaPOS_asignar_cajas":
                            comando.Parameters.Add("@id_usuario", MySqlDbType.String).Value = listado[1].ToString();
                            comando.Parameters.Add("@id_caja", MySqlDbType.Int32).Value = listado[0].ToString();
                            break;
                        case "ELIMINAR_VENTA_POS_FELWEB":
                            comando.Parameters.Add("@no_factura_ent", MySqlDbType.Double).Value = listado[0].ToString();
                            comando.Parameters.Add("@serie_ent", MySqlDbType.VarChar).Value = listado[1].ToString();
                            break;
                        case "ventaPOS_insertar_cortecaja":
                            comando.Parameters.Add("@fecha", MySqlDbType.Date).Value = listado[0].ToString();
                            comando.Parameters.Add("@efectivocontado", MySqlDbType.Double).Value = listado[1].ToString();
                            comando.Parameters.Add("@tarjetacontado", MySqlDbType.Double).Value = listado[2].ToString();
                            comando.Parameters.Add("@chequecontado", MySqlDbType.Double).Value = listado[3].ToString();
                            comando.Parameters.Add("@valecontado", MySqlDbType.Double).Value = listado[4].ToString();
                            comando.Parameters.Add("@efectivocalculado", MySqlDbType.Double).Value = listado[5].ToString();
                            comando.Parameters.Add("@tarjetacalculado", MySqlDbType.Double).Value = listado[6].ToString();
                            comando.Parameters.Add("@chequecalculado", MySqlDbType.Double).Value = listado[7].ToString();
                            comando.Parameters.Add("@valecalculado", MySqlDbType.Double).Value = listado[8].ToString();
                            comando.Parameters.Add("@efectivodiferencia", MySqlDbType.Double).Value = listado[9].ToString();
                            comando.Parameters.Add("@tarjetadiferencia", MySqlDbType.Double).Value = listado[10].ToString();
                            comando.Parameters.Add("@chequediferencia", MySqlDbType.Double).Value = listado[11].ToString();
                            comando.Parameters.Add("@valediferencia", MySqlDbType.Double).Value = listado[12].ToString();
                            comando.Parameters.Add("@efectivoretiro", MySqlDbType.Double).Value = listado[13].ToString();
                            comando.Parameters.Add("@tarjetaretiro", MySqlDbType.Double).Value = listado[14].ToString();
                            comando.Parameters.Add("@chequeretiro", MySqlDbType.Double).Value = listado[15].ToString();
                            comando.Parameters.Add("@valeretiro", MySqlDbType.Double).Value = listado[16].ToString();
                            comando.Parameters.Add("@uuidcaja", MySqlDbType.String).Value = listado[17].ToString();
                            comando.Parameters.Add("@id_caja", MySqlDbType.Int32).Value = listado[18].ToString();
                            break;
                        case "insertar_compras":
                            comando.Parameters.Add("@no_factura", MySqlDbType.Double).Value = listado[0].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[1].ToString();
                            comando.Parameters.Add("@fecha", MySqlDbType.Date).Value = listado[2].ToString();
                            comando.Parameters.Add("@status", MySqlDbType.VarChar).Value = listado[3].ToString();
                            comando.Parameters.Add("@id_cliente", MySqlDbType.Int32).Value = listado[4].ToString();
                            comando.Parameters.Add("@cliente", MySqlDbType.VarChar).Value = listado[5].ToString();                            
                            comando.Parameters.Add("@direccion", MySqlDbType.Text).Value = listado[7].ToString();
                            comando.Parameters.Add("@total", MySqlDbType.Double).Value = listado[8].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[9].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[10].ToString();
                            comando.Parameters.Add("@nit", MySqlDbType.VarChar).Value = listado[11].ToString();
                            comando.Parameters.Add("@dias", MySqlDbType.Int32).Value = listado[12].ToString();
                            comando.Parameters.Add("@tdescto", MySqlDbType.Double).Value = listado[13].ToString();
                            comando.Parameters.Add("@cont_cred", MySqlDbType.Int32).Value = listado[14].ToString();
                            break;
                        case "insertar_detfacturas":
                            comando.Parameters.Add("@no_factura", MySqlDbType.Double).Value = listado[0].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[1].ToString();
                            comando.Parameters.Add("@id_codigo", MySqlDbType.Int32).Value = listado[2].ToString();
                            comando.Parameters.Add("@cantidad", MySqlDbType.Double).Value = listado[3].ToString();
                            comando.Parameters.Add("@precio", MySqlDbType.Double).Value = listado[4].ToString();
                            comando.Parameters.Add("@subtotal", MySqlDbType.Double).Value = listado[5].ToString();
                            comando.Parameters.Add("@no_cor", MySqlDbType.VarChar).Value = listado[6].ToString();
                            comando.Parameters.Add("@tdescto", MySqlDbType.Double).Value = listado[7].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[8].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[9].ToString();
                            break;
                        case "insertar_kardexinven_entrada":
                            comando.Parameters.Add("@id_codigo", MySqlDbType.Int32).Value = listado[0].ToString();
                            comando.Parameters.Add("@id_agencia", MySqlDbType.Int32).Value = listado[1].ToString();
                            comando.Parameters.Add("@fecha", MySqlDbType.Date).Value = listado[2].ToString();
                            comando.Parameters.Add("@id_movi", MySqlDbType.Int32).Value = listado[3].ToString();
                            comando.Parameters.Add("@docto", MySqlDbType.VarChar).Value = listado[4].ToString();
                            comando.Parameters.Add("@serie", MySqlDbType.VarChar).Value = listado[5].ToString();
                            comando.Parameters.Add("@obs", MySqlDbType.Text).Value = listado[6].ToString();
                            comando.Parameters.Add("@hechopor", MySqlDbType.VarChar).Value = listado[7].ToString();
                            comando.Parameters.Add("@entrada", MySqlDbType.Double).Value = listado[8].ToString();
                            comando.Parameters.Add("@costo1", MySqlDbType.Double).Value = listado[9].ToString();
                            comando.Parameters.Add("@precio", MySqlDbType.Double).Value = listado[10].ToString();
                            comando.Parameters.Add("@codigoemp", MySqlDbType.Int32).Value = listado[11].ToString();
                            comando.Parameters.Add("@correlativo", MySqlDbType.Int32).Value = listado[12].ToString();
                            break;
                    }



                    comando.ExecuteNonQuery();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(MySqlException ex)
            {
                cError = ex.Message;
                return true;
            }

        }



        /// <summary>
        /// Procedimiento para llenar data table de procedimientos almacenados
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</para
        public DataTable SelectDataTableFromStoreProcedure(string namestoreprocedure, DataTable dt, string DB)
        {

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            if (numero_error == 0)
            {
                comando = new MySqlCommand(namestoreprocedure, conexion);
                comando.CommandType = CommandType.StoredProcedure;
                daSQL = new MySqlDataAdapter();
                daSQL.SelectCommand = comando;
                daSQL.Fill(dt);

            }

            return dt;
        }


        public List<tickets> SegTickets(string sentenciaSQL, string DB, List<tickets> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<tickets>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ClientesFac> ClientesFac(string sentenciaSQL, string DB, List<ClientesFac> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ClientesFac>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        public List<OrdenServicioFact> ListOrdenServicioFacturar(string sentenciaSQL, string DB, List<OrdenServicioFact> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<OrdenServicioFact>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        public List<FacturasNC> ListFacturasNC(string sentenciaSQL, string DB, List<FacturasNC> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<FacturasNC>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        public List<ListaPrecioGAS> ListadoPreciosGAS(string sentenciaSQL, string DB, List<ListaPrecioGAS> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaPrecioGAS>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ListaTicketProblema> ListadoProblemaTicket(string sentenciaSQL, string DB, List<ListaTicketProblema> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaTicketProblema>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ListaClienteTicket> ListadoClienteTicket(string sentenciaSQL, string DB, List<ListaClienteTicket> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaClienteTicket>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        //ORDENTECNICA coca
        public List<ListaOrdenTecnica> ListaOrdenTecnica(string sentenciaSQL, string DB, List<ListaOrdenTecnica> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaOrdenTecnica>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }

        //ORDENTECNICA coca
        public List<frasesexentas> ListaFrases(string sentenciaSQL, string DB, List<frasesexentas> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                
                    db.Open();
                    listado = db.Query<frasesexentas>(sentenciaSQL, commandType: CommandType.Text).ToList();                
                    db.Close();
                
            }
            return listado;
        }


        public List<ListaMovKardex> ListadoMovKardex(string sentenciaSQL, string DB, List<ListaMovKardex> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaMovKardex>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }



        public List<ListaTipoTicketProblema> ListadoTipoProblemaTicket(string sentenciaSQL, string DB, List<ListaTipoTicketProblema> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaTipoTicketProblema>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        /// <summary>
        /// Procedimiento para ejecutar lecturas de datos con Dapper
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        ///        
        public List<Encabezado_Documentos> ObtieneEncabezado(string sentenciaSQL, string DB, List<Encabezado_Documentos> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<Encabezado_Documentos>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        /// <summary>
        /// Procedimiento para ejecutar lecturas de datos con Dapper
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        ///        
        public List<Detalle_Documentos> ObtieneDetalle(string sentenciaSQL, string DB, List<Detalle_Documentos> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<Detalle_Documentos>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<AbcProductos> AbcProductos(string sentenciaSQL, string DB, List<AbcProductos> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<AbcProductos>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ListaInventario> ListaIventario(string sentenciaSQL, string DB, List<ListaInventario> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaInventario>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ListaDocumentos> ListaDocumentos(string sentenciaSQL, string DB, List<ListaDocumentos> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaDocumentos>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        //LLENAR ORDENES DE SERVICIO EN TABLA DEVEXPRESS
        public List<ListaOrdenesServicio> ListaOrdenesServicio(string sentenciaSQL, string DB, List<ListaOrdenesServicio> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaOrdenesServicio>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }


        //REPORTE GENERAL DE FACTURAS EN TABLA DEVEXPRESS
        public List<RpoGenFacturas> ListaRpoGenFacturas(string sentenciaSQL, string DB, List<RpoGenFacturas> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<RpoGenFacturas>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }


        //REPORTE GENERAL DE FACTURAS EN TABLA DEVEXPRESS
        public List<RpoTrasEntreBodegas> ListaRpoTrasladoBodegas(string sentenciaSQL, string DB, List<RpoTrasEntreBodegas> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<RpoTrasEntreBodegas>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }


        //REPORTE LISTADO DE COBRADORES CV
        public List<RpoListadoDeCobradoresC> ListaRpoListadoCobradoresC(string sentenciaSQL, string DB, List<RpoListadoDeCobradoresC> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<RpoListadoDeCobradoresC>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }

        //CLIENTES
        public List<ListaClientes> ListaClientes(string sentenciaSQL, string DB, List<ListaClientes> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaClientes>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }

        //REPORTE LISTADO DE CLIENTES X SECTOR
        public List<ListaReporteClientesxSector> ListaRpoClientesxSector (string sentenciaSQL, string DB, List<ListaReporteClientesxSector> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaReporteClientesxSector>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }


        //REPORTE SABANA FACTURAS
        public List<RpoSabanaFact> ListaRpoSabanaFact(string sentenciaSQL, string DB, List<RpoSabanaFact> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<RpoSabanaFact>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }


        //ESTADISTICA VENTA PROD X CLIENTE
        public List<EstadisticaVentaProdxClie> ListaEstadisticaVentaProdxClie(string sentenciaSQL, string DB, List<EstadisticaVentaProdxClie> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<EstadisticaVentaProdxClie>(sentenciaSQL, commandType: CommandType.Text).ToList();
                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }



        //LISTA LICENCIAS 
        public List<ListaLicencias> ListaLincenciasEmpresa(string sentenciaSQL, string DB, List<ListaLicencias> listado)
        {
            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaLicencias>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return listado;
        }








        //PARA LLENAR LAS COTIZACIONES EN TABLA DE DevExpress
        public List<ListaCotizaciones> ListaCotizaciones(string sentenciaSQL, string DB, List<ListaCotizaciones> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaCotizaciones>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }
        public List<ListaCompras> ListaCompras(string sentenciaSQL, string DB, List<ListaCompras> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaCompras>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        public List<ListaOrdenes> ListaOrdenes(string sentenciaSQL, string DB, List<ListaOrdenes> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaOrdenes>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }
        //PARA LLENAR LAS COTIZACIONES EN TABLA DE DevExpress
        public List<ListaSegCotizaciones> SegCotizaciones(string sentenciaSQL, string DB, List<ListaSegCotizaciones> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaSegCotizaciones>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }
        public List<ListaVentas> ListaVentas(string sentenciaSQL, string DB, List<ListaVentas> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaVentas>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }

        public List<ListaCorte> ListaCorteCaja(string sentenciaSQL, string DB, List<ListaCorte> listado)
        {

            string Cadena_Conexion = DB;

            using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
            {

                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                    listado = db.Query<ListaCorte>(sentenciaSQL, commandType: CommandType.Text).ToList();

                }
                if (db.State == ConnectionState.Open)
                {
                    db.Close();

                }

            }
            return listado;
        }


        //////public List<SelectListItem> LLenarDropDownList(string sentenciaSQL, string DB, List<SelectListItem> lista)
        //////{
        //////    // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
        //////    int numero_error = AbrirConexionServidor(DB);

        //////    // Si no hay error ejecutamos la lectura de datos
        //////    if (numero_error == 0)
        //////    {
        //////        comando = new MySqlCommand(sentenciaSQL, conexion);
        //////        consulta = comando.ExecuteReader();

        //////        while (consulta.Read())
        //////        {
        //////            lista.Add(new SelectListItem { Text = consulta[1].ToString().Trim(), Value = consulta[0].ToString().Trim() });
        //////        }
        //////    }

        //////    return lista;
        //////}


        public string CurdateYear(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion            
            string year = "";
            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (comando = new MySqlCommand(sentenciaSQL, conexion))
                {
                    using (consulta = comando.ExecuteReader())
                    {
                        while (consulta.Read())
                        {
                            year = Convert.ToDateTime(consulta[0].ToString()).ToString("yyyy-MM-dd").PadRight(4);
                        }
                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            return year;
        }


        public string TotalFacturado(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion            
            string total = "";
            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (comando = new MySqlCommand(sentenciaSQL, conexion))
                {
                    using (consulta = comando.ExecuteReader())
                    {
                        if (consulta.Read())
                        {
                           // while (consulta.Read())
                           // {
                                total = consulta[0].ToString();
                           // }
                        }
                        else
                        {
                            total = "0.00";
                        }
                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            return total;
        }




        public bool EncuentraValor(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion            
            bool lEncontrado = false;
            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (comando = new MySqlCommand(sentenciaSQL, conexion))
                {
                    using (consulta = comando.ExecuteReader())
                    {
                        while (consulta.Read())
                        {
                            lEncontrado = true;
                        }
                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            return lEncontrado;
        }



        public List<SelectListItem> LLenarDropDownList(string sentenciaSQL, string DB, List<SelectListItem> lista)
        {
            
            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (comando = new MySqlCommand(sentenciaSQL, conexion))
                {
                    using (consulta = comando.ExecuteReader())
                    {
                        while (consulta.Read())
                        {

                            lista.Add(new SelectListItem { Text = consulta[1].ToString().Trim(), Value = consulta[0].ToString().Trim() });
                        }
                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            return lista;
        }



        public List<SelectListItem> LLenarDropDownListIncoterm(string sentenciaSQL, string DB, List<SelectListItem> lista)
        {
            // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                comando = new MySqlCommand(sentenciaSQL, conexion);
                consulta = comando.ExecuteReader();

                while (consulta.Read())
                {
                    lista.Add(new SelectListItem { Text = consulta[0].ToString().Trim() + " - " + consulta[1].ToString().Trim(), Value = consulta[0].ToString().Trim() });
                }
            }

            return lista;
        }

        public List<ApplicationTypeModel> LLenarDropDownListUsuarios(string sentenciaSQL, string DB, List<ApplicationTypeModel> lista)
        {
            // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                comando = new MySqlCommand(sentenciaSQL, conexion);
                consulta = comando.ExecuteReader();

                while (consulta.Read())
                {
                    lista.Add(new ApplicationTypeModel { nombre = consulta[1].ToString().Trim(), id = consulta[0].ToString().Trim() });
                }
            }

            return lista;
        }

        
        public bool InsertDapper(DynamicParameters parameters, string DB, string cTabla)
        {
            try
            {
                string Cadena_Conexion = DB;
                using (IDbConnection db = new MySqlConnection(Cadena_Conexion))
                {
                    if (db.State == ConnectionState.Closed)
                    {
                        db.Open();
                        db.Execute(cTabla, parameters, commandType: CommandType.TableDirect);

                    }
                    if (db.State == ConnectionState.Open)
                    {
                        db.Close();

                    }
                }
                return false;
            }
            catch
            {
                return true;
            }




        }
        /// <summary>
        /// Procedimiento para ejecutar lecturas de datos
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        public void EjecutarLectura(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                comando = new MySqlCommand(sentenciaSQL, conexion);
                consulta = comando.ExecuteReader();
            }
        }

        public string _EjecutarLectura(string sentenciaSQL, string DB)
        {
            try
            {
                if (this.AbrirConexionServidor(DB) == 0)
                {
                    this.comando = new MySqlCommand(sentenciaSQL, this.conexion);
                    this.consulta = this.comando.ExecuteReader();
                    return "true";
                }
                else
                {
                    
                    return "false";
                }


            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// Procedimiento para indicar que las sentencias SQL se ejecutaran en modo batch
        /// </summary>
        /// <param name="DB"></param>
        public void IniciarTransaccion(string DB)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                comando = new MySqlCommand("BEGIN", conexion);
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Procedimiento para llenar un objeto DataTable especifico
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara para buscar datos</param>
        /// <param name="Tabla_para_Datos">Objeto DataTable en el cual se insertaran los datos</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        public void LlenarTabla(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);

                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);

                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();

                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);
            }

            // Cerramos la conexion
            CerrarConexion();
        }

        public MySqlDataAdapter LlenarConsulta(string sentenciaSQL, string DB)
        {

            AbrirConexionServidor(DB);

            // Asignamos los parametros necesarios al adaptador de datos
            daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
            // Ejecutamos el comando del adaptador de datos
            cbSQL = new MySqlCommandBuilder(daSQL);

            return daSQL;
        }

        public string LlenarDTTableHTmlUser(string sentenciaSQL, DataTable Tabla_para_Datos, string DB, int Service)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                int validaEncabezado = 0;


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablaroles\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");

                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        if (dr[dc.ColumnName].ToString() == "True" || dr[dc.ColumnName].ToString() == "False")
                        {
                            string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "True" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                            sb.Append(input);
                        }
                        else if (dr[dc.ColumnName].ToString() == "1" || dr[dc.ColumnName].ToString() == "0")
                        {
                            if (validaEncabezado == 0)
                            {
                                sb.Append(dr[dc.ColumnName].ToString());
                                validaEncabezado = 1;
                            }
                            else
                            {
                                string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "1" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                                sb.Append(input);
                            }
                        }
                        else
                        {
                            sb.Append(dr[dc.ColumnName].ToString());
                        }


                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }



        public string LlenarDTTableHTmlEmpresa(string sentenciaSQL, DataTable Tabla_para_Datos, string DB, int Service)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                int validaEncabezado = 0;


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablaroles\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");

                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        if (dr[dc.ColumnName].ToString() == "True" || dr[dc.ColumnName].ToString() == "False")
                        {
                            string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "True" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                            sb.Append(input);
                        }
                        else if (dr[dc.ColumnName].ToString() == "1" || dr[dc.ColumnName].ToString() == "0")
                        {
                            // if (validaEncabezado == 0)
                            // {
                            //     sb.Append(dr[dc.ColumnName].ToString());
                            //     validaEncabezado = 1;
                            // }
                            // else
                            // {
                            string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "1" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                            sb.Append(input);
                            // }
                        }
                        else
                        {
                            sb.Append(dr[dc.ColumnName].ToString());
                        }


                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        //Se creo una nueva funcion para llenar una tabla HTML para el usuario para no alterar el resto de funciones que llenaban tablas.
        public string LlenarDTTableHTmlListarUsuario(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                //sb.Append("<table id = \"tabla\" class=\"table table-responsive table-bordered table-hover table-striped w-100\">");
                sb.Append("<table class=\"table table-responsive table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("<th>");
                sb.Append("#");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("#");
                sb.Append("</th>");



                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("<td>");
                    string usuario = dr.Field<string>("ROL");
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        string linkBaseEditar = "<a href = \"" + "/Usuario/Editar/" + usuario + "\" class =\"btn btn-warning stretched-link\"> Editar </a>";
                        sb.Append(linkBaseEditar);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        string linkBaseEliminar = "<button onclick = \"EliminarUsuario('" + usuario + "')\" class =\"btn btn-danger stretched-link\"> Eliminar </button>";
                        sb.Append(linkBaseEliminar);
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }


        //coca
        public string LlenarDDLLineaVehiculoMod(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();
                //sb.Append("<select id =\"lineavehiculo\" name=\"lineavehiculo\" class=\"form-control\">");

                for (int i = 0; i < Tabla_para_Datos.Rows.Count; i++)
                {
                    sb.Append("<option ");
                    sb.Append("value =\"");
                    sb.Append(Tabla_para_Datos.Rows[i]["ID"].ToString() + "\">");
                    sb.Append(Tabla_para_Datos.Rows[i]["LINEA"].ToString());
                    sb.Append("</option>");
                }

                //sb.Append("</select>");
                cTableHTml = sb.ToString();
            }
            CerrarConexion();
            return cTableHTml;
        }


        public string LlenarDTTableHTmlListarUsuarioWeb(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                //sb.Append("<table id = \"tabla\" class=\"table table-responsive table-bordered table-hover table-striped w-100\">");
                sb.Append("<table class=\"table table-responsive table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("<th>");
                sb.Append("#");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("#");
                sb.Append("</th>");



                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("<td>");
                    string usuario = dr.Field<string>("USUARIO");
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        string linkBaseEditar = "<a href = \"" + "/Usuario/EditarWebUser/" + usuario + "\" class =\"btn btn-warning stretched-link\"> Editar </a>";
                        sb.Append(linkBaseEditar);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        string linkBaseEliminar = "<button onclick = \"EliminarUsuario('" + usuario + "')\" class =\"btn btn-danger stretched-link\"> Eliminar </button>";
                        sb.Append(linkBaseEliminar);
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }







        //LlenarDTTableHTmlListarCajasParaAsignar
        public string LlenarDTTableHTmlListarCajasRegistradas(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tabla\" class=\"table table-responsive table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("<th>");
                sb.Append("Editar Caja");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Eliminar Caja");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Asignar Caja");
                sb.Append("</th>");
                

                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("<td>");
                    int caja = dr.Field<int>("id_caja");
                    string cajaString = caja.ToString();
                    if (!string.IsNullOrEmpty(cajaString))
                    {
                        string linkBaseEditar = "<a href = \"" + "/POS/Cajas/Editar/" + cajaString + "\" class =\"btn btn-warning stretched-link\"> Editar </a>";
                        sb.Append(linkBaseEditar);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        string linkBaseEliminar = "<button onclick = \"EliminarCaja('" + cajaString + "')\" class =\"btn btn-danger stretched-link\"> Eliminar </button>";
                        sb.Append(linkBaseEliminar);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        string linkBaseAsignar = "<a href = \"" + "/POS/Cajas/Asignar/" + cajaString + "\" class =\"btn btn-success stretched-link\"> Asignar Caja </a>";
                        sb.Append(linkBaseAsignar);
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        //LlenarDTTableHTmlListarCajasParaAsignar
        public string LlenarDTTableHTmlListarCajasAsignadas(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tabla\" class=\"table table-responsive table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("<th>");
                sb.Append("Desasignar Caja");
                sb.Append("</th>");


                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("<td>");
                    int caja = dr.Field<int>("id_caja");
                    string usuario = dr.Field<string>("usuario");

                    string cajaString = caja.ToString();
                    if (!string.IsNullOrEmpty(cajaString))
                    {
                        string linkBaseDesasignar = "<button onclick = \"DesasignarCaja('" + cajaString + "', '" + usuario + "')\" class =\"btn btn-secondary stretched-link\"> Desasignar Caja </button>";
                        sb.Append(linkBaseDesasignar);
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }




        public string LlenarDTTableSeguimientoTicket(string sentenciaSQL, DataTable Tabla_para_Datos, string DB, int Service)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                int validaEncabezado = 0;


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablaroles\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");

                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        if (dr[dc.ColumnName].ToString() == "True" || dr[dc.ColumnName].ToString() == "False")
                        {
                            string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "True" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                            sb.Append(input);
                        }
                        else if (dr[dc.ColumnName].ToString() == "1" || dr[dc.ColumnName].ToString() == "0")
                        {
                            if (validaEncabezado == 0)
                            {
                                sb.Append(dr[dc.ColumnName].ToString());
                                validaEncabezado = 1;
                            }
                            else
                            {
                                string input = "<input type='checkbox' " + (dr[dc.ColumnName].ToString() == "1" ? "checked" : "") + " onChange='changePermissionUser(this," + Service.ToString() + ")' >";
                                sb.Append(input);
                            }
                        }
                        else
                        {
                            sb.Append(dr[dc.ColumnName].ToString());
                        }


                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }







        public string LlenarDTTableHTmlListarCotizaciones(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";
            string cScript = "";

            cScript = "<script>   setTimeout(function () { ";
            cScript += "$('#tableCotizaciones').dataTable(";
            cScript += "{";
            cScript += "responsive: true,";
            cScript += "    lengthChange: false,";
            cScript += "    dom:";
            cScript += "    \" < 'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB >> \" +";
            cScript += "    \" < 'row'<'col-sm-12'tr >> \" +";
            cScript += "    \" < 'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p >> \",";
            cScript += "    buttons: [";
            cScript += "       {";
            cScript += "    extend: 'pdfHtml5',";
            cScript += "            text: 'PDF',";
            cScript += "            titleAttr: 'Generar PDF',";
            cScript += "            className: 'btn-outline-danger btn-sm mr-1'";
            cScript += "        },";
            cScript += "        {";
            cScript += "    extend: 'excelHtml5',";
            cScript += "            text: 'Excel',";
            cScript += "            titleAttr: 'Generar Excel',";
            cScript += "            className: 'btn-outline-success btn-sm mr-1'";
            cScript += "        },";
            cScript += "        {";
            cScript += "    extend: 'print',";
            cScript += "            text: 'Imprimir',";
            cScript += "            titleAttr: 'Imprimir Tabla de Datos',";
            cScript += "            className: 'btn-outline-primary btn-sm'";
            cScript += "        }";
            cScript += "    ]";
            cScript += "});}, 500);";
            cScript += "</script>";




            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();


                sb.Append("<table id='tableCotizaciones' class=\"table table-responsive table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("<th>");
                sb.Append("OPCIONES");
                sb.Append("</th>");

                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    double factura = dr.Field<double>("no_factura");
                    sb.Append("<td>");
                    string linkBaseEditar = "<a href = \"" + "/ConsultarCotizaciones/VerCotizacion/" + factura + "\" class =\"btn btn-info stretched-link\" target=\"_blank\"> Ver </a>";
                    sb.Append(linkBaseEditar);
                    sb.Append("</td>");


                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                sb.Append(cScript);
                cTableHTml = sb.ToString();
            }


            CerrarConexion();

            return cTableHTml;
        }

        //para obtener los permisos que dispone el usuario tanto en menus como en categorias, utilizando COOKIES 
        ////////////public HttpCookie EjecutarCommandoPermisosUsuarioMenus(string sentenciaSQL, string DB)
        ////////////{
        ////////////    // Abrimos la conexion y capturamos el error
        ////////////    int numero_error = AbrirConexionServidor(DB);

        ////////////    string[,] retorno = null;

        ////////////    HttpCookie cookiesMenu = new HttpCookie("cookiesMenus");


        ////////////    // Si no hay error ejecutarmos la consulta
        ////////////    if (numero_error == 0)
        ////////////    {
        ////////////        // Instruccion para capturar si existe error en el comando que se ejecutara
        ////////////        try
        ////////////        {
        ////////////            // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
        ////////////            comando = new MySqlCommand(sentenciaSQL, conexion);

        ////////////            // Ejecutamos el comando
        ////////////            MySqlDataReader reader = comando.ExecuteReader();

        ////////////            //Recorremos el resultado para asignarlo a un array para retornarlo mas adelante. 
        ////////////            while (reader.Read())
        ////////////            {
        ////////////                cookiesMenu[reader["clave"].ToString()] = reader["estado"].ToString();
        ////////////            }

        ////////////            // Cerramos la conexion al servidor
        ////////////            CerrarConexion();
        ////////////            return cookiesMenu;

        ////////////        }

        ////////////        catch //(MySqlException ex)
        ////////////        {
        ////////////            CerrarConexion();
        ////////////            //si hay error en la consulta, retornamos el string con null 
        ////////////            return cookiesMenu;
        ////////////        }
        ////////////    }
        ////////////    else
        ////////////    {
        ////////////        //si hay error en la consulta, retornamos el string con null 
        ////////////        return cookiesMenu;
        ////////////    }
        ////////////}



        public HttpCookie EjecutarCommandoPermisosUsuarioMenus(string sentenciaSQL, string DB)
        {
           
            HttpCookie cookiesMenu = new HttpCookie("cookiesMenus");


            using (IDbConnection dbConnection = new MySqlConnection(DB))
            {
                dbConnection.Open();

                // Realizar la consulta SQL utilizando Dapper
                
                IEnumerable<AccountUserMenus> accountsmenus = dbConnection.Query<AccountUserMenus>(sentenciaSQL);

                // Iterar a través de los resultados
                foreach (var accountmenu  in accountsmenus)
                {
                    cookiesMenu[accountmenu.clave.ToString()] = accountmenu.estado.ToString();
                }
                dbConnection.Close();
            }


            
            return cookiesMenu;
        }

        public HttpCookie EjecutarCommandoPermisosUsuarioCategorias(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            //int numero_error = AbrirConexionServidor(DB);


            HttpCookie cookiesCategorias = new HttpCookie("cookiesCategorias");



            using (IDbConnection dbConnection = new MySqlConnection(DB))
            {
                dbConnection.Open();

                // Realizar la consulta SQL utilizando Dapper

                IEnumerable<AccountUserMenus> accountsmenus = dbConnection.Query<AccountUserMenus>(sentenciaSQL);

                // Iterar a través de los resultados
                foreach (var accountmenu in accountsmenus)
                {
                    cookiesCategorias[accountmenu.clave.ToString()] = accountmenu.estado.ToString();
                }
                dbConnection.Close();
            }


            return cookiesCategorias;

            //// Si no hay error ejecutarmos la consulta
            //if (numero_error == 0)
            //{
            //    // Instruccion para capturar si existe error en el comando que se ejecutara
            //    try
            //    {
            //        // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
            //        comando = new MySqlCommand(sentenciaSQL, conexion);

            //        // Ejecutamos el comando
            //        MySqlDataReader reader = comando.ExecuteReader();

            //        //Recorremos el resultado para asignarlo a un array para retornarlo mas adelante. 
            //        while (reader.Read())
            //        {
            //            cookiesCategorias[reader["clave"].ToString()] = reader["estado"].ToString();
            //        }

            //        // Cerramos la conexion al servidor
            //        CerrarConexion();
            //        return cookiesCategorias;

            //    }

            //    catch //(MySqlException ex)
            //    {
            //        CerrarConexion();
            //        //si hay error en la consulta, retornamos el string con null 
            //        return cookiesCategorias;
            //    }
            //}
            //else
            //{
            //    //si hay error en la consulta, retornamos el string con null 
            //    return cookiesCategorias;
            //}
        }

        //para obtener datos del usuario para poder modificar los datos como el nombre y la contrase�a
        public Tuple<string, string> EjecutarCommandoUsuario(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Read();
                    var admin = reader["admin"].ToString();
                    var nombre = reader["nombre"].ToString();

                    Tuple<string, string> t = new Tuple<string, string>(admin, nombre);


                    // Cerramos la conexion al servidor
                    CerrarConexion();

                    //Devolver el valor
                    return t;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    Tuple<string, string> t = new Tuple<string, string>(null, null);
                    return t;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                Tuple<string, string> t = new Tuple<string, string>(null, null);
                return t;
            }
        }

        public Tuple<string, string> EjecutarCommandoCaja(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Read();
                    var id_caja = reader["id_caja"].ToString();
                    var nombre = reader["nombre"].ToString();

                    Tuple<string, string> t = new Tuple<string, string>(id_caja, nombre);


                    // Cerramos la conexion al servidor
                    CerrarConexion();

                    //Devolver el valor
                    return t;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    Tuple<string, string> t = new Tuple<string, string>(null, null);
                    return t;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                Tuple<string, string> t = new Tuple<string, string>(null, null);
                return t;
            }
        }

        public Tuple<string, string> EjecutarCommandoUsuarioAsignada(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);
            var usuario = "";
            var usuario2 = "";

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Read();

                    if (reader["usuario"] == null)
                    {
                        usuario = "";
                        usuario2 = "";
                    }
                    else
                    {
                        usuario = reader["usuario"].ToString();
                        usuario2 = reader["usuario"].ToString();
                    }

                    Tuple<string, string> t = new Tuple<string, string>(usuario, usuario2);


                    // Cerramos la conexion al servidor
                    CerrarConexion();

                    //Devolver el valor
                    return t;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    Tuple<string, string> t = new Tuple<string, string>("", "");
                    return t;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                Tuple<string, string> t = new Tuple<string, string>(null, null);
                return t;
            }
        }


        public List<ListaCorte> EjecutarComandoVerCorteCaja(string sentenciaSQL, string DB)
        {

            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);
            var corteLista = new List<ListaCorte>();

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        var corte = new ListaCorte
                        {
                            TOTAL = reader.GetString(reader.GetOrdinal("TOTAL")),
                            EFECTIVO = reader.GetString(reader.GetOrdinal("EFECTIVO")),
                            TARJETA = reader.GetString(reader.GetOrdinal("TARJETA")),
                            CHEQUE = reader.GetString(reader.GetOrdinal("CHEQUE")),
                            TRANSFERENCIAL = reader.GetString(reader.GetOrdinal("TRANSFERENCIA")),
                            VALE = reader.GetString(reader.GetOrdinal("VALE"))
                        };
                        corteLista.Add(corte);
                    }

                    CerrarConexion();

                    //Devolver el valor
                    return corteLista;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    return corteLista;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                return corteLista;
            }
        }


        //PARA VER UNA COTIZACION EjecutarComandoVerCorteCaja
        public List<Cotizacion> EjecutarCommandoVerCotizacion(string sentenciaSQL, string DB)
        {

            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);
            var cotizacionLista = new List<Cotizacion>();

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        var cotizacion = new Cotizacion
                        {
                            no_factura = reader.GetString(reader.GetOrdinal("no_factura")),
                            serie = reader.GetString(reader.GetOrdinal("serie")),
                            fecha = reader.GetString(reader.GetOrdinal("fecha")),
                            cliente = reader.GetString(reader.GetOrdinal("cliente")),
                            nit = reader.GetString(reader.GetOrdinal("nit")),
                            vendedor = reader.GetString(reader.GetOrdinal("vendedor")),
                            direccion = reader.GetString(reader.GetOrdinal("direccion")),
                            total = reader.GetString(reader.GetOrdinal("total")),
                            obs = reader.GetString(reader.GetOrdinal("obs")),
                            producto = reader.GetString(reader.GetOrdinal("producto")),
                            cantidad = reader.GetString(reader.GetOrdinal("cantidad")),
                            precio = reader.GetString(reader.GetOrdinal("precio")),
                            tdescto = reader.GetString(reader.GetOrdinal("tdescto")),
                            subtotal = reader.GetString(reader.GetOrdinal("subtotal")),
                            url = reader.GetString(reader.GetOrdinal("url")),
                            telefono = reader.GetString(reader.GetOrdinal("telefono")),
                            email = reader.GetString(reader.GetOrdinal("email")),
                            nombre_vendedor = reader.GetString(reader.GetOrdinal("nombre_vendedor")),
                            telefono_vendedor = reader.GetString(reader.GetOrdinal("telefono_vendedor")),
                            correo_vendedor = reader.GetString(reader.GetOrdinal("correo_vendedor")),
                            tasa = reader.GetString(reader.GetOrdinal("tasa")),
                            atencion = reader.GetString(reader.GetOrdinal("atencion")),
                        };
                        cotizacionLista.Add(cotizacion);
                    }

                    CerrarConexion();

                    //Devolver el valor
                    return cotizacionLista;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    return cotizacionLista;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                return cotizacionLista;
            }
        }

        //para obtener la URL de la ficha tecnica.
        public string EjecutarCommandoFichaTecnicaInventario(string sentenciaSQL, string DB)
        {
            // Abrimos la conexion y capturamos el error
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutarmos la consulta
            if (numero_error == 0)
            {
                // Instruccion para capturar si existe error en el comando que se ejecutara
                try
                {
                    // Creamos el comando con la instruccion SQL y la conexion al servidor de datos
                    comando = new MySqlCommand(sentenciaSQL, conexion);

                    // Ejecutamos el comando
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Read();
                    var url = reader["url"].ToString();

                    // Cerramos la conexion al servidor
                    CerrarConexion();

                    //Devolver el valor
                    return url;

                }

                catch //(MySqlException ex)
                {


                    CerrarConexion();
                    string url = null;
                    return url;
                }
            }
            else
            {
                // Devolvemos un valor para indicar que existio un error
                string url = null;
                return url;
            }
        }

        //////public string LlenarDTTableHTml(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        //////{
        //////    string cTableHTml = "";

        //////    // Abrimos la conexion
        //////    int numero_error = AbrirConexionServidor(DB);
        //////    // Si no hay error ejecutamos la lectura de datos
        //////    if (numero_error == 0)
        //////    {
        //////        // Asignamos los parametros necesarios al adaptador de datos
        //////        daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
        //////        // Ejecutamos el comando del adaptador de datos
        //////        cbSQL = new MySqlCommandBuilder(daSQL);
        //////        // Eliminamos la informacion de la tabla
        //////        Tabla_para_Datos.Clear();
        //////        // Llenamos la base de datos
        //////        daSQL.Fill(Tabla_para_Datos);


        //////        StringBuilder sb = new StringBuilder();
        //////        sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

        //////        sb.Append("<thead class=\"bg-primary-600\">");
        //////        sb.Append("<tr>");
        //////        foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////        {
        //////            sb.Append("<th>");
        //////            sb.Append(dc.ColumnName.ToString());
        //////            sb.Append("</th>");
        //////        }
        //////        sb.Append("</tr>");
        //////        sb.Append("</thead>");
        //////        sb.Append(" <tbody>");
        //////        foreach (DataRow dr in Tabla_para_Datos.Rows)
        //////        {
        //////            sb.Append("<tr>");
        //////            foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////            {
        //////                sb.Append("<td>");
        //////                sb.Append(dr[dc.ColumnName].ToString());
        //////                sb.Append("</td>");
        //////            }
        //////            sb.Append("</tr>");
        //////        }
        //////        sb.Append(" <tbody>");
        //////        sb.Append("</table>");
        //////        cTableHTml = sb.ToString();
        //////    }

        //////    CerrarConexion();

        //////    return cTableHTml;
        //////}


        public string LlenarDTTableHTml(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";


            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);


                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                        sb.Append("<thead class=\"bg-primary-600\">");
                        sb.Append("<tr>");
                        foreach (DataColumn dc in Tabla_para_Datos.Columns)
                        {
                            sb.Append("<th>");
                            sb.Append(dc.ColumnName.ToString());
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append(" <tbody>");
                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {
                            sb.Append("<tr>");
                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                if (dc.ColumnName.ToString() == "MEMO")
                                {
                                    sb.Append("<td> <a href='" + dr[dc.ColumnName].ToString() + "'>LinkMemo</a>");
                                    sb.Append("</td>");
                                }
                                else
                                {
                                    sb.Append("<td>");
                                    sb.Append(dr[dc.ColumnName].ToString());
                                    sb.Append("</td>");
                                }
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(" <tbody>");
                        sb.Append("</table>");
                        cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }

            return cTableHTml;

        }


        public string LlenarDTTableHTmlcxc(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";


            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);


                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table id = \"tablacxc\" class=\"table table-bordered table-hover table-striped w-100\">");

                        sb.Append("<thead class=\"bg-primary-600\">");
                        sb.Append("<tr>");
                        foreach (DataColumn dc in Tabla_para_Datos.Columns)
                        {
                            sb.Append("<th>");
                            sb.Append(dc.ColumnName.ToString());
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append(" <tbody>");
                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {
                            sb.Append("<tr>");
                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                if (dc.ColumnName.ToString() == "MEMO")
                                {
                                    sb.Append("<td> <a href='" + dr[dc.ColumnName].ToString() + "'>LinkMemo</a>");
                                    sb.Append("</td>");
                                }
                                else
                                {
                                    sb.Append("<td>");
                                    sb.Append(dr[dc.ColumnName].ToString());
                                    sb.Append("</td>");
                                }
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(" <tbody>");
                        sb.Append("</table>");
                        cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }

            return cTableHTml;

        }



        public string LlenarDDLLineaVehiculo(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();
                sb.Append("<select id =\"lineavehiculo\" name=\"lineavehiculo\" class=\"form-control\">");

                for (int i = 0; i < Tabla_para_Datos.Rows.Count; i++)
                {
                    sb.Append("<option ");
                    sb.Append("value =\"");
                    sb.Append(Tabla_para_Datos.Rows[i]["ID"].ToString() + "\">");
                    sb.Append(Tabla_para_Datos.Rows[i]["LINEA"].ToString());
                    sb.Append("</option>");
                }

                sb.Append("</select>");
                cTableHTml = sb.ToString();
            }
            CerrarConexion();
            return cTableHTml;
        }


        public string LlenarDDLCatLineasi(string sentenciaSQL, DataTable Tabla_para_Datos, string DB, string id_linea)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();
                sb.Append("<select id =\"catlineasi\" name=\"catlineasi\" class=\"form-control\">");

                for (int i = 0; i < Tabla_para_Datos.Rows.Count; i++)
                {
                    if (Tabla_para_Datos.Rows[i]["ID"].ToString().Trim() == id_linea) {
                        sb.Append("<option selected ");
                    } else
                    {
                        sb.Append("<option ");
                    }

                    sb.Append("value =\"");

                    sb.Append(Tabla_para_Datos.Rows[i]["ID"].ToString() + "\">");
                    sb.Append(Tabla_para_Datos.Rows[i]["DESCRIPCION"].ToString());
                    sb.Append("</option>");
                }       

                sb.Append("</select>");
                cTableHTml = sb.ToString();
            }
            CerrarConexion();
            return cTableHTml;
        }




        //////    public string LlenarDTTableHTmlN(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        //////{
        //////    string cTableHTml = "";

        //////    // Abrimos la conexion
        //////    int numero_error = AbrirConexionServidor(DB);
        //////    // Si no hay error ejecutamos la lectura de datos
        //////    if (numero_error == 0)
        //////    {
        //////        // Asignamos los parametros necesarios al adaptador de datos
        //////        daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
        //////        // Ejecutamos el comando del adaptador de datos
        //////        cbSQL = new MySqlCommandBuilder(daSQL);
        //////        // Eliminamos la informacion de la tabla
        //////        Tabla_para_Datos.Clear();
        //////        // Llenamos la base de datos
        //////        daSQL.Fill(Tabla_para_Datos);


        //////        StringBuilder sb = new StringBuilder();
        //////        sb.Append("<table class=\"table table-bordered table-hover table-striped w-100\">");

        //////        sb.Append("<thead class=\"bg-primary-600\">");
        //////        sb.Append("<tr>");
        //////        foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////        {
        //////            sb.Append("<th>");
        //////            sb.Append(dc.ColumnName.ToString());
        //////            sb.Append("</th>");
        //////        }
        //////        sb.Append("</tr>");
        //////        sb.Append("</thead>");
        //////        sb.Append(" <tbody>");
        //////        foreach (DataRow dr in Tabla_para_Datos.Rows)
        //////        {
        //////            sb.Append("<tr>");
        //////            foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////            {
        //////                sb.Append("<td>");
        //////                sb.Append(dr[dc.ColumnName].ToString());
        //////                sb.Append("</td>");
        //////            }
        //////            sb.Append("</tr>");
        //////        }
        //////        sb.Append(" <tbody>");
        //////        sb.Append("</table>");
        //////        cTableHTml = sb.ToString();
        //////    }

        //////    CerrarConexion();

        //////    return cTableHTml;
        //////}


        public string LlenarDTTableHTmlN(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";


            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);


                        StringBuilder sb = new StringBuilder();
                        // sb.Append("<table id=\"tablaroles\" class=\"table table-bordered table-hover table-striped w-100\">");
                        sb.Append("<table class=\"table table-bordered table-hover table-striped w-100\">");

                        sb.Append("<thead class=\"bg-primary-600\">");
                        sb.Append("<tr>");
                        foreach (DataColumn dc in Tabla_para_Datos.Columns)
                        {
                            sb.Append("<th>");
                            sb.Append(dc.ColumnName.ToString());
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append(" <tbody>");
                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {
                            sb.Append("<tr>");
                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(dr[dc.ColumnName].ToString());
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(" <tbody>");
                        sb.Append("</table>");
                        cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }




            return cTableHTml;
        }



        public string LlenarDTTableHTmlCliente(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        public string LlenarDTTableHTmlInventario(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablainventario\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionarinventario(this);\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        public string LlenarDTTableHTmlCotiz(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablacotiz\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionarcotiz(this);\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }


        //PARA EL POS
        //////////public string LlenarDTTableHTmlClientePOS(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        //////////{
        //////////    string cTableHTml = "";

        //////////    // Abrimos la conexion
        //////////    int numero_error = AbrirConexionServidor(DB);
        //////////    // Si no hay error ejecutamos la lectura de datos
        //////////    if (numero_error == 0)
        //////////    {
        //////////        // Asignamos los parametros necesarios al adaptador de datos
        //////////        daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
        //////////        // Ejecutamos el comando del adaptador de datos
        //////////        cbSQL = new MySqlCommandBuilder(daSQL);
        //////////        // Eliminamos la informacion de la tabla
        //////////        Tabla_para_Datos.Clear();
        //////////        // Llenamos la base de datos
        //////////        daSQL.Fill(Tabla_para_Datos);


        //////////        StringBuilder sb = new StringBuilder();
        //////////        sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

        //////////        sb.Append("<thead class=\"bg-primary-600\">");
        //////////        sb.Append("<tr>");
        //////////        foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////////        {
        //////////            sb.Append("<th>");
        //////////            sb.Append(dc.ColumnName.ToString());
        //////////            sb.Append("</th>");
        //////////        }
        //////////        sb.Append("</tr>");
        //////////        sb.Append("</thead>");
        //////////        sb.Append(" <tbody>");
        //////////        foreach (DataRow dr in Tabla_para_Datos.Rows)
        //////////        {
        //////////            sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
        //////////            foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //////////            {
        //////////                sb.Append("<td>");
        //////////                sb.Append(dr[dc.ColumnName].ToString());
        //////////                sb.Append("</td>");
        //////////            }
        //////////            sb.Append("</tr>");
        //////////        }
        //////////        sb.Append(" <tbody>");
        //////////        sb.Append("</table>");
        //////////        cTableHTml = sb.ToString();
        //////////    }

        //////////    CerrarConexion();

        //////////    return cTableHTml;
        //////////}


        public string LlenarDTTableHTmlClientePOS(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            //int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos

            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);


                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                        sb.Append("<thead class=\"bg-primary-600\">");
                        sb.Append("<tr>");
                        foreach (DataColumn dc in Tabla_para_Datos.Columns)
                        {
                            sb.Append("<th>");
                            sb.Append(dc.ColumnName.ToString());
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append(" <tbody>");
                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {
                            sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(dr[dc.ColumnName].ToString());
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(" <tbody>");
                        sb.Append("</table>");
                        cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }

            return cTableHTml;
        }



        public string LlenarDTTableHTmlClienteOS(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablapdf\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }



        public string LlenarTablaSeguimientoTicket(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();

                sb.Append("<table id = \"seguimientoticket\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }
            CerrarConexion();
            return cTableHTml;
        }









        public string LlenarDDLTipoProblema(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);

                StringBuilder sb = new StringBuilder();
                sb.Append("<select id =\"tipoproblema\" name=\"tipoproblema\" class=\"form-control\">");

                for (int i=0; i < Tabla_para_Datos.Rows.Count; i++)
                {
                    sb.Append("<option ");
                    sb.Append("value =\"");
                    sb.Append(Tabla_para_Datos.Rows[i]["ID"].ToString() + "\">");
                    sb.Append(Tabla_para_Datos.Rows[i]["PROBLEMA"].ToString());
                    sb.Append("</option>");
                }

                sb.Append("</select>");
                cTableHTml = sb.ToString();
            }
            CerrarConexion();
            return cTableHTml;
        }


        public List<SelectListItem> LLenarDropDownListCoca(string sentenciaSQL, string DB, List<SelectListItem> lista)
        {
            // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                comando = new MySqlCommand(sentenciaSQL, conexion);
                consulta = comando.ExecuteReader();

                while (consulta.Read())
                {
                    lista.Add(new SelectListItem { Text = consulta[1].ToString().Trim(), Value = consulta[0].ToString().Trim() });
                }
            }

            return lista;
        }





        //Coca
        public string Consulta(string sentenciaSQL, string DB)
        {

            // la Primera Chamba de Oscar
            // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
            //int numero_error = AbrirConexionServidor(DB);
            //string t = "";
            // Si no hay error ejecutamos la lectura de datos
            //if (numero_error == 0)
            //{
            //    comando = new MySqlCommand(sentenciaSQL, conexion);
            //    consulta = comando.ExecuteReader();

            //    while (consulta.Read())
            //    {
            //        t = consulta[0].ToString().Trim();
            //            //Value = consulta[0].ToString().Trim() });
            //    }
            //}
            string t = "";




            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (comando = new MySqlCommand(sentenciaSQL, conexion))
                {
                    using (consulta = comando.ExecuteReader())
                    {
                        if (consulta.Read())
                        {
                            // while (consulta.Read())
                            // {
                            t = consulta[0].ToString().Trim();
                            // }
                        }
                       
                    }
                    consulta.Close();
                }
                conexion.Close();
            }



            return t;
        }


       









        public string LlenarDTTableHTmlClienteCXC(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablapdf\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr id=\"fila\" onclick=\"seleccionar(this);\">");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }



        //oscar cxc

        public string LlenarDTTableHTmlCXC(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);
                string cId = "";

                StringBuilder sb = new StringBuilder();
                sb.Append("<table id = \"tablacxc\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    //sb.Append("<tr id=\"fila\" onclick=\"seleccionarEdicion(this);\">");
                    sb.Append("<tr id=\"fila\" >");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        if (dc.ColumnName == "ABONO")
                        {
                            cId = dr["SALDOACTUAL"].ToString();
                            //sb.Append("<td contenteditable='true' onchange=\"seleccionarEdicion("+ dr["IMPORTEORIGINAL"].ToString() +"|"+ dr["SALDOACTUAL"].ToString()+ ");\">");
                            sb.Append("<td contenteditable='true' onkeyup=\"seleccionarEdicion(" +cId + ");\" onClick=\"this.select();\">");
                        }
                        else
                        {
                            sb.Append("<td>");
                        }

                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        //public string LlenarDTTableHTmlInventarioPOS(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        //{
        //    string cTableHTml = "";

        //    // Abrimos la conexion
        //    int numero_error = AbrirConexionServidor(DB);
        //    // Si no hay error ejecutamos la lectura de datos
        //    if (numero_error == 0)
        //    {
        //        // Asignamos los parametros necesarios al adaptador de datos
        //        daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
        //        // Ejecutamos el comando del adaptador de datos
        //        cbSQL = new MySqlCommandBuilder(daSQL);
        //        // Eliminamos la informacion de la tabla
        //        Tabla_para_Datos.Clear();
        //        // Llenamos la base de datos
        //        daSQL.Fill(Tabla_para_Datos);


        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("<table id = \"tablainventario\" class=\"table table-bordered table-hover table-striped w-100\">");

        //        sb.Append("<thead class=\"bg-primary-600\">");
        //        sb.Append("<tr>");
        //        foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //        {
        //            sb.Append("<th>");
        //            sb.Append(dc.ColumnName.ToString());
        //            sb.Append("</th>");
        //        }
        //        sb.Append("</tr>");
        //        sb.Append("</thead>");
        //        sb.Append(" <tbody>");
        //        foreach (DataRow dr in Tabla_para_Datos.Rows)
        //        {
        //            sb.Append("<tr id=\"fila\" onclick=\"seleccionarinventario(this);\">");
        //            foreach (DataColumn dc in Tabla_para_Datos.Columns)
        //            {
        //                sb.Append("<td>");
        //                sb.Append(dr[dc.ColumnName].ToString());
        //                sb.Append("</td>");
        //            }
        //            sb.Append("</tr>");
        //        }
        //        sb.Append(" <tbody>");
        //        sb.Append("</table>");
        //        cTableHTml = sb.ToString();
        //    }

        //    CerrarConexion();

        //    return cTableHTml;
        //}

        public string LlenarDTTableHTmlInventarioPOS(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";



            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);


                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table id = \"tablainventario\" class=\"table table-bordered table-hover table-striped w-100\">");

                        sb.Append("<thead class=\"bg-primary-600\">");
                        sb.Append("<tr>");
                        foreach (DataColumn dc in Tabla_para_Datos.Columns)
                        {
                            sb.Append("<th>");
                            sb.Append(dc.ColumnName.ToString());
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append(" <tbody>");
                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {
                            sb.Append("<tr id=\"fila\" onclick=\"seleccionarinventario(this);\">");
                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(dr[dc.ColumnName].ToString());
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(" <tbody>");
                        sb.Append("</table>");
                        cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }


            return cTableHTml;



        }




        public string ScriptLlenarDTTableHTml(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";
            string cScript = "";

            cScript = "<script>";
            cScript += "$('#tabla').dataTable(";
            cScript += "{";
            cScript += "responsive: true,";
            cScript += "    lengthChange: false,";
            cScript += "    dom:";
            cScript += "    \" < 'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB >> \" +";
            cScript += "    \" < 'row'<'col-sm-12'tr >> \" +";
            cScript += "    \" < 'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p >> \",";
            cScript += "    buttons: [";
            cScript += "       {";
            cScript += "    extend: 'pdfHtml5',";
            cScript += "            text: 'PDF',";
            cScript += "            titleAttr: 'Generar PDF',";
            cScript += "            className: 'btn-outline-danger btn-sm mr-1'";
            cScript += "        },";
            cScript += "        {";
            cScript += "    extend: 'excelHtml5',";
            cScript += "            text: 'Excel',";
            cScript += "            titleAttr: 'Generar Excel',";
            cScript += "            className: 'btn-outline-success btn-sm mr-1'";
            cScript += "        },";
            cScript += "        {";
            cScript += "    extend: 'print',";
            cScript += "            text: 'Imprimir',";
            cScript += "            titleAttr: 'Imprimir Tabla de Datos',";
            cScript += "            className: 'btn-outline-primary btn-sm'";
            cScript += "        }";
            cScript += "    ]";
            cScript += "});";
            cScript += "</script>";



            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append(cScript);
                sb.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        public string ScriptLlenarDTTableHTmlSinFiltro(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";
            string cScript = "";

            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append(cScript);
                sb.Append("<div class=\"panel-content\"><table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");

                sb.Append("<thead class=\"bg-primary-600\">");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th>");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody>");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append(" <tbody>");
                sb.Append("</table></div>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }


        public string ScriptLlenarDTTableHTmlFreeze(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";
            string cScript = "";
            double nSum1 = 0;
            double nSum2 = 0;
            double nSum3 = 0;
            double ncontador = 0;
            Funciones f = new Funciones();
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);
            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);
                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);
                // Eliminamos la informacion de la tabla
                Tabla_para_Datos.Clear();
                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);


                StringBuilder sb = new StringBuilder();
                sb.Append(cScript);
                //sb.Append(" <div class=\"dt-basic-example_wrapper\"><table id = \"dt-basic-example\" role=\"grid\" class=\"table table-bordered table-hover table-striped w-100 dataTable dtr-inline\">");
                //sb.Append("<table id = \"dt-basic-example\" role=\"grid\" class=\"table table-bordered table-hover table-striped w-100 dataTable dtr-inline\">");
                sb.Append("<div  style=\"overflow-x:auto;\"><table id = \"dt-basic-example\" class=\"table table-bordered table-hover table-striped w-100\">");
                sb.Append("<thead");
                sb.Append("<tr>");
                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    sb.Append("<th class=\"sorting\">");
                    sb.Append(dc.ColumnName.ToString());
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append(" <tbody> <div class=\"dataTables_scrollBody\"");
                foreach (DataRow dr in Tabla_para_Datos.Rows)
                {
                    sb.Append("<tr role=\"row\" class=\"odd\" >");
                    
                    foreach (DataColumn dc in Tabla_para_Datos.Columns)
                    {
                       
                        sb.Append("<td>");
                        sb.Append(dr[dc.ColumnName].ToString());
                        sb.Append("</td>");
                        ncontador = ncontador + 1;
                        if (ncontador == 2)
                        {
                            nSum1 = nSum1 + Convert.ToDouble(dr[dc.ColumnName].ToString());
                        }
                        if (ncontador == 3)
                        {
                            nSum2 = nSum2 + Convert.ToDouble(dr[dc.ColumnName].ToString());
                        }
                        if (ncontador == 4)
                        {
                            nSum3 = nSum3 + Convert.ToDouble(dr[dc.ColumnName].ToString());
                        }


                    }
                    ncontador = 0;
                    sb.Append("</tr>");
                }
                sb.Append(" </tbody>");
                sb.Append("<tfoot>");

                sb.Append("<th class=\"sorting\">");
                sb.Append("T O T A L E S");
                sb.Append("</th>");


                sb.Append("<th class=\"sorting\">");
                sb.Append(f.FormatoDecimal(nSum1.ToString(),2,false));
                sb.Append("</th>");

                sb.Append("<th class=\"sorting\">");
                sb.Append(f.FormatoDecimal(nSum2.ToString(), 2, false));
                sb.Append("</th>");

                sb.Append("<th class=\"sorting\">");
                sb.Append(f.FormatoDecimal(nSum3.ToString(), 2, false));
                sb.Append("</th>");

                sb.Append("<th class=\"sorting\">");
                sb.Append("");
                sb.Append("</th>");



                //foreach (DataColumn dc in Tabla_para_Datos.Columns)
                //{
                //    sb.Append("<th class=\"sorting\">");
                //    sb.Append(dc.ColumnName.ToString());
                //    sb.Append("</th>");
                //}
                sb.Append("</tfoot>");

                sb.Append("</table></div>");
                cTableHTml = sb.ToString();
            }

            CerrarConexion();

            return cTableHTml;
        }

        //string oGraph = "labels: ['First quarter of the year', 'Second quarter of the year', 'Third quarter of the year', 'Fourth quarter of the year'],";
        //oGraph += "series: [ ";
        //oGraph += " [60000, 40000, 80000, 70000],";
        //oGraph += " [40000, 30000, 70000, 65000],";
        //oGraph += " [8000, 3000, 10000, 6000]";
        //oGraph += "]";

        public string LlenarDTTableHTmlPipeline(string sentenciaSQL, DataTable Tabla_para_Datos, DataTable Tabla_para_Datos2, string DB)
        {
            string str1 = "";
            Funciones funciones = new Funciones();
            string str2 = "<div class=\"d-flex flex-wrap demo demo-h-spacing mt-3 mb-3\"><div class=\"rounded-pill bg-white shadow-sm p-2 border-faded mr-3 d-flex flex-row align-items-center justify-content-center flex-shrink-0\">" + "<div class=\"ml-2 mr-3\"><h5 class=\"m-0\">";
            string str3 = "<small class=\"m-0 fw-300\">{OPPRO}</small></h5><a data-toggle=\"modal\" data-target=\".example-modal-centered-transparent\" title=\"{TOOLTIP}\"><i class=\"ni ni-check\" nombre =\"{ID} \"></i></a></div></div></div>";
            if (this.AbrirConexionServidor(DB) == 0)
            {
                this.daSQL = new MySqlDataAdapter(sentenciaSQL, this.conexion);
                this.cbSQL = new MySqlCommandBuilder(this.daSQL);
                Tabla_para_Datos.Clear();
                this.daSQL.Fill(Tabla_para_Datos);
                StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");
                // stringBuilder.Append("<thead class=\"bg-primary-600\">");
                stringBuilder.Append("<table id = \"tabla\" class=\"table table-bordered table-hover table-striped w-100\">");
                //stringBuilder.Append("<thead class=\"bg-warning-500\">");
                stringBuilder.Append("<thead>");
                stringBuilder.Append("<tr>");


                foreach (DataRow row in (InternalDataCollectionBase)Tabla_para_Datos.Rows)
                {
                    foreach (DataColumn column in (InternalDataCollectionBase)Tabla_para_Datos.Columns)
                    {
                        stringBuilder.Append("<th>");
                        stringBuilder.Append(row[column.ColumnName].ToString());
                        stringBuilder.Append("</th>");
                    }
                }
                stringBuilder.Append("</tr>");
                stringBuilder.Append("</thead>");
                stringBuilder.Append(" <tbody>");
                foreach (DataRow row in (InternalDataCollectionBase)Tabla_para_Datos2.Rows)
                {
                    stringBuilder.Append("<tr>");
                    foreach (DataColumn column in (InternalDataCollectionBase)Tabla_para_Datos2.Columns)
                    {
                        stringBuilder.Append("<td>");
                        if (row[column.ColumnName].ToString() != "")
                        {
                            string newValue1 = funciones.ObtieneDatos("oportunidades_prospectos", "CONCAT(nombre_prospecto,' ',oportunidad)", "id=" + row[column.ColumnName].ToString(), DB);
                            string newValue2 = funciones.ObtieneDatos("oportunidades_prospectos", "IF(tipo='O','OPORTUNIDAD','PROSPECTO')", "id=" + row[column.ColumnName].ToString(), DB);
                            stringBuilder.Append(str2);
                            stringBuilder.Append(newValue1);
                            stringBuilder.Append(str3.Replace("{TOOLTIP}", newValue1).Replace("{OPPRO}", newValue2).Replace("{ID}", row[column.ColumnName].ToString().Trim()));
                        }
                        stringBuilder.Append("</td>");
                    }
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append(" <tbody>");
                stringBuilder.Append("</table>");
                str1 = stringBuilder.ToString();
            }
            this.CerrarConexion();
            return str1;
        }

        public string GeneraGraficaBarras(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {


            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);

                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            string oGraph = "";
            oGraph = "labels: [";
            foreach (DataRow dr in Tabla_para_Datos.Rows)
            {

                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    if (dc.ColumnName == "NOMBRE_ESTABLECIMIENTO")
                        oGraph += "'" + dr[dc.ColumnName].ToString().Replace(",", "") + "',";
                }

            }

            oGraph += "],";
            oGraph += "series: [ " + "[";
            foreach (DataRow dr in Tabla_para_Datos.Rows)
            {

                foreach (DataColumn dc in Tabla_para_Datos.Columns)
                {
                    if (dc.ColumnName == "TOTAL")
                        oGraph += dr[dc.ColumnName].ToString().Replace(",", "") + ",";
                }

            }
            oGraph += "]],";



            

            return oGraph;
        }


        public void AddRowsTable(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);

                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);


                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);
            }

            // Cerramos la conexion
            CerrarConexion();
        }


        /// <summary>
        /// Procedimiento para llenar un objeto DataSet especifico
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara para buscar datos</param>
        /// <param name="Tabla_para_Datos">Objeto DataSet en el cual se insertaran los datos</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        public void LlenarDataSet(string sentenciaSQL, DataTable Data_para_Datos, string DB)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);

                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);

                // Eliminamos la informacion de la tabla
                Data_para_Datos.Clear();


                // Llenamos la base de datos
                daSQL.Fill(Data_para_Datos);
            }

            // Cerramos la conexion
            CerrarConexion();
        }




        /// <summary>
        /// Procedimiento para llenar un objeto DataTable especifico
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara para buscar datos</param>
        /// <param name="Tabla_para_Datos">Objeto DataTable en el cual se insertaran los datos</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        /// <param name="LimpiarTabla">Verdadero si se limpiara el objeto DataTable, en otro caso Falso</param>
        public void LlenarTabla(string sentenciaSQL, DataTable Tabla_para_Datos, string DB, bool LimpiarTabla)
        {
            // Abrimos la conexion
            int numero_error = AbrirConexionServidor(DB);

            // Si no hay error ejecutamos la lectura de datos
            if (numero_error == 0)
            {
                // Asignamos los parametros necesarios al adaptador de datos
                daSQL = new MySqlDataAdapter(sentenciaSQL, conexion);

                // Ejecutamos el comando del adaptador de datos
                cbSQL = new MySqlCommandBuilder(daSQL);

                // Verificamos si se eliminara la informacion contenida en el objeto DataTable
                if (LimpiarTabla == true)
                {
                    // Eliminamos la informacion de la tabla
                    Tabla_para_Datos.Clear();
                }

                // Llenamos la base de datos
                daSQL.Fill(Tabla_para_Datos);
            }

            // Cerramos la conexion
            CerrarConexion();
        }


        

        /// <summary>
        /// Procedimiento para cerrar la conexion al servidor
        /// </summary>
        /// 
        public void CerrarConexion()
        {

            if (conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
                conexion.Dispose();
            }

            
        }


        public int KillAllMySQL(string DB)
        {
            string query = "SHOW FULL PROCESSLIST";
            // 
            try
            {
                int numero_error = AbrirConexionServidor(DB);

                // Si no hay error ejecutamos la lectura de datos
                if (numero_error == 0)
                {
                    comando = new MySqlCommand(query, conexion);

                    // Ejecutamos el comando
                    comando.ExecuteNonQuery();


                    consulta = comando.ExecuteReader();
                    if (consulta.HasRows)
                    {
                        while (consulta.Read())
                        {
                            // kill processes with elapsed time > 200 seconds and in Sleep 
                            if (consulta.GetString(1) == "root")
                            {
                                //if (consulta.GetInt32(5) > 5 & consulta.GetString(4) == "Sleep" )
                                //{
                                //    KillMySqlProcess("KILL " + consulta.GetInt32(0), DB);
                                //}

                                //if (consulta.GetString(3) ==null & consulta.GetString(4) == "Sleep")
                                //{
                                    //KillMySqlProcess("KILL " + consulta.GetInt32(0), DB);
                                //}
                                if(!consulta.IsDBNull(3))
                                {
                                    if (consulta.GetString(3) == "dlempresa")
                                    {
                                        if (consulta.GetString(4) == "Sleep")
                                        {
                                            KillMySqlProcess("KILL " + consulta.GetInt32(0), DB);
                                        }
                                    }
                                }
                                else
                                {
                                    KillMySqlProcess("KILL " + consulta.GetInt32(0), DB);
                                }
                                
                            }
                        }
                    }
                    CerrarConexion();

                }

            }
            catch
            {
                return -1;
            }
            return 0;
        }

        public int KillMySqlProcess(string myQuery, string DB)
        {
            // 1. Create a query
            string query = myQuery;
            // 
            try
            {
                int numero_error = AbrirConexionServidor(DB);

                // Si no hay error ejecutamos la lectura de datos
                if (numero_error == 0)
                {
                    comando = new MySqlCommand(query, conexion);
                    comando.ExecuteNonQuery();
                    CerrarConexion();
                }


            }
            catch
            {
                return -1;
            }

            return 0;
        }


        //COCA
        public string LlenarItemsPDF(string sentenciaSQL, DataTable Tabla_para_Datos, string DB)
        {
            string cTableHTml = "";
            string subtotal = "";
            string linea = "";

            string itemEnderezado = "";
            double totalEnderezado = 0;

            string itemMecanica = "";
            double totalMecanica = 0;

            string itemRepuestosEx = "";
            double totalRepuestosEx = 0;

            string itemRepuestosIn = "";
            double totalRepuestosIn = 0;

            string itemOtros = "";
            double totalOtros = 0;

            double total = 0;
            double totalcompleto = 0;


            using (conexion = new MySqlConnection(DB))
            {
                // Abrir la conexión
                conexion.Open();

                // Realizar una consulta SQL

                using (daSQL = new MySqlDataAdapter(sentenciaSQL, conexion))
                {
                    using (cbSQL = new MySqlCommandBuilder(daSQL))
                    {
                        Tabla_para_Datos.Clear();
                        // Llenamos la base de datos
                        daSQL.Fill(Tabla_para_Datos);

                        StringBuilder sb = new StringBuilder();

                        foreach (DataRow dr in Tabla_para_Datos.Rows)
                        {

                            sb.Append("<tr class=xl71 height = 20 style='mso-height-source:userset;height:15.6pt'>");
                            sb.Append("<td height = 20 class=xl69 colspan = 2 style='height:15.6pt;mso-ignore:colspan'>");

                            foreach (DataColumn dc in Tabla_para_Datos.Columns)
                            {
                                if (dc.ColumnName.ToString() != "LINEA" && dc.ColumnName.ToString() != "SUBTOTAL")
                                {
                                    sb.Append(dr[dc.ColumnName].ToString() + " ");
                                }

                                if (dc.ColumnName.ToString() == "SUBTOTAL")
                                {
                                    subtotal = dr[dc.ColumnName].ToString();

                                }

                                if (dc.ColumnName.ToString() == "LINEA")
                                {
                                    linea = dr[dc.ColumnName].ToString();
                                }
                            }

                            sb.Append("</td>");
                            sb.Append("<td class=xl69></td>");
                            sb.Append("<td class=xl69></td>");
                            sb.Append("<td class=xl69></td>");
                            sb.Append("<td class=xl69></td>");
                            sb.Append("<td class=xl71></td>");
                            sb.Append("<td class=xl71></td>");
                            sb.Append("<td class=xl71></td>");
                            sb.Append("<td class=xl73 align = right >Q " + subtotal + "</td>");
                            sb.Append("<td class=xl71></td>");
                            sb.Append("<td class=xl71></td>");
                            sb.Append("</tr>");

                            total += Convert.ToDouble(subtotal);

                            switch (linea)
                            {
                                case "1":
                                    itemEnderezado += sb.ToString();
                                    totalEnderezado += total;
                                    break;

                                case "2":
                                    itemMecanica += sb.ToString();
                                    totalMecanica += total;
                                    break;

                                case "3":
                                    itemRepuestosIn += sb.ToString();
                                    totalRepuestosIn += total;
                                    break;

                                case "4":
                                    itemRepuestosEx += sb.ToString();
                                    totalRepuestosEx += total;
                                    break;

                                case "5":
                                    itemOtros += sb.ToString();
                                    totalOtros += total;
                                    break;
                            }

                            total = 0;
                            sb.Clear();

                        }

                        totalcompleto = totalEnderezado + totalMecanica + totalRepuestosEx + totalRepuestosIn + totalOtros;

                        cTableHTml = itemEnderezado + "|" + itemMecanica + "|" + itemRepuestosEx + "|" + itemRepuestosIn + "|" + itemOtros + "|" + totalEnderezado + "|" + totalMecanica + "|" + totalRepuestosEx + "|" + totalRepuestosIn + "|" + totalOtros + "|" + totalcompleto;


                        //cTableHTml = sb.ToString();
                    }

                }
                conexion.Close();
            }

            return cTableHTml;

        }



    }
}