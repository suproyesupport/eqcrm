using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers.OrdenC
{
    public class IngresoProveedoresController : Controller
    {
        // GET: IngresoProveedores
        public ActionResult IngresoProveedores()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            return (ActionResult)this.View();
        }

        [HttpPost]
        public string IngProveedor(DatosProspectosOportunidades op)
        {
            Funciones f = new Funciones();
            StringConexionMySQL stringConexionMySql = new StringConexionMySQL();
            string DB = (string)this.Session["StringConexion"];
            string nCodigoCliente = Funciones.NumeroMayor("proveedores", "id_codigo", "", DB).ToString();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";
            string cfecha = f.dFechaServer(DB);

            string sentenciaSQL = "" + "INSERT INTO proveedores (cliente,status,direccion,nit,obs,email,id_codigo,atencion,telefono,fechain,fax,diascred,limcred) " + string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')", (object)op.cliente, (object)"A", (object)op.direccion, (object)op.nit, (object)op.observaciones, (object)op.email, nCodigoCliente.ToString(), (object)op.nombre_contacto, (object)op.telefono, cfecha.ToString(), "", (object)op.dias, (object)op.limite);

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