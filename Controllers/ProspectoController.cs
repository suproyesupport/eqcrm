using System.Collections.Generic;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
  public class ProspectoController : Controller
  {
        public ActionResult IngClientes()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            string DB = (string)this.Session["StringConexion"];
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            List<SelectListItem> lista1 = new List<SelectListItem>();
            stringConexionMySql.LLenarDropDownList("SELECT id_codigo,nombre FROM vendedores", DB, lista1);
            this.ViewData["asesores"] = (object)lista1;
            //List<SelectListItem> lista2 = new List<SelectListItem>();
            //stringConexionMySql.LLenarDropDownList("SELECT id_prospecto,nombre FROM prospectos", DB, lista2);
            //this.ViewData["contacto1"] = (object)lista2;            
            
            List<SelectListItem> lista5 = new List<SelectListItem>();
            stringConexionMySql.LLenarDropDownList("SELECT id_tipo,descripcion FROM cattipoclie", DB, lista5);
            this.ViewData["tipoclie"] = (object)lista5;
            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.View();
        }

        public ActionResult IngProspectos()
    {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
              string DB = (string) this.Session["StringConexion"];
              StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
              List<SelectListItem> lista1 = new List<SelectListItem>();
              stringConexionMySql.LLenarDropDownList("SELECT id_codigo,nombre FROM vendedores", DB, lista1);
              this.ViewData["asesores"] = (object) lista1;
              List<SelectListItem> lista2 = new List<SelectListItem>();
              stringConexionMySql.LLenarDropDownList("SELECT id_prospecto,nombre FROM prospectos", DB, lista2);
              this.ViewData["contacto1"] = (object) lista2;
              List<SelectListItem> lista3 = new List<SelectListItem>();
              stringConexionMySql.LLenarDropDownList("SELECT id_fuente,nombre FROM catfuenteprospecto", DB, lista3);
              this.ViewData["fuente"] = (object) lista3;
              List<SelectListItem> lista4 = new List<SelectListItem>();
              stringConexionMySql.LLenarDropDownList("SELECT id_contacto,nombre FROM contactos", DB, lista4);
              this.ViewData["contacto2"] = (object) lista4;
              List<SelectListItem> lista5 = new List<SelectListItem>();
              stringConexionMySql.LLenarDropDownList("SELECT id_tipo,descripcion FROM cattipoclie", DB, lista5);
              this.ViewData["tipoclie"] = (object) lista5;
              stringConexionMySql.CerrarConexion();
              return (ActionResult) this.View();
    }
        public ActionResult IngCliente(DatosIngresoProspectos op)
        {
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            string nCodigoCliente = Funciones.NumeroMayor("clientes", "id_codigo", "", DB).ToString();
            string cfecha = f.dFechaServer(DB);
            string sentenciaSQL = "" + "INSERT INTO clientes (cliente,tipocliente,id_vendedor,direccion,nit,obs,email,id_codigo,atencion,telefono,fechain,fax,diascred,limcred) " + string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}')", (object)op.oportunidades.nombre_prospecto, (object)"P", (object)op.oportunidades.id_vendedor, (object)op.oportunidades.direccion, (object)op.oportunidades.nit, (object)op.oportunidades.observaciones, (object)op.oportunidades.email, nCodigoCliente.ToString(), (object)op.oportunidades.nombre_contacto, (object)op.oportunidades.telefono,cfecha.ToString(),"", (object)op.oportunidades.dias, (object)op.oportunidades.limite);
            stringConexionMySql.EjecutarCommando(sentenciaSQL, DB);
            stringConexionMySql.CerrarConexion();
            //return (ActionResult)this.RedirectToAction("IngProspectos", "IngClientes");
            return (ActionResult)this.RedirectToAction("IngClientes", "Prospecto");
            //return (ActionResult)this.View();
        }


        [HttpPost]
    public ActionResult IngContacto(DatosContactos contactos)
    {
      StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
      string DB = (string) this.Session["StringConexion"];
      string sentenciaSQL = "" + "INSERT INTO contactos (id_tipo,nombre,id_vendedor,direccion,telefono,celular,email,web_page) " + string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}')", (object) contactos.id_tipo, (object) contactos.nombre, (object) contactos.id_vendedor, (object) contactos.direccion, (object) contactos.telefono, (object) contactos.celular, (object) contactos.email, (object) contactos.web_page);
      stringConexionMySql.EjecutarCommando(sentenciaSQL, DB);
      stringConexionMySql.CerrarConexion();
      return (ActionResult) this.RedirectToAction("IngProspectos", "Prospecto");
    }

    [HttpPost]
    public ActionResult IngProspecto(DatosProspecto prospecto)
    {
      StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
      string DB = (string) this.Session["StringConexion"];
      string sentenciaSQL = "" + "INSERT INTO prospectos (id_tipo,nombre,id_vendedor,direccion,telefono,celular,email,web_page) " + string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}')", (object) prospecto.id_tipo, (object) prospecto.nombre, (object) prospecto.id_vendedor, (object) prospecto.direccion, (object) prospecto.telefono, (object) prospecto.celular, (object) prospecto.email, (object) prospecto.web_page);
      stringConexionMySql.EjecutarCommando(sentenciaSQL, DB);
      stringConexionMySql.CerrarConexion();
      return (ActionResult) this.RedirectToAction("IngProspectos", "Prospecto");
    }

     [HttpPost]
        public ActionResult IngProspectoCliente(DatosProspecto prospecto)
        {
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            string sentenciaSQL = "" + "INSERT INTO prospectos (id_tipo,nombre,id_vendedor,direccion,telefono,celular,email,web_page) " + string.Format("VALUES ({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}')", (object)prospecto.id_tipo, (object)prospecto.nombre, (object)prospecto.id_vendedor, (object)prospecto.direccion, (object)prospecto.telefono, (object)prospecto.celular, (object)prospecto.email, (object)prospecto.web_page);
            stringConexionMySql.EjecutarCommando(sentenciaSQL, DB);
            stringConexionMySql.CerrarConexion();
            return (ActionResult)this.RedirectToAction("IngProspectos", "IngClientes");
        }

        [HttpPost]
    public ActionResult IngLead(DatosIngresoProspectos op)
    {
      StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
      string DB = (string) this.Session["StringConexion"];
      string sentenciaSQL = "" + "INSERT INTO oportunidades_prospectos (nombre_prospecto,tipo,id_vendedor,id_prospecto,id_contacto,observaciones,id_fuente) " + string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}')", (object) op.oportunidades.nombre_prospecto, (object) "P", (object) op.oportunidades.id_vendedor, (object) op.oportunidades.id_prospecto, (object) op.oportunidades.id_contacto, (object) op.oportunidades.observaciones, (object) op.oportunidades.id_fuente);
      stringConexionMySql.EjecutarCommando(sentenciaSQL, DB);
      stringConexionMySql.CerrarConexion();
      return (ActionResult) this.RedirectToAction("IngProspectos", "Prospecto");
    }
      
       
    }
}
