using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EqCrm.Models
{
    public class UsuarioPermiso
    {
        [Required]
        public int id { get; set; }
        [Required]
        public bool valor{ get; set; }
        [Required]
        public int servicio { get; set; }
        [Required]
        public string usuario { get; set; }
    }
}