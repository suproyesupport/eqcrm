using EqCrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm
{
    static class LlenarListaVentas
    {
        public static List<ListaVentas> listaVentas = new List<ListaVentas>();

        public static List<ClientesFac> listaClientes = new List<ClientesFac>();
        public static List<OrdenServicioFact> listaOrdenesServicio = new List<OrdenServicioFact>();
        public static List<FacturasNC> listaFacturas = new List<FacturasNC>();
    }
}