using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
    public class UsuarioCategorias
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Id_categoria { get; set; }
        [Required]
        public int Estado { get; set; }
    }
}


