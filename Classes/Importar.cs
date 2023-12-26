using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.WebControls;

namespace EqCrm
{
    class Importar
    {
        OleDbConnection conn;
        OleDbDataAdapter MyDataAdapter;
        DataTable dt;

        public void importarExcel(GridView dgv,String nombreHoja,String cCampos,bool NombreColumnas, String ruta)
        {
           
            String cInst = "";
            try
            {
                if (NombreColumnas == true)
                {
                    conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;data source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'");
                }
                else
                {
                    conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;data source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=No'");
                }
                if (cCampos == "")
                {
                    MyDataAdapter = new OleDbDataAdapter("Select * from [" + nombreHoja + "$] ", conn);
                }
                else
                {
                    cInst = "Select " + cCampos + " from [" + nombreHoja + "$] ";
                    MyDataAdapter = new OleDbDataAdapter(cInst, conn);
                }
                    dt = new DataTable();
                    MyDataAdapter.Fill(dt);
                    dgv.DataSource = dt;
                    dgv.DataBind();                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
