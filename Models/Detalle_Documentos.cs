using System;
using System.ComponentModel.DataAnnotations;

namespace EqCrm
{
  public class Detalle_Documentos
  {
    public string no_linea { get; set; }

    public string id_codigo { get; set; }

    public string codigoe { get; set; }

    public Decimal cantidad { get; set; }

    public string producto { get; set; }

    public Decimal preciou { get; set; }

    public Decimal descto { get; set; }

    [Display(Name = "Calculo del Subotal")]
    public Decimal ntotalitem
    {
      get
      {
        return this.cantidad * this.preciou - this.descto;
      }
    }

    public Decimal subtotal { get; set; }
  }
}
