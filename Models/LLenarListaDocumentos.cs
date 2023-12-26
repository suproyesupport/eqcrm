using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EqCrm
{
    static class LlenarListaDocumentos
    {
        
        public static List<ListaDocumentos> listaDocumentos= new List<ListaDocumentos>();

        public static List<ListaOrdenesServicio> listaOrdenesServicio = new List<ListaOrdenesServicio>();

        public static List<tickets> tickets = new List<tickets>();

        public static List<ListaTicketProblema> listaTicketProblema = new List<ListaTicketProblema>();
        
        public static List<ListaTipoTicketProblema> listaTipoTicketProblema = new List<ListaTipoTicketProblema>();

        public static List<ListaClienteTicket> listaClienteTicket= new List<ListaClienteTicket>();

        public static List<ListaPrecioGAS> listaPreciosGAS = new List<ListaPrecioGAS>();

        public static List<ListaMovKardex> listaMovKardex = new List<ListaMovKardex>();

        public static List<ListaReporteClientesxSector> listaReporteClientesxSector = new List<ListaReporteClientesxSector>();

        public static List<ListaClientes> listaClientes = new List<ListaClientes>();
        public static List<ListaOrdenTecnica> listaOrdenTecnica = new List<ListaOrdenTecnica>();

    }
}