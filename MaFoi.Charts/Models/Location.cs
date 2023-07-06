using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class Location : BaseTableEntity
  {
    public Guid CityId { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string Code { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string Name { get; set; }
    public City Cities { get; set; }
  }
}
