using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace EqCrm.Controllers
{
    public class BackupsController : Controller
    {
        // GET: Backups
        [HttpPost]
        public string GetDataBackup(string cBackup)
        {
            string oBase = (string)this.Session["oBase"];
            string cRuta = @"C:\Backups\"+oBase+".sql";
            string DB = (string)this.Session["StringConexion"];
            DB += ";convertzerodatetime=true;";
            //DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();

                            mb.ExportInfo.AddCreateDatabase = true;
                            mb.ExportInfo.ExportTableStructure = true;
                            mb.ExportToFile(cRuta);      

                            conn.Close();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                cRuta = ex.Message;
            }

            return cRuta;
            
        }
    }
}