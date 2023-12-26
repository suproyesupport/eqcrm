
namespace EqCrm
{
    public class AccountPassword
    {
        public string usuario { get; set; }

        public string password { get; set; }

        public string id_sucursal { get; set; }

        public string intercompany { get; set; }


    }
}
//sentenciaSQL = "SELECT usuario, password, id_sucursal, intercompany FROM usuarios_web WHERE usuario ='" + acc.Name + "' AND password='" + acc.Password + "'";