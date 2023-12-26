using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
    public class EqAppQuery
    {
        [Required]
        public string Nit { get; set; }
        [Required]
        public string Query { get; set; }
        [Required]
        public string BaseDatos { get; set; }
    }
}


