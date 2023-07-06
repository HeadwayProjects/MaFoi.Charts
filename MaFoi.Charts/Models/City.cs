using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class City : BaseTableEntity
  {
    [StringLength(50), Column(TypeName = "varchar")]
    public string Code { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string Name { get; set; }
    public Guid StateId { get; set; }
    public State State { get; set; }
  }
}
