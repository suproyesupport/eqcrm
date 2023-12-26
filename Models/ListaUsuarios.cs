using System;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class ListaUsuarios
  {


        //select usuario, admin, nombre, depto, id_vendedor 
        //from usuarios;

        public string USUARIO { get; set; }
        public string ADMIN { get; set; }
        public string NOMBRE { get; set; }    
        public string DEPTO { get; set; }
        public string IDVENDEDOR { get; set; }

    }
}
