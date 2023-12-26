using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class DataTipoJson
    {
        public string JSON { get; set; }
    }

    public class Resultado
    {
        public string resultado { get; set; }
        public List<DataTipoJson> Data { get; set; }
        
    }
}



