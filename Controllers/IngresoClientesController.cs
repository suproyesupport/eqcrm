using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngresoClientesController : Controller
    {
        // GET: IngresoClientes
        public ActionResult IngresoClientes()
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

        [HttpPost]
        public string IngCliente(DatosProspectosOportunidades op)
        {
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            string nCodigoCliente = Funciones.NumeroMayor("clientes", "id_codigo", "", DB).ToString();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";
            string cfecha = f.dFechaServer(DB);


            string coordinador = op.coor.ToString();

            if (coordinador == null ) { coordinador = string.Empty; }


            string sentenciaSQL = "" + "INSERT INTO clientes (cliente,tipocliente,id_vendedor,direccion,nit,obs,email,id_codigo,atencion,telefono,fechain,fax,diascred,limcred,pcomi,status,id_coordinador) " + 
                string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', {14}, '{15}', {16}) ", (object)op.cliente, (object)"P", (object)op.id_vendedor, (object)op.direccion, (object)op.nit, (object)op.observaciones, (object)op.email, nCodigoCliente.ToString(), (object)op.nombre_contacto, (object)op.telefono, cfecha.ToString(), "", (object)op.dias, (object)op.limite, (object)op.comision, "A", "0");

            try
            {
                lError = stringConexionMySql.ExecCommand(sentenciaSQL, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    stringConexionMySql.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;

                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"REGISTRO GUARDADO \", \"PRECIO\": 0}";

            stringConexionMySql.CerrarConexion();
            return str;

        }
    }
}