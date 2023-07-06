using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class Role : BaseTableEntity
  {
    [StringLength(255), Column(TypeName = "varchar")]
    public string Name { get; set; }
    [StringLength(1000), Column(TypeName = "varchar")]
    public string Description { get; set; }
    public string Pages { get; set; }
  }
}
