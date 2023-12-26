using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm
{
    public class ListaCorte
    {
        //PARA LLENAR LA TABLA DE LISTADO DE CORTE DE CAJA
        public string TIPODOCTO { get; set; }
        public string NO_DOCTO { get; set; }
        public string SERIE { get; set; }
        public string NIT { get; set; }
        public string OBS { get; set; }
        public string FECHA { get; set; }
        public string ENTRADA { get; set; }
        public string SALIDA { get; set; }
        public string CAJA { get; set; }
        public string SALDO { get; set; }
        //FIN DE PARAMETROS PARA LISTADO DE CORTE DE CAJA

        //PARA EL CORTE BOTON FILTRAR
        public string TOTAL { get; set; }
        public string EFECTIVO { get; set; }
        public string TARJETA { get; set; }
        public string CHEQUE { get; set; }
        public string TRANSFERENCIAL { get; set; }
        public string VALE { get; set; }
        //

        //PARA EL CORTE CON FILTRO DE FECHAS
        public string ID { get; set; }
        public string FECHACAJA { get; set; }
        public string EFECTIVOCONTADO { get; set; }
        public string TARJETACONTADO { get; set; }
        public string CHEQUECONTADO { get; set; }
        public string VALESCONTADO { get; set; }

        //FIN

    }
}