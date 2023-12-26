using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqCrm.Controllers
{
    public class IngresoMovKardexController : Controller
    {
        // GET: IngresoMovKardex
        public ActionResult IngresoMovKardex()
        {
            if (string.IsNullOrEmpty((string)this.Session["Usuario"]))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }

            return (ActionResult)this.View();
        }



        [HttpPost]
        public string InsertarMovKardex(Inventario oInv)
        {
            string DB = (string)this.Session["StringConexion"];
            string str = "";
            bool lError;
            string cError = "";
            string cMensaje = "";

            StringConexionMySQL insertar = new StringConexionMySQL();

            try
            {
                string cInst = "INSERT INTO catkardex (id_movi, nombre, serie1) VALUES (";
                cInst += oInv.id + ", ";
                cInst += "'" + oInv.movimiento + "', ";
                cInst += "'" + oInv.serie + "'); ";
                lError = insertar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    insertar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            insertar.CerrarConexion();
            return str;
        }



        [HttpPost]
        public object GetMovKardex(Models.FiltroGenerico doctos)
        {
            string cUserConected = (string)(Session["Usuario"]);

            if (string.IsNullOrEmpty(cUserConected))
            {
                return (ActionResult)this.RedirectToAction("Login", "Account");
            }
            else
            {
                string DB = (string)this.Session["StringConexion"];
                string cInst = "";

                StringConexionMySQL llenar = new StringConexionMySQL();

                cInst = "SELECT id_movi AS ID, nombre AS MOVIMIENTO, serie1 AS SERIE ";
                cInst += "FROM catkardex ";
                cInst += "ORDER BY 1;";

                llenar.KillAllMySQL(DB);

                LlenarListaDocumentos.listaMovKardex = llenar.ListadoMovKardex(cInst, DB, LlenarListaDocumentos.listaMovKardex);

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(LlenarListaDocumentos.listaMovKardex);
        }



        [HttpPost]
        public string ModificarMovKardex(Inventario oInv)
        {
            string cUserConected = (string)(Session["Usuario"]);
            string str = "";

            string oDb = (string)(Session["StringConexion"]);
            StringConexionMySQL modifica = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "UPDATE catkardex SET ";
                cInst += "nombre = '" + oInv.movimientoo + "', ";
                cInst += "serie1 = '" + oInv.seriee + "' ";
                cInst += "WHERE id_movi = " + oInv.idd + ";";

                lError = modifica.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    modifica.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }

            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            modifica.CerrarConexion();
            return str;
        }



        [HttpPost]
        public string EliminarMovKardex(Inventario oInv)
        {
            StringConexionMySQL eliminar = new StringConexionMySQL();
            bool lError;
            string cError = "";
            string cMensaje = "";
            string str = "";

            string DB = (string)this.Session["StringConexion"];

            try
            {
                string cInst = "DELETE FROM catkardex WHERE ";
                cInst += "id_movi = " + oInv.id + "; ";

                lError = eliminar.ExecCommand(cInst, DB, ref cError);

                if (lError == true)
                {
                    cMensaje = cError;
                    eliminar.CerrarConexion();
                    str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + cError + "\", \"PRECIO\": 0}";
                    return str;
                }
            }
            catch (Exception ex)
            {
                str = "{\"CODIGO\": \"ERROR\", \"PRODUCTO\": \"" + ex.Message.ToString() + "\", \"PRECIO\": 0}";
            }

            str = "{\"CODIGO\": \"TRUE\", \"PRODUCTO\": \"PRODUCTO GUARDADO \", \"PRECIO\": 0}";

            eliminar.CerrarConexion();
            return str;
        }


    }
}


