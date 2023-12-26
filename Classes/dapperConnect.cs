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
using DevExpress.Xpo.DB.Helpers;


/// Clase desarrollada por Julio Ponce, igual que la conexion a Mysql
/// <summary>
/// Clase pase para el menejo de conexiones con Dapper Mysql, se puede en un futuro parametrizar para sqlserver
/// </summary>
/// <Bitacora1>se empezo 2023-10-29 pero con dos funciones de momento muy utiles </Bitacora1>


namespace EqCrm
{
    public class dapperConnect
    {
        // Definimos las variables publicas que se utilizaran en el sistema

        /// <summary>
        /// Funcion que hace un crud a una tabla de Mysql, sin necesidad de manejar un modelo, desarrollado por Julio Ponce GT 2023
        /// </summary>
        /// <param name="sentenciaSQL"></param>
        /// <param name="DB"></param>
        /// <param name="cError"></param>
        /// <returns></returns>
        /// 
        public bool EqExecute(string sentenciaSQL, string DB, ref string cError)
        {
            
            try
            {
                using (IDbConnection dbConnection = new MySqlConnection(DB))
                {
                    dbConnection.Open();

                    int rowsAffected = dbConnection.Execute(sentenciaSQL);

                    dbConnection.Close();
                }

                return false;

            }            
            catch (MySqlException ex)
            {
                cError = "Error No. " + ex.Number.ToString() + "  " + ex.Message.ToString() + "  " + sentenciaSQL;                
                return true;
            }


        }


        /// <summary>
        /// Ejecuta un query update, delete con parametros automaticos
        /// </summary>
        /// <param name="sentenciaSQL"></param>
        /// <param name="DB"></param>
        /// <param name="cError"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>

        public bool EqExecuteParam(string sentenciaSQL, string DB, ref string cError, object[] parametros)
        {
            var param = parametros;
            try
            {
                using (IDbConnection dbConnection = new MySqlConnection(DB))
                {
                    dbConnection.Open();

                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            int rowsAffected = dbConnection.Execute(sentenciaSQL, param,transaction);

                            transaction.Commit();

                            return false;
                        }
                        catch (Exception)
                        {
                            // Ocurrió un error durante la ejecución de la consulta
                            transaction.Rollback();
                            throw; // Relanza la excepción para que se maneje en el bloque catch externo
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                cError = "Error No. " + ex.Number.ToString() + "  " + ex.Message.ToString() + "  " + sentenciaSQL;
                return true;
            }
        
        }




        /// <summary>
        /// Manejo de Listas automaticas, esto me llevo bastante tiempo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="query"></param>
        /// <param name="fechacreacion">2023-10-29</param>
        /// <param name="fechaultimamodificacion">2023-10-29</param>
        /// <returns></returns>
        public List<T> ExecuteList<T>(string connectionString, string query)
        {
            using (IDbConnection dbConnection = new MySqlConnection(connectionString))
            {
                dbConnection.Open();
                return dbConnection.Query<T>(query).AsList();
            }
        }


        //Coca
        public bool ValidaTablas(string db, string tabla)
        {
            bool respuesta = false;

            using (IDbConnection dbConnection = new MySqlConnection(db))
            {
                try
                {
                    dbConnection.Open();

                    // Consulta SQL para verificar la existencia de la tabla.
                    string sqlQuery = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '@Tabla'";
                    int resultado = dbConnection.ExecuteScalar<int>(sqlQuery, new { Tabla = tabla });

                    if (resultado == 1)
                    {
                        respuesta = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al verificar la existencia de la tabla: {ex.Message}");
                }
            }

            return respuesta;
        }


        //Coca
        public Object PrimerItem<T> (string db, string query)
        {
            using (IDbConnection dbConnection = new MySqlConnection(db))
            {
                dbConnection.Open();
                var ejemplo = dbConnection.QueryFirstOrDefault<T>(query);
                return ejemplo;
            }
        }
                



    }
}