using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
    public class Usuario
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Admin { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Depto { get; set; }
        [Required]
        public string Id_vendedor { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Tecnico { get; set; }
        [Required]
        public string Sucursal { get; set; }
    }
}


