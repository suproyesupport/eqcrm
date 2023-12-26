using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Configuration;

namespace EqCrm
{
    public class ConexionMySQL
    {
        // Definimos las variables publicas que se utilizaran en el sistema
        public MySqlConnection conexion;
        public MySqlDataAdapter daSQL;
        public MySqlCommandBuilder cbSQL;
        public MySqlCommand comando;
        public MySqlDataReader consulta = null;


        /// <summary>
        /// Procedimiento que abre y verifica la conexion al servidor de datos
        /// </summary>
        /// <param name="Base_de_Datos">Nombre de la base de datos a consultar</param>
        /// <returns>Devolvera el numero de error si este existiera, 0 si no hay error</returns>

        public string generarStringDB(string Base_de_Datos)
        {
            
        
            string Cadena_Conexion = "";
            Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                    Properties.Settings.Default.DireccionServidor.ToString(),
                    Properties.Settings.Default.PuertoServidor.ToString(),
                    Properties.Settings.Default.Usuario.ToString(),
                    Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()),
                    Base_de_Datos);
            return Cadena_Conexion;
           
        }


        public string generarStringDlempresa()
        {


            string Cadena_Conexion = "";
            Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}",
                    Properties.Settings.Default.DireccionServidor.ToString(),
                    Properties.Settings.Default.PuertoServidor.ToString(),
                    Properties.Settings.Default.Usuario.ToString(),
                    Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()) );
            return Cadena_Conexion;

        }

        public int AbrirConexionServidor(string Base_de_Datos)
        {
            // Creamos una variable de cadena en blanco para guardar la instruccion de conexion al servidor
            string Cadena_Conexion = "";

            // Verificamos si se ingreso un nombre de base de datos
            if (Base_de_Datos == "")
            {
                //Cadena de conexion sin base de datos
                Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                    Properties.Settings.Default.DireccionServidor.ToString(),
                    Properties.Settings.Default.PuertoServidor.ToString(),
                    Properties.Settings.Default.Usuario.ToString(),
                    Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()),
                    Base_de_Datos);
                //Cadena_Conexion = string.Format("server={0}; port= {1}; user id={2}; password={3}",
                //    "localhost",
                //    "3306",
                //    "root",
                //    "alejandro");
            }
            else
            {
                // Cadena de conexion con base de datos
                Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                    Properties.Settings.Default.DireccionServidor.ToString(),
                    Properties.Settings.Default.PuertoServidor.ToString(),
                    Properties.Settings.Default.Usuario.ToString(),
                    Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()),
                    Base_de_Datos);
                //Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                //    "localhost",
                //    "3306",
                //    "root",
                //    "alejandro",
                //    Base_de_Datos);
            }

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
                            mensaje = "El usuario ingresado no existe o la contraseña es incorrecta";
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
        public bool EjecutarCommando(string sentenciaSQL, string DB, bool AbrirConexion)
        {
            // Declaramos una variable para almacenar el numero de error en la conexion SQL
            int numero_error = 0;

            // Verificamos si hay que abrir la conexion
            if (AbrirConexion == true)
            {
                // Abrimos la conexion y capturamos el error
                numero_error = AbrirConexionServidor(DB);
            }

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

                    // Devolvemos un valor falso para informar que no existe error
                    return false;
                }
                // Capturamos el error y lo controlamos para mostrar el mensaje especifico del error
                catch //(MySqlException ex)
                {
                    // Mostramos el mensaje de error
                    //MessageBox.Show(ex.Message.Substring(6), "Error No. " + ex.Number.ToString());

                    // Mostramos un mensaje con la sentencia ingresada
                    // MessageBox.Show(sentenciaSQL, "Informaci�n...");

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


        public List<SelectListItem> LLenarDropDownList(string sentenciaSQL, string DB, List<SelectListItem> lista)
        {
            ConexionMySQL mysql = new ConexionMySQL();
            DB = mysql.generarStringDB("dlempresa");

            //using (IDbConnection dbConnection = new MySqlConnection(DB))
            //{
            //    dbConnection.Open();

            //    // Realizar la consulta SQL utilizando Dapper                
            //    IEnumerable<DatosEmp> empresas = dbConnection.Query<DatosEmp>(sentenciaSQL);

            //    // Iterar a través de los resultados
            //    foreach (var emp in empresas)
            //    {
            //        lista.Add(new SelectListItem { Text = emp.nombre_empresa, Value = emp.base_datos });
            //    }
            //    dbConnection.Close();
            //}



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
                            lista.Add(new SelectListItem { Text = consulta[1].ToString(), Value = consulta[0].ToString() });
                        }
                    }
                    consulta.Close();
                }
                conexion.Close();
            }

            return lista;

            // Abrimos la conexion //lista.Add(new SelectListItem { Text = "Select", Value = "0" });
            //int numero_error = AbrirConexionServidor(DB);

            //// Si no hay error ejecutamos la lectura de datos
            //if (numero_error == 0)
            //{
            //    comando = new MySqlCommand(sentenciaSQL, conexion);
            //    consulta = comando.ExecuteReader();

            //    while (consulta.Read())
            //    {
            //        lista.Add(new SelectListItem { Text = consulta[1].ToString(), Value = consulta[0].ToString() });
            //    }
            //}

           // return lista;






        }
       

        /// <summary>
        /// Procedimiento para ejecutar lecturas de datos con Dapper
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        ///        
        public List<Encabezado_Documentos> ObtieneEncabezado(string sentenciaSQL, string DB, List<Encabezado_Documentos> listado)
        {

            
            string Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                   Properties.Settings.Default.DireccionServidor.ToString(),
                   Properties.Settings.Default.PuertoServidor.ToString(),
                   Properties.Settings.Default.Usuario.ToString(),
                   Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()),
                   DB);
            //string Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
            //    "localhost",
            //    "3306",
            //    "root",
            //    "alejandro",
            //    DB);

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

           string Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
                    Properties.Settings.Default.DireccionServidor.ToString(),
                    Properties.Settings.Default.PuertoServidor.ToString(),
                    Properties.Settings.Default.Usuario.ToString(),
                    Funciones.Base64Decode(Properties.Settings.Default.Password.ToString()),
                    DB);
            

            //string Cadena_Conexion = string.Format("server={0}; port = {1}; user id={2}; password={3}; database={4}",
            //   "localhost",
            //   "3306",
            //   "root",
            //   "alejandro",
            //   DB);

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


        /// <summary>
        /// Procedimiento para ejecutar lecturas de datos
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia SQL que se ejecutara (SELECT)</param>
        /// <param name="DB">Nombre de la base de datos en que se ejecutara la sentencia</param>
        public string EjecutarLectura(string sentenciaSQL, string DB)
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
        public void CerrarConexion()
        {

            if (conexion.State == System.Data.ConnectionState.Open)
            {

                conexion.Close();
                conexion.Dispose();
            }

            //
        }
    }
}