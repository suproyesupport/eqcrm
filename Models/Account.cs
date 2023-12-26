using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class Account
  {
    [Required]
    [EmailAddress]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    public string nPass1 { get; set; }


    public string id_sucursal { get; set; }
    public string intercompany { get; set; }

    }
}
