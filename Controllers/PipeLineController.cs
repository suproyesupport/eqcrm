using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace EqCrm.Controllers
{
  public class PipeLineController : Controller
  {
    public ActionResult PipeLine()
    {
          string cUser = (string) this.Session["Usuario"];
          StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
          StringConexionMySQL subQuery = new StringConexionMySQL();
          DataTable Tabla_para_Datos = new DataTable();
          DataTable dataTable = new DataTable();
          string DB = (string)this.Session["StringConexion"];
          string sentenciaSQL1 = "";
          string strQueryPipe = "";
          string cCreateTemparoral;
          string cInst = "";
          string cDelete = "";
          string cEncabezado = "";

            if (string.IsNullOrEmpty(cUser))
          {
            return (ActionResult)this.RedirectToAction("Login", "Account");
          }

          int nContador = 1;
          int nFila = 1;

          using (MySqlConnection cn = new MySqlConnection(DB)) //configuramos la conexión
          {
                cn.Open();
                sentenciaSQL1 = "SELECT id_pipeline, nombre FROM catpipeline order by id_pipeline";
                //string cCreateTemparoral = " CREATE TEMPORARY TABLE IF NOT EXISTS  pipeline_temp ( ID int(11) NOT NULL AUTO_INCREMENT, ";
                cCreateTemparoral = " CREATE TEMPORARY TABLE IF NOT EXISTS  pipeline_temp ( ID int(11) NOT NULL AUTO_INCREMENT, ";

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.HasRows)
                {
                    while (stringConexionMySql.consulta.Read())
                    {
                        cCreateTemparoral += stringConexionMySql.consulta.GetString(1).Replace(" ", "") + " varchar(20) NOT NULL default'',  ";

                    }

                    cCreateTemparoral += "  PRIMARY KEY (ID) ";
                    cCreateTemparoral += "  ) ENGINE=InnoDB  ";
                }

               
                cDelete = "DELETE FROM pipeline_temp WHERE ";

                stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
                if (stringConexionMySql.consulta.HasRows)
                {
                    while (stringConexionMySql.consulta.Read())
                    {

                        strQueryPipe = "SELECT ";
                        strQueryPipe = strQueryPipe + " IF(a.id_pipeline = " + stringConexionMySql.consulta.GetString(0) + ", a.id ,'') AS " + stringConexionMySql.consulta.GetString(1).Replace(" ", "");
                        strQueryPipe = strQueryPipe + " FROM oportunidades_prospectos a " + " INNER JOIN catpipeline b ON (a.id_pipeline = b.id_pipeline) WHERE a.status = 'A' ORDER BY 1 DESC";
                        subQuery.EjecutarLectura(strQueryPipe, DB);

                        if (subQuery.consulta.HasRows)
                        {


                            if (nContador == 1)
                            {

                                while (subQuery.consulta.Read())
                                {
                                    cInst += " INSERT INTO pipeline_temp (" + stringConexionMySql.consulta.GetString(1).Replace(" ", "") + ") values ('" + subQuery.consulta.GetString(0) + "');";
                                    
                                }
                            }
                            else
                            {
                                while (subQuery.consulta.Read())
                                {
                                    cInst += " UPDATE pipeline_temp SET " + stringConexionMySql.consulta.GetString(1).Replace(" ", "") + "='" + subQuery.consulta.GetString(0)+"'";
                                    cInst += " WHERE ID=" + nFila.ToString()+";";

                                    
                                    nFila = nFila + 1;
                                }

                                nFila = 1;

                            }
                        }
                        cEncabezado=cEncabezado + stringConexionMySql.consulta.GetString(1).Replace(" ", "") + ",";
                        cDelete = cDelete + stringConexionMySql.consulta.GetString(1).Replace(" ", "") + "='' AND ";
                        nContador = nContador + 1;
                    }
                   
                }

               
                cDelete = cDelete + "AND";
                cDelete = cDelete.Replace("AND AND", "");

                using (var command = cn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = cCreateTemparoral;
                    command.ExecuteNonQuery();

                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM pipeline_temp";
                    command.ExecuteNonQuery();

                    command.CommandType = CommandType.Text;
                    command.CommandText = "ALTER TABLE pipeline_temp AUTO_INCREMENT=1";
                    command.ExecuteNonQuery();

                    command.CommandType = CommandType.Text;
                    command.CommandText = cInst;
                    command.ExecuteNonQuery();

                    command.CommandType = CommandType.Text;
                    command.CommandText =cDelete;
                    command.ExecuteNonQuery();
                }
               

            }

            cCreateTemparoral = "SELECT  "+cEncabezado; /// Hay que poner el encabezado

            cCreateTemparoral = cCreateTemparoral + " FROM pipeline_temp ORDER BY 1 DESC,2 DESC, 3 DESC,4 DESC";
            string sentenciaSQL2 = cCreateTemparoral.Replace(", FROM", " FROM");

            stringConexionMySql.LlenarTabla(sentenciaSQL2, dataTable, DB);

            string cCatPipeLine = "SELECT nombre FROM catpipeline order by id_pipeline";
            ViewBag.Tabla = stringConexionMySql.LlenarDTTableHTmlPipeline(cCatPipeLine, Tabla_para_Datos, dataTable, DB);


            List<SelectListItem> lista1 = new List<SelectListItem>();
            string sentenciaSQL4 = "SELECT id_pipeline,nombre FROM catpipeline";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL4, DB, lista1);
            this.ViewData["CatPipeLine"] = (object)lista1;

            List<SelectListItem> lista2 = new List<SelectListItem>();
            string sentenciaSQL5 = "SELECT id_tipoactividad,descripcion FROM cattipoactividad";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL5, DB, lista2);
            this.ViewData["TipoActividad"] = (object)lista2;

            List<SelectListItem> lista3 = new List<SelectListItem>();
            string sentenciaSQL6 = "SELECT id_status,descripcion FROM cattipoactividadstatus";
            stringConexionMySql.LLenarDropDownList(sentenciaSQL6, DB, lista3);
            this.ViewData["Status"] = (object)lista3;




            stringConexionMySql.KillAllMySQL(DB);



            return (ActionResult) this.View();
    }

    [HttpPost]
    public string GetDataOP(string Id)
    {
      DatosIngresoProspectos ingresoProspectos = new DatosIngresoProspectos();
      StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
      string str = "";
      string DB = (string) this.Session["StringConexion"];
      string sentenciaSQL1 = "SELECT json_object('NOMBRE', concat(a.oportunidad, ' ', a.nombre_prospecto)," + "'VENDEDOR', b.nombre," + "'DIAS', DATEDIFF(CURDATE(), a.fecha)," + "'CONTACTOPRINCIPAL',ifnull(c.nombre, '')," + "'CONTACTOSECUNDARIO',ifnull(d.nombre,'')," + "'PIPELINE',ifnull(a.id_pipeline, ''))" + " FROM oportunidades_prospectos a " + " LEFT JOIN vendedores b ON(a.id_vendedor = b.id_codigo)" + " LEFT JOIN prospectos c ON(a.id_prospecto = c.id_prospecto)" + " LEFT JOIN contactos d ON(a.id_contacto = d.id_contacto)" + " WHERE a.id = " + Id;
      stringConexionMySql.EjecutarLectura(sentenciaSQL1, DB);
            if (stringConexionMySql.consulta.Read())
            {
                str = stringConexionMySql.consulta.GetString(0).ToString();
            }

      //string sentenciaSQL2 = " select id_actividad as ID, fecha as FECHA,id_tipoactividad as TIPO,status as STATUS,actividad AS ASUNTO,seguimiento as DESCRIPCION ,fechaproxima as FECHASEGUIMIENTO from actividadescrm where id_op =" + Id;
      //DataTable Tabla_para_Datos = new DataTable();
      
      
      stringConexionMySql.CerrarConexion();
      return str;
    }

    [HttpPost]
    public ActionResult ActualizarDatos(DatosProspecto prospecto)
    {
      StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
      string DB = (string) this.Session["StringConexion"];
      stringConexionMySql.EjecutarCommando("UPDATE oportunidades_prospectos SET id_pipeline='" + prospecto.id_tipo + "' WHERE id=" + prospecto.id.ToString(), DB);
      stringConexionMySql.CerrarConexion();
      return RedirectToAction("PipeLine", "PipeLine");
    }

    [HttpPost]
    public string ActualizarActividad(string id,string id_asunto,string id_descripcion,string id_tipo_actividad,string id_status,string fechai,string fechap)
    {
      try
      {
        string str = "Actividad Actualizada con Exito";
        StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
        string DB = (string) this.Session["StringConexion"];
        string sentenciaSQL1 = "" + "INSERT INTO actividadescrm (id_op,fecha,actividad,seguimiento,fechaproxima,id_tipoactividad,status) " + string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}')", (object) id, (object) fechai, (object) id_asunto, (object) id_descripcion, (object) fechap, (object) id_tipo_actividad, (object) id_status);
        stringConexionMySql.EjecutarCommando(sentenciaSQL1, DB);
        string sentenciaSQL2 = " select id_actividad as ID, fecha as FECHA,id_tipoactividad as TIPO,status as STATUS,actividad AS ASUNTO,seguimiento as DESCRIPCION ,fechaproxima as FECHASEGUIMIENTO from actividadescrm where id_op =" + id;
        DataTable Tabla_para_Datos = new DataTable();
        
        stringConexionMySql.CerrarConexion();
        return str;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
  }
}
