namespace EqCrm
{
  public class ClientesFac
  {
        //SELECT id_codigo AS CODIGO, cliente AS CLIENTE, direccion AS DIRECCION, nit AS NIT, diascred as DIASCREDITO, email AS CORREO, facturar AS FACTURAR FROM clientes
        public string CODIGO { get; set; }
        public string CLIENTE { get; set; }
        public string DIRECCION { get; set; }
        public string NIT { get; set; }
        public string DIASCREDITO { get; set; }
        public string CORREO { get; set; }
        public string FACTURAR { get; set; }

        public string id_status { get; set; }
        public string nombre { get; set; }

    }
}
